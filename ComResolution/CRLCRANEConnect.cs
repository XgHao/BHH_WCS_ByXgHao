using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LOG;
using System.Threading.Tasks;
using DataOperate;
using COMRW;
using System.Data;
using BaseData;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ComResolution
{
    public class CRLCRANEConnect : CRLBase
    {
        private CRCObject coc = new CRCObject();
        int n = 0;
        private Timer CheckTimer = null;
        private Timer GetStatusTimer = null;

        public static Log LogWrite = new Log("系统日志", @".\堆垛机日志\");

        #region DB块相关字段
        private const int dbnumber = 101;
        private const int wdbnumber = 100;
        private const ushort bytlength = 42;
        private const int value = 0;
        private static byte[] buffer;
        private static byte[] wbuffer;
        #endregion

        //将异常内容反馈给窗体
        public delegate void NotifyShow(string type, string msg);
        public event NotifyShow NotifyShowEvent;
        /// <summary>
        /// 1=二楼空托盘出库；2=一楼出库；3=二楼拣选；4=二楼入库；5=移库；6=召回
        /// </summary>
        private int runorder = 1;
        //private int OnJobNum = Convert.ToInt32(ConfigurationManager.AppSettings["OnJobNum"]);

        public CRLCRANEConnect(string ipadress, int port, string scno)
        {
            try
            {
                HsControlServer plcList = new HsControlServer(ipadress);
                if (!cPLCList.ContainsKey(scno)) 
                {
                    cPLCList.Add(scno, plcList);
                }
                else
                {
                    //如果已存在，更新链接信息
                    cPLCList[scno] = new HsControlServer(ipadress);
                }
                coc.IpAddress = ipadress;
                coc.Port = port;
                coc.ScNo = scno;

                CraneStr crs = new CraneStr();
                if (string.IsNullOrEmpty(scno)) 
                {
                    crs.Btid = scno;
                    CraneStrList.Add(crs);
                }
            }
            catch (Exception ex)
            {
                NotifyShowEvent?.Invoke("C", $"堆垛机{scno}初始化异常，异常信息为{ex.Message}");
            }
        }

        /// <summary>
        /// 连接堆垛机
        /// </summary>
        /// <returns></returns>
        public bool CarneConnect()
        {
            bool flag = cPLCList[coc.ScNo].HsServerConnect();
            var res = flag ? "连接成功" : "连接失败";
            LogWrite.WriteLog($"{res}，IP地址{coc.IpAddress}，堆垛机名称{coc.ScNo}，端口{coc.Port}");
            NotifyShowEvent?.Invoke("C", $"{res}，IP地址{coc.IpAddress}，堆垛机名称{coc.ScNo}，端口{coc.Port}");
            return flag;
        }

        /// <summary>
        /// 读取堆垛机信息
        /// </summary>
        /// <param name="co"></param>
        public void CraneRead(ref CRCObject co)
        {
            if (coc.connectstatus == 0) 
            {
                coc.connectstatus = 1;
                cPLCList[co.ScNo].HsServerCloseConnect();
                CarneConnect();
            }
            buffer = new byte[bytlength];
            bool flag = cPLCList[co.ScNo].HsRead($"DB{dbnumber}.{value}", bytlength, ref buffer);
            if (flag)
            {
                #region 入库出库
                runorder = co.runorder;
                if (runorder == 1)
                {
                    co.runorder = 2;
                }
                else if (runorder == 2) 
                {
                    co.runorder = 1;
                }
                #endregion

                BindCraneStr(co.ScNo, buffer);
            }
            else
            {
                coc.connectstatus = 0;
                NotifyShowEvent?.Invoke("C", $"堆垛机{co.ScNo}读取数据失败");
                cPLCList[co.ScNo].HsServerCloseConnect();
                CarneConnect();
            }
        }

        /// <summary>
        /// 将读取到的数据绑定到堆垛机实体类
        /// </summary>
        /// <param name="btid"></param>
        /// <param name="buffer"></param>
        public void BindCraneStr(string btid, byte[] buffer)
        {
            CraneStr cst = CraneStrList.Find(cs => cs.Btid == btid);

            if (cst != null) 
            {
                cst.Btid = btid; //设备ID
                cst.Czfs = buffer[2].ToString();    //操作方式（0、未定义  1、维修  2、手动  3、单机自动  4、联机自动）
                cst.Jdbz = buffer[3].ToString();    //阶段标志（0、待机  1、取货中  2、取货完成  3、放货中  4、放货完成）
                //cst.Zxrwh = buffer[4] * 256 * 256 * 256 + buffer[4] * 256 * 256 + buffer[6] * 256 + buffer[7];
                cst.Zxrwh = buffer[4] * 256 * 256 * 256 + buffer[5] * 256 * 256 + buffer[6] * 256 + buffer[7];  //任务号
                cst.Alarm = buffer[8].ToString();   //记录故障报警
                cst.Rkyh = "0";
                cst.Ckwh = "0";
                if (buffer[8].ToString() == "5")
                {
                    cst.Rkyh = "1"; //入库目标有货
                    cst.Dqzt = "2";
                }
                else if (buffer[8].ToString() == "6")
                {
                    cst.Ckwh = "1"; //出库目标无货
                    cst.Dqzt = "1";
                }
                else if (cst.Alarm != "0") 
                {
                    cst.Dqzt = "4";
                }
                else
                {
                    cst.Dqzt = "0";
                }
                cst.Dqph = buffer[38].ToString();   //获取堆垛机当前排
                cst.Dqlh = buffer[39].ToString();   //获取堆垛机当前列
                cst.Dqch = buffer[40].ToString();   //获取堆垛机当前层

                if (cst.Czfs == "4") 
                {
                    IssuedCraneTask(cst.Btid, buffer);
                }
            }
        }

        CRLTRANSControl crl = new CRLTRANSControl();
        /// <summary>
        /// 给堆垛机下达任务
        /// </summary>
        /// <param name="craneid"></param>
        /// <param name="buffer"></param>
        public void IssuedCraneTask(string craneid, byte[] buffer)
        {
            CraneStr cs = CRLBase.CraneStrList.Find(c => c.Btid == craneid);

            //任务结束，删除指令，放货完成
            if (cs.Jdbz == "4") 
            {
                cs.Zyfs = "4";
                LogWrite.WriteLog($"放货完成开始删除堆垛机{cs.Btid}任务{cs.Zxrwh}");
                NotifyShowEvent?.Invoke("R", $"防火完成开始删除堆垛机{cs.Btid}任务{cs.Zxrwh}");
                if (WriteToCrane(cs))
                {
                    DataSet dssch = DataTrans.D_GetSchByTaskno(cs.Zxrwh.ToString());
                    if (dssch == null)
                    {
                        return;
                    }
                    if (dssch.Tables[0].Rows.Count > 0)
                    {
                        //入库或者移库
                        if (dssch.Tables[0].Rows[0]["TASKTYPE"].ToString() == "1" || dssch.Tables[0].Rows[0]["TASKTYPE"].ToString() == "3")
                        {
                            DataTrans.D_CraneCommandTrans(cs.Btid, dssch.Tables[0].Rows[0]["TASKID"].ToString(), "4", "4", "4");
                            LogWrite.WriteLog($"放货完成删除堆垛机{cs.Btid}任务{cs.Zxrwh}完成 托盘条码：{dssch.Tables[0].Rows[0]["traycode"]}");
                            NotifyShowEvent?.Invoke("R", $"放货完成删除堆垛机{cs.Btid}任务{cs.Zxrwh}完成 托盘条码：{dssch.Tables[0].Rows[0]["traycode"]}");
                        }
                        //出库
                        else if (dssch.Tables[0].Rows[0]["TASKTYPE"].ToString() == "2")
                        {
                            DataTrans.D_CraneCommandTrans(cs.Btid, dssch.Tables[0].Rows[0]["TASKID"].ToString(), "3", "1", "3");
                            LogWrite.WriteLog($"放货完成删除堆垛机{cs.Btid}任务{cs.Zxrwh}完成 托盘条码：{dssch.Tables[0].Rows[0]["traycode"]}");
                            NotifyShowEvent?.Invoke("R", $"放货完成删除堆垛机{cs.Btid}任务{cs.Zxrwh}完成 托盘条码：{dssch.Tables[0].Rows[0]["traycode"]}");
                        }
                    }
                }
                //入库目标有货
                else if (cs.Rkyh == "1" && cs.Alarm != "0")
                {
                    //根据任务号获取调度指令
                    DataSet dsoldsh = DataTrans.D_GetSchByTaskno(cs.Zxrwh.ToString());

                    //判断是否入库或者移库
                    if ((dsoldsh != null && dsoldsh.Tables[0].Rows.Count > 0 && (dsoldsh.Tables[0].Rows[0]["tasktype"].ToString() == "1")) || dsoldsh.Tables[0].Rows[0]["tasktype"].ToString() == "3")
                    {
                        #region 向中间表插入数据，向WMS发起请求
                        //发起前先判断中间表是否已存在该条申请，状态为 1=已下发，2=已处理
                        DataSet dsisexist = DataTrans.D_GetIDX_ASRS_SEND(dsoldsh.Tables[0].Rows[0]["TASKID"].ToString(), "10", $"1','2");
                        if (dsisexist != null && dsisexist.Tables[0].Rows.Count == 0)
                        {
                            LogWrite.WriteLog($"堆垛机{cs.Btid}调度指令{dsoldsh.Tables[0].Rows[0]["taskid"]}入库货位有货，开始向WMS申请新货位...");
                            NotifyShowEvent?.Invoke("R", $"堆垛机{cs.Btid}调度指令{dsoldsh.Tables[0].Rows[0]["taskid"]}入库货位有货，开始向WMS申请新货位...");
                            string alleyid = AlleyIdRelation.GetAlleyId(cs.Btid);
                            //10代表存货占位，1代表下发
                            int n = DataTrans.D_InsertIDX_ASRS_SEND("10", "1", dsoldsh.Tables[0].Rows[0]["taskid"].ToString(), dsoldsh.Tables[0].Rows[0]["TRAYCODE"].ToString(), dsoldsh.Tables[0].Rows[0]["TCARGO_SPACE_ID"].ToString(), alleyid);
                            string res = n > 0 ? "成功" : "失败";
                            LogWrite.WriteLog($"堆垛机{cs.Btid}调度指令{dsoldsh.Tables[0].Rows[0]["taskid"]}入库货位有货，向WMS申请新货位{res}");
                            NotifyShowEvent?.Invoke("R", $"堆垛机{cs.Btid}调度指令{dsoldsh.Tables[0].Rows[0]["taskid"]}入库货位有货，向WMS申请新货位{res}");

                            //等待wms处理2s
                            Thread.Sleep(2000);
                        }
                        #endregion

                        //获取WMS重新分配的货位
                        DataSet dsidx = DataTrans.D_GetIDX_ASRS_SEND(dsoldsh.Tables[0].Rows[0]["TASKID"].ToString(), "10", "2");
                        //查询到数据说明获取到货位
                        if (dsidx != null && dsidx.Tables[0].Rows.Count > 0)
                        {
                            LogWrite.WriteLog($"堆垛机{cs.Btid}货位有货重新分配货位");
                            NotifyShowEvent?.Invoke("R", $"堆垛机{cs.Btid}货位有货重新分配货位");

                            string taskid = dsoldsh.Tables[0].Rows[0]["TASKID"].ToString();
                            string sendid = dsidx.Tables[0].Rows[0]["SENDID"].ToString();
                            //重分的货位号
                            string spaceid = dsidx.Tables[0].Rows[0]["LOCATION"].ToString();
                            //根据货位号获取货位信息
                            DataSet dsspace = DataTrans.P_getSpaceInfo(spaceid);
                            if (dsspace == null)
                            {
                                return;
                            }
                            //目标排号
                            string desrow = dsspace.Tables[0].Rows[0]["CSPLATOON"].ToString();
                            //目标列号
                            string descolnum = dsspace.Tables[0].Rows[0]["CSCOLUMN"].ToString();
                            //目标层号
                            string desfloor = dsspace.Tables[0].Rows[0]["CSFLOOR"].ToString();
                            //目标名称
                            string spacename = dsspace.Tables[0].Rows[0]["CARGO_SPACE_NAME"].ToString();

                            int remainder = Convert.ToInt32(desrow) % 2;
                            cs.Mbph = remainder == 0 ? "2" : "1";

                            cs.Mblh = descolnum;
                            cs.Mbch = desfloor;
                            AlleyIdRelation.GetRKDesSpace(cs.Btid, ref cs);
                            cs.Zyfs = "5";
                            if (WriteToCrane(cs))
                            {
                                LogWrite.WriteLog($"{cs.Btid}满入解警成功");
                                NotifyShowEvent?.Invoke("R", $"{cs.Btid}满入解警成功");
                                //更新相关表货位信息
                                DataTrans.D_GetSpaceUpInfo(taskid, cs.Btid, "3", sendid, spaceid, desrow, descolnum, desfloor, spacename);
                                cs.Zyfs = "1";

                                string res = WriteToCrane(cs) ? "成功" : "失败";

                                LogWrite.WriteLog($"给堆垛机{cs.Btid}重新分货位{spaceid}{res}");
                                NotifyShowEvent?.Invoke("R", $"给堆垛机{cs.Btid}重新分货位{spaceid}{res}");
                            }
                            else
                            {
                                LogWrite.WriteLog($"{cs.Btid}满入解警失败");
                                NotifyShowEvent?.Invoke("R", $"{cs.Btid}满入解警失败");
                            }
                        }
                    }
                }
                //出库无货
                else if (cs.Ckwh == "1" && cs.Alarm != "0")
                {
                    //根据任务号获取调度指令
                    DataSet dsoldsch = DataTrans.D_GetSchByTaskno(cs.Zxrwh.ToString());

                    //判断是否出库
                    //出库将旧指令删除即可，调度指令作废
                    if (dsoldsch != null && dsoldsch.Tables[0].Rows.Count > 0 && dsoldsch.Tables[0].Rows[0]["tasktype"].ToString() == "2")
                    {
                        #region 向中间表插入数据，向WMS发起请求
                        //发起前先判断中间表是否已存在该条申请
                        //状态  1=下发，2=已处理状态
                        DataSet dsisexist = DataTrans.D_GetIDX_ASRS_SEND(dsoldsch.Tables[0].Rows[0]["TASKID"].ToString(), "11", $"1','3',4','2");
                        if (dsisexist.Tables[0].Rows.Count == 0)
                        {
                            //判断出库目标无货无申请，则插入一条申请
                            string alleyid = AlleyIdRelation.GetAlleyId(cs.Btid);
                            //11=出库目标无货  1=下发
                            int n = DataTrans.D_InsertIDX_ASRS_SEND("11", "1", dsoldsch.Tables[0].Rows[0]["taskid"].ToString(), dsoldsch.Tables[0].Rows[0]["TRAYCODE"].ToString(), dsoldsch.Tables[0].Rows[0]["TCAGRO_SPACE_ID"].ToString(), alleyid);
                            if (n > 0)
                            {
                                //插入成功，将调度指令更新为作废，将中间表更新为已完成
                                int k = DataTrans.D_RKYHDel(cs.Btid, dsoldsch.Tables[0].Rows[0]["taskid"].ToString());
                                if (k > 0)
                                {
                                    string res = WriteToCrane(cs) ? "成功" : "失败";
                                    LogWrite.WriteLog($"{cs.Btid}空出解警成功");
                                    NotifyShowEvent?.Invoke("R", $"{cs.Btid}空出解警成功");
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        cs.Zyfs = "5";
                        string res = WriteToCrane(cs) ? "成功" : "失败";
                        LogWrite.WriteLog($"{cs.Btid}空出解警{res}");
                        NotifyShowEvent?.Invoke("R", $"{cs.Btid}空出解警{res}");
                    }
                }
                //待机
                else if (cs.Jdbz == "0") 
                {
                    #region 一楼出库
                    //一楼出库
                    if (runorder == 1 && n == 0) 
                    {
                        DataTable dt = new DataTable();
                        //查询一楼出库口输送机信息，设备类型为103
                        string dttype = "103";
                        TransportStr ts = lsTransport.Find(t => t.DTYPE == dttype && t.BTID == cs.Btid && t.KXBZ == "1");

                        if (ts == null) 
                        {
                            return;
                        }
                        crl.HsWcsReadSSJ(ref ts);
                        if (ts.KXBZ != "1") 
                        {
                            return;
                        }
                        //查询调度任务，
                        //任务类型2（TASKTYPE出库），任务类型描述（DESCRIPTION），状态为2（STATUS待执行）
                        string alleyid = AlleyIdRelation.GetAlleyId(cs.Btid);
                        //查询出库调度任务
                        //任务类型为2（TASKTYPE出库），层号（FLOOR）为1，状态为2（STATUS待执行）
                        DataSet dstask = DataTrans.D_GetOutSchTaskByCrane("2", "1", "2", alleyid, string.Empty, cs);
                        if (dstask != null && dstask.Tables[0].Rows.Count > 0) 
                        {
                            dt = dstask.Tables[0].Clone();
                            dt.ImportRow(dstask.Tables[0].Rows[0]);
                            bool flag1 = true;
                            //调度指令表中任务号不存在，分配新的任务号
                            if (dt.Rows[0]["task"].ToString() == "0" || string.IsNullOrEmpty(dt.Rows[0]["taskno"].ToString())) 
                            {
                                //任务号分配
                                string taskno = DataTrans.D_AllotTaskno().ToString();
                                if (!string.IsNullOrEmpty(taskno) && taskno != "0")
                                {
                                    dt.Rows[0]["taskno"] = taskno;
                                    flag1 = DataTrans.D_UpSchTask(taskno, dt.Rows[0]["taskid"].ToString());
                                }
                                else
                                {
                                    flag1 = false;
                                }
                            }
                            if (flag1)
                            {
                                LogWrite.WriteLog($"巷道{cs.Btid}开始一楼出库任务，任务号为{dt.Rows[0]["taskno"]}托盘条码：{dt.Rows[0]["traycode"]}");
                                NotifyShowEvent?.Invoke("R", $"巷道{cs.Btid}开始一楼出库任务，任务号为{dt.Rows[0]["taskno"]}托盘条码：{dt.Rows[0]["traycode"]}");
                                bool flag = DDJCommand(ref cs, dt);

                                if (flag)
                                {
                                    //根据巷道获取出库目标
                                    AlleyIdRelation.GetCKOneDesSpace(cs.Btid, ref cs);

                                    //出库
                                    cs.Zyfs = "2";

                                    if (WriteToCrane(cs))
                                    {
                                        LogWrite.WriteLog($"给堆垛机{cs.Btid}下发出库任务{dt.Rows[0]["taskno"]}成功，托盘条码：{dt.Rows[0]["traycode"]}，目标地址：{cs.Mbph}排{cs.Mblh}列{cs.Mbch}层");
                                        NotifyShowEvent?.Invoke("R", $"给堆垛机{cs.Btid}下发出库任务{dt.Rows[0]["taskno"]}成功，托盘条码：{dt.Rows[0]["traycode"]}，目标地址：{cs.Mbph}排{cs.Mblh}列{cs.Mbch}层");
                                        DataTrans.D_CraneCommandTrans(cs.Btid, dt.Rows[0]["TASKID"].ToString(), "2", "2", "3");
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        LogWrite.WriteLog($"给堆垛机{cs.Btid}下发出库任务{dt.Rows[0]["taskno"]}失败");
                                        NotifyShowEvent?.Invoke("R", $"给堆垛机{cs.Btid}下发出库任务{dt.Rows[0]["taskno"]}失败");
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 一楼入库
                    else if (runorder == 2)
                    {
                        DataTable dt = new DataTable();
                        //查询二楼入库口输送机信息，设备类型为207
                        string dttype = "102";
                        TransportStr ts = lsTransport.Find(s => s.DTYPE == dttype && s.BTID == cs.Btid && s.DWXH == "1");
                        if (ts == null) 
                        {
                            return;
                        }
                        else
                        {
                            crl.HsWcsReadSSJ(ref ts);
                            if (ts.DWXH != "1") 
                            {
                                return;
                            }
                            //根据输送机任务号获取调度信息
                            DataSet dstask = DataTrans.D_GetSchByTaskno(ts.ZXRWH);
                            DataRow[] drs = dstask.Tables[0].Select("TCARGO_SPACE_ID is not null", "");

                            if (drs.Length > 0) 
                            {
                                dt = dstask.Tables[0].Clone();
                                dt.ImportRow(drs[0]);
                                LogWrite.WriteLog($"开始入库任务，任务号为：{dt.Rows[0]["taskno"]}，托盘条码：{dt.Rows[0]["traycode"]}");
                                NotifyShowEvent?.Invoke("R", $"开始入库任务，任务号为：{dt.Rows[0]["taskno"]}，托盘条码：{dt.Rows[0]["traycode"]}");
                                bool flag = DDJCommand(ref cs, dt);
                                if (flag)
                                {
                                    //如果入库，目标地址为巷道
                                    AlleyIdRelation.GetRKDesSpace(cs.Btid, ref cs);
                                    cs.Zyfs = "1";
                                    //如果插入指令表成功，将指令写入到堆垛机中
                                    if (WriteToCrane(cs)) 
                                    {
                                        LogWrite.WriteLog($"给堆垛机{cs.Btid}下发任务{dt.Rows[0]["taskno"]}成功，托盘条码：{dt.Rows[0]["traycode"]}，目标地址{cs.Mbph}排-{cs.Mblh}列-{cs.Mbch}层");
                                        NotifyShowEvent?.Invoke("R", $"给堆垛机{cs.Btid}下发任务{dt.Rows[0]["taskno"]}成功，托盘条码：{dt.Rows[0]["traycode"]}，目标地址{cs.Mbph}排-{cs.Mblh}列-{cs.Mbch}层");
                                        DataTrans.D_CraneCommandTrans(cs.Btid, dt.Rows[0]["TASKID"].ToString(), "3", "3", "3");
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        LogWrite.WriteLog($"给堆垛机{cs.Btid}下发入库任务{dt.Rows[0]["taskno"]}失败");
                                        NotifyShowEvent?.Invoke("R", $"给堆垛机{cs.Btid}下发入库任务{dt.Rows[0]["taskno"]}失败");
                                    }
                                }
                                return;
                            }
                        }
                    }
                    #endregion
                }
            }
        }


        /// <summary>
        /// 给堆垛机指令表中插入指令数据
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool DDJCommand(ref CraneStr cs, DataTable dt)
        {
            bool flag = false;

            //用于存储指令表中的任务
            int cranetaskno = 0;

            cs.Zxrwh = dt.Rows[0]["taskno"].ToInt();
            if (dt.Rows[0]["TPLATOON"].ToInt().ToString().Equals("0"))
            {
                cs.Mbph = "0";
            }
            else
            {
                int remainder = Convert.ToInt32(dt.Rows[0]["TPLATOON"]) % 2;
                if (remainder == 0) 
                {
                    cs.Mbph = "2";
                }
                else
                {
                    cs.Mbph = "1";
                }
            }
            cs.Mblh = dt.Rows[0]["TCOLUMN"].ToString();
            cs.Mbch = dt.Rows[0]["TFLOOR"].ToString();
            if (dt.Rows[0]["SPLATOON"].ToInt().ToString().Equals("0")) 
            {
                cs.Dqph = "0";
            }
            else
            {
                int remainder = Convert.ToInt32(dt.Rows[0]["SPLATOON"]) % 2;
                if (remainder == 0) 
                {
                    cs.Dqph = "2";
                }
                else
                {
                    cs.Dqph = "1";
                }
            }
            cs.Dqlh = dt.Rows[0]["SCOLUMN"].ToString();
            cs.Dqch = dt.Rows[0]["SFLOOR"].ToString();

            //查询指令表中是否存在指令未执行
            DataSet dscommand = DataTrans.D_GetCraneCommand(cs.Btid);
            if (dscommand.Tables[0].Rows.Count > 0) 
            {
                cranetaskno = dscommand.Tables[0].Rows[0]["RWH"].ToInt();
            }
            if (cranetaskno > 0)
            {
                if (cranetaskno == Convert.ToInt32(dt.Rows[0]["TASKNO"]))
                {
                    LogWrite.WriteLog($"堆垛机{cs.Btid}对应任务号{cranetaskno}还未给设备下发，无法向堆垛机指令表中再次写入！");
                    NotifyShowEvent?.Invoke("R", $"堆垛机{cs.Btid}对应任务号{cranetaskno}还未给设备下发，无法向堆垛机指令表中再次写入！");
                    flag = true;
                }
                else
                {
                    LogWrite.WriteLog($"堆垛机{cs.Btid}对应任务号{cranetaskno}还未给设备下发,新任务{dt.Rows[0]["TASKNO"]}，无法向堆垛机指令表中再次写入！");
                    NotifyShowEvent?.Invoke("R", $"堆垛机{cs.Btid}对应任务号{cranetaskno}还未给设备下发，新任务{dt.Rows[0]["TASKNO"]}，无法向堆垛机指令表中再次写入！");
                    flag = false;
                    DataTrans.D_CraneCommanddel(cs.Btid);
                }
            }
            else
            {
                DataTrans.D_InsertCraneCommand(cs, dt);
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 将任务号等写入堆垛机
        /// </summary>
        /// <param name="ts"></param>
        public static bool WriteToCrane(CraneStr ts)
        {
            try
            {
                wbuffer = new byte[20];
                //作业方式。入库、出库、移库
                wbuffer[1] = (byte)(Convert.ToInt32(ts.Zyfs) % 256);
                //任务号
                wbuffer[2] = (byte)Convert.ToInt32(Convert.ToInt32(ts.Zxrwh) / (256 * 256 * 256));
                wbuffer[3] = (byte)Convert.ToInt32(Convert.ToInt32(ts.Zxrwh) / (256 * 256));
                wbuffer[4] = (byte)Convert.ToInt32(Convert.ToInt32(ts.Zxrwh) / 256);
                wbuffer[5] = (byte)Convert.ToInt32(Convert.ToInt32(ts.Zxrwh) - (wbuffer[0] * 256 * 256 * 256) - (wbuffer[1] * 256 * 256) - (wbuffer[2] * 256));

                //目标排
                wbuffer[6] = (byte)(Convert.ToInt32(ts.Mbph) % 256);
                wbuffer[7] = (byte)(Convert.ToInt32(ts.Mblh) % 256);
                wbuffer[8] = (byte)(Convert.ToInt32(ts.Mbch) % 256);

                //源地址排
                wbuffer[12] = (byte)(Convert.ToInt32(ts.Dqph) % 256);
                //源地址列
                wbuffer[13] = (byte)(Convert.ToInt32(ts.Dqlh) % 256);
                //源地址层
                wbuffer[14] = (byte)(Convert.ToInt32(ts.Dqch) % 256);
                bool flag = cPLCList[ts.Btid].HsWrite($"DB{wdbnumber}.0", buffer);
                return true;
            }
            catch (Exception ex)
            {
                LogWrite.WriteLog($"堆垛机写入异常，异常信息为：{ex.Message}");
                return false;
            }
        }



        
    }
}
