using BaseData;
using COMRW;
using DataOperate;
using LedDisplay;
using LOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeSpanCount;
using LedDisplay;
using System.Security.Cryptography;

namespace ComResolution
{
    public class CRLTRANSControl : CRLBase
    {
        /// <summary>
        /// 一楼输送机字节
        /// </summary>
        private byte[] firstbuffer;
        /// <summary>
        /// 一楼电机字节
        /// </summary>
        private byte[] firstdjbuffer;
        /// <summary>
        /// 二楼输送机字节
        /// </summary>
        private byte[] secondbuffer;
        /// <summary>
        /// 二楼电机字节
        /// </summary>
        private byte[] seconddjbuffer;

        private PerformanceTimer pft = new PerformanceTimer();
        public static Log logWrite = new Log("系统日志", @".\输送机日志\");

        //将消息发送给界面
        public delegate void NotifyDg(string msg);
        public event NotifyDg NotifyEvent;

        public delegate void DescTransportInfoDG(TransportStr si);
        public event DescTransportInfoDG DescTransportInfoEvent;

        public delegate void GetEquipMentInfoDG(List<TransportStr> transportStrs);
        public event GetEquipMentInfoDG GetEquipMentInfoEvent;

        //对输送机下达任务
        public delegate void IssuedDG(byte[] firstbuffer, byte[] firstdjbuffer, byte[] secondbuffer, byte[] seconddjbuffer);
        public event IssuedDG IssuedEvent;

        //输送机已下发任务号
        Dictionary<string, string> HasIssued = new Dictionary<string, string>();

        /// <summary>
        /// 写db块
        /// </summary>
        private int dbnumber = 55;
        /// <summary>
        /// 开始字节
        /// </summary>
        private int value = 0;
        /// <summary>
        /// 电机db块
        /// </summary>
        private int djdbnumber = 54;

        LedServer led = new LedServer();
        /// <summary>
        /// 用于记录给LED发送过的信息
        /// </summary>
        Dictionary<string, string> dcsaveledstr = new Dictionary<string, string>();
        /// <summary>
        /// 获取到的消息
        /// </summary>
        string rcgmsg;

        /// <summary>
        /// 初始化输送机对应参数
        /// </summary>
        /// <returns></returns>
        public bool TransDeviceInit()
        {
            DataSet ds = DataTrans.D_GetDeviceSSJ();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //获取设备编号第一个字母，用于与绑定IP地址
                    _SSJDhead = ds.Tables[0].Rows[i]["VAR1"].ToString().Substring(0, 1);
                    string _ip = ConfigurationManager.AppSettings[_SSJDhead].ToString();

                    //需要改进，将端口，DB块，端口等也进行构造函数初始化l
                    if (!PLCList.ContainsKey(_SSJDhead))
                    {
                        PLCList.Add(_SSJDhead, new RWLOPCServerl(_ip));
                        PLCFlag.Add(_SSJDhead, false);
                        PLCList[_SSJDhead].Connect();
                    }
                    if (!HsPLCList.ContainsKey(_SSJDhead))
                    {
                        HsPLCList.Add(_SSJDhead, new HsControlServer(_ip));
                        bool flag = HsPLCList[_SSJDhead].HsServerConnect();
                        string res = flag ? "成功" : "失败";
                        NotifyEvent?.Invoke($"PLC{_ip}连接{res}");
                    }

                    TransportStr tp = new TransportStr();
                    tp.SSJID = ds.Tables[0].Rows[i]["SSJID"].ToString();
                    tp.BTID = ds.Tables[0].Rows[i]["BTID"].ToString();
                    tp.DTYPE = ds.Tables[0].Rows[i]["DTYPE"].ToString();
                    tp.ZXRWH = ds.Tables[0].Rows[i]["ZXRWH"].ToString();
                    tp.DWXH = ds.Tables[0].Rows[i]["DWXH"].ToString();
                    tp.KXBZ = ds.Tables[0].Rows[i]["KXBZ"].ToString();
                    tp.TRAYCODE = ds.Tables[0].Rows[i]["TRAYCODE"].ToString();
                    tp.JYM = ds.Tables[0].Rows[i]["JYM"].ToString();
                    tp.BFLAG = ds.Tables[0].Rows[i]["BFLAG"].ToString();
                    tp.ALLEYID = ds.Tables[0].Rows[i]["ALLEYID"].ToString();
                    //tp.ALLEYID = "";
                    tp.VAR1 = ds.Tables[0].Rows[i]["VAR1"].ToString();
                    tp.VAR2 = ds.Tables[0].Rows[i]["VAR2"].ToString();
                    tp.VAR3 = ds.Tables[0].Rows[i]["VAR3"].ToString();
                    tp.VAR4 = ds.Tables[0].Rows[i]["VAR4"].ToString();
                    tp.VAR5 = ds.Tables[0].Rows[i]["VAR5"].ToString();
                    tp.SSRWLX = ds.Tables[0].Rows[i]["SSRWLX"].ToString();
                    tp.SSJIDhead = _SSJDhead;
                    lsTransport.Add(tp);
                }
                return true;
            }
            return false;
        }

        public void Run()
        {
            ComTask = Task.Run(() =>
            {
                while (true)
                {
                    //下发任务
                    IssusedTask();
                    Thread.Sleep(500);
                }
            });
        }


        #region 入库调度指令处理
        Dictionary<string, double> dcoldweight = new Dictionary<string, double>();
        private bool SSJInWareCommand(ref TransportStr si)
        {
            bool flag = false;
            if (!HsWcsReadSSJ(ref si))
            {
                flag = false;
                return false;
            }
            si.TRAYCODE = string.Empty;

            if (si.Weight > 700) 
            {
                NotifyEvent?.Invoke($"货物超重，请注意！重量为{si.Weight}");
                logWrite.WriteLog($"货物超重，请注意！重量为{si.Weight}");
                string str = $"称重重量：{Math.Floor(si.Weight)}\r货物超重，请注意!";
                if (dcsaveledstr[si.SSJID] != str) 
                {
                    LedSendStr(si.SSJID, str, 2);
                }
                return false;
            }
            if (si.Weight > 1) 
            {
                flag = true;
                DataSet ds = new DataSet();
                DataTrans.D_GetInwareTask(si.SSJID);
                if (ds != null && ds.Tables[0].Rows.Count > 0) 
                {
                    if (ds.Tables[0].Rows[0]["RESERVE3"]?.ToString() != "1") 
                    {
                        DataTrans.P_UpActWeight(ds.Tables[0].Rows[0]["TASKID"].ToString().Trim(), si.Weight.ToString());
                    }
                    si.TRAYCODE = ds.Tables[0].Rows[0]["TRAYCODE"].ToString().Trim();
                    string str = $"托盘条码{ds.Tables[0].Rows[0]["TRAYCODE"].ToString().Trim()}\r物料编码{ds.Tables[0].Rows[0]["productcode"].ToString().Trim()}\r物料名称：{ds.Tables[0].Rows[0]["productname"].ToString().Trim()}\r物料批次：{ds.Tables[0].Rows[0]["lotinfo"].ToString().Trim()}\r重量/称重重量：{ds.Tables[0].Rows[0]["assignnum"].ToString().Trim()}/{Math.Floor(si.Weight)}";
                    if (dcsaveledstr[si.SSJID] != str) 
                    {
                        LedSendStr(si.SSJID, str, 0);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    string str = $"称重重量：{Math.Floor(si.Weight).ToString()}\r";
                    if (dcsaveledstr[si.SSJID] != str) 
                    {
                        LedSendStr(si.SSJID, str, 2);
                    }
                    else
                    {
                        flag = false;
                    }
                    logWrite.WriteLog($"请确定输送机{si.SSJID}是否有创建任务！");
                }
            }
            else
            {
                string str = string.Empty;
                if (dcsaveledstr[si.SSJID] != str) 
                {
                    LedSendStr(si.SSJID, str, 2);
                }
            }
            return flag;
        }
        #endregion

        /// <summary>
        /// 下发任务
        /// </summary>
        private void IssusedTask()
        {
            try
            {
                #region 入库口输送机
                for (int i = 1; i <= 4; i++) 
                {
                    TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "101");

                    if (devicessj!=null)
                    {
                        if (!dcsaveledstr.ContainsKey(devicessj.SSJID)) 
                        {
                            dcsaveledstr.Add(devicessj.SSJID, "TMP");
                        }
                        if (!dcoldweight.ContainsKey(devicessj.SSJID)) 
                        {
                            dcoldweight.Add(devicessj.SSJID, 0);
                        }
                    }

                    #region 外形检测报警
                    bool flag = false;
                    int btid = Convert.ToInt32(devicessj.BTID);
                    byte[] bty = new byte[2];
                    BitArray arr = new BitArray(bty);
                    if (!HsWcsReadalarm(out arr))
                    {
                        return;
                    }
                    else
                    {
                        string str = string.Empty;
                        for (int j = 0; j < 3; j++)
                        {
                            int start = (btid - 1) * 3;
                            flag = arr[start + j];

                            if (flag)
                            {
                                if (string.IsNullOrEmpty(str)) 
                                {
                                    str += "外形检测不合格\r";
                                }
                                switch (j)
                                {
                                    case 0:
                                        str += "超高\r";
                                        break;
                                    case 1:
                                        str += "左超\r";
                                        break;
                                    case 2:
                                        str += "右超\r";
                                        break;
                                }
                            }
                        }
                        if (str.Length > 0) 
                        {
                            if (dcsaveledstr[devicessj.SSJID] != str) 
                            {
                                LedSendStr(devicessj.SSJID, str, 0);
                            }
                            continue;
                        }
                    }
                    #endregion

                    if (SSJInWareCommand(ref devicessj))
                    {
                        logWrite.WriteLog($"输送机{devicessj.SSJID}对应托盘条码{devicessj.TRAYCODE}重量为：{devicessj.Weight}");
                        NotifyEvent?.Invoke($"输送机{devicessj.SSJID}对应托盘条码{devicessj.TRAYCODE}重量为：{devicessj.Weight}");
                    }
                }
                #endregion

                #region 巷道入库口外
                for (int i = 1; i <= 7; i++) 
                {
                    TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "105");
                    if (!HsWcsReadSSJ(ref devicessj))
                    {
                        continue;
                    }
                }
                #endregion

                #region 巷道入库口内
                for (int i = 1; i <= 7; i++)
                {
                    TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "102");
                    if (!HsWcsReadSSJ(ref devicessj))
                    {
                        continue;
                    }
                    if (devicessj.TRAYCODE.Length > 0) 
                    {
                        //一楼其他输送机都是正在调度任务执行阶段
                        DataSet ds = DataTrans.D_GetTaskInfoByStatus("3", devicessj.TRAYCODE);
                        if (ds != null && ds.Tables[0].Rows.Count > 0) 
                        {
                            devicessj.Taskid = ds.Tables[0].Rows[0]["TASKID"].ToString();
                            //申请货位
                            if (ds.Tables[0].Rows[0]["TPLATOON"].ToString() == string.Empty) 
                            {
                                AllotSpaceId(devicessj.Taskid, devicessj);
                            }
                        }
                    }
                }
                #endregion

                #region 出库口输送机到位空闲更新
                for (int i = 1; i <= 4; i++)
                {
                    TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "104");
                    if (devicessj != null) 
                    {
                        HsWcsReadSSJ(ref devicessj);
                        if (!dcsaveledstr.ContainsKey(devicessj.SSJID)) 
                        {
                            dcsaveledstr.Add(devicessj.SSJID, "TMP");
                        }
                        if (devicessj.KXBZ == "0" && devicessj.TRAYCODE != string.Empty) 
                        {
                            DataSet ds = DataTrans.D_GetJobInfoToLed(devicessj.TRAYCODE);
                            if (ds != null && ds.Tables[0].Rows.Count > 0) 
                            {
                                string str = $"托盘条码{ds.Tables[0].Rows[0]["TRAYCODE"].ToString().Trim()}\r物料编码:{ds.Tables[0].Rows[0]["productcode"].ToString().Trim()}\r物料名称：{ds.Tables[0].Rows[0]["productname"].ToString().Trim()}\r物料批次：{ds.Tables[0].Rows[0]["lotinfo"].ToString().Trim()}\r出库重量/总重量：{ds.Tables[0].Rows[0]["ASSIGNNUM"].ToString().Trim()}/{ds.Tables[0].Rows[0]["JOBNUM"].ToString().Trim()}";
                                if (dcsaveledstr[devicessj.SSJID] != str) 
                                {
                                    LedSendStr(devicessj.SSJID, str, 0);
                                }
                            }
                        }
                        else
                        {
                            string str = string.Empty;
                            if (dcsaveledstr[devicessj.SSJID] != str) 
                            {
                                LedSendStr(devicessj.SSJID, str, 2);
                            }
                        }
                    }
                    else
                    {
                        string str = string.Empty;
                        if (dcsaveledstr[devicessj.SSJID] != str) 
                        {
                            LedSendStr(devicessj.SSJID, str, 2);
                        }
                    }
                }
                #endregion

                #region 巷道出库口输送机到位空闲更新
                for (int i = 1; i <= 7; i++) 
                {
                    TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "103");
                    HsWcsReadSSJ(ref devicessj);
                }
                #endregion
            }
            catch (Exception ex)
            {
                NotifyEvent?.Invoke($"输送机异常报警，异常信息为：{ex.Message}");
            }
        }


        #region 向WMS申请货位，如果中间表货位申请，不存在-申请。存在-检查申请是否处理
        public int AllotSpaceId(string taskid,TransportStr si)
        {
            int n = 0;
            #region 向中间表插入数据，向WMS发起请求
            //发起前先判断中间表是否已存在该条申请，状态1=下发  2=已处理状态
            DataSet dsisexist = DataTrans.D_GetIDX_ASRS_SEND(taskid, "15", "1','2");
            if (dsisexist != null && dsisexist.Tables[0].Rows.Count == 0) 
            {
                DataSet ds = DataTrans.D_GetAlleyIdBytaskid(si.Taskid);
                if (ds == null)
                {
                    return 0;
                }
                string alleyid = ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString();
                //15=货位申请  1=下方
                int k = DataTrans.D_InsertIDX_ASRS_SEND("15", "1", taskid, si.TRAYCODE, "", alleyid);
                if (k > 0) 
                {
                    logWrite.WriteLog($"输送机{si.SSJID}调度指令{taskid}入库已向WMS申请货位托盘条码为{si.TRAYCODE}");
                    NotifyEvent?.Invoke($"输送机{si.SSJID}调度指令{taskid}入库已向WMS申请货位托盘条码为{si.TRAYCODE}");
                }
                else
                {
                    logWrite.WriteLog($"输送机{si.SSJID}调度指令{taskid}入库向WMS未申请到货位");
                    NotifyEvent?.Invoke($"输送机{si.SSJID}调度指令{taskid}入库向WMS未申请到货位");
                }
            }
            #endregion

            //获取WMS重新分配的货位，2代表WMS已下发
            DataSet dsidx = DataTrans.D_GetIDX_ASRS_SEND(taskid, "15", "2");
            //若是查询到数据说明获取到货位
            if (dsidx != null && dsidx.Tables[0].Rows.Count > 0) 
            {
                logWrite.WriteLog($"输送机{si.SSJID}调度指令{taskid}申请的货位WMS已处理");
                NotifyEvent?.Invoke($"输送机{si.SSJID}调度指令{taskid}申请的货位WMS已处理");
                //临时库位字段
                string spaceid = dsidx.Tables[0].Rows[0]["LOCATION"].ToString();
                //根据货位id获取货位信息
                DataSet ds = DataTrans.P_getSpaceInfo(spaceid);
                if (ds != null && ds.Tables[0].Rows.Count > 0) 
                {
                    //货位名称
                    string spacename = ds.Tables[0].Rows[0]["CARGO_SPACE_NAME"].ToString();
                    //货位排
                    string tCSPLATOON= ds.Tables[0].Rows[0]["CSPLATOON"].ToString();
                    //货位列
                    string tCSCOLUMN = ds.Tables[0].Rows[0]["CSCOLUMN"].ToString();
                    //货位层
                    string tCSFLOOR = ds.Tables[0].Rows[0]["CSFLOOR"].ToString();

                    n = DataTrans.P_UpSchSpaceInfo(spaceid, spacename, tCSPLATOON, tCSCOLUMN, tCSFLOOR, si.Taskid);
                }
            }
            return n;
        }
        #endregion

        /// <summary>
        /// 读取外形检测报警信息
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private bool HsWcsReadalarm(out BitArray arr)
        {
            bool flag;
            int start = 0;
            byte[] bty = new byte[2];
            arr = new BitArray(bty);
            try
            {
                if (HsPLCList["A"].HsRead($"DB18.{start}", 2, ref bty)) 
                {
                    arr = new BitArray(bty);
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                logWrite.WriteLog($"读取DB48异常，状态为：{ex.Message}");
            }
            return flag;
        }

        public bool HsWriteToSSJ(TransportStr ts)
        {
            try
            {
                int start = int.Parse(ts.VAR3);

                byte[] buffer = new byte[18];
                int[] info = new int[18];

                #region 将任务号转为16进制，然后存入2字节

                #endregion

                byte[] taskno = new byte[2];
                taskno[0] = (byte)Convert.ToInt32(Convert.ToInt32(ts.ZXRWH) / 256);
                taskno[1] = (byte)Convert.ToInt32(Convert.ToInt32(ts.ZXRWH) % 256);
                byte[] barcode = new byte[4];
                ts.TRAYCODE = string.IsNullOrEmpty(ts.TRAYCODE) ? "0" : ts.TRAYCODE;
                barcode[0] = (byte)Convert.ToInt32(Convert.ToInt32(ts.TRAYCODE) / (256 * 256 * 256));
                barcode[1] = (byte)Convert.ToInt32(Convert.ToInt32(ts.TRAYCODE) / (256 * 256));
                barcode[2] = (byte)Convert.ToInt32(Convert.ToInt32(ts.TRAYCODE) / 256);
                barcode[3] = (byte)Convert.ToInt32(Convert.ToInt32(ts.TRAYCODE) - barcode[0] * 256 * 256 * 256 - barcode[1] * 256 * 256 - barcode[2] * 256);

                for (int i = 0; i < 18; i++)
                {
                    info[i] = 0;
                }
                for (int i = 0; i < taskno.Length; i++)
                {
                    info[i] = int.Parse(taskno[i].ToString());
                }
                for (int i = 0; i < barcode.Length; i++)
                {
                    info[i + 2] = int.Parse(barcode[i].ToString());
                }
                for (int i = 0; i < info.Length; i++)
                {
                    buffer[i] = (byte)info[i];
                }

                bool flag = false;
                if (HsPLCList.ContainsKey(ts.VAR1))
                {
                    flag = HsPLCList[ts.VAR1].HsWrite($"DB{dbnumber}.{start}", buffer);
                }
                else
                {
                    HsPLCList.Add(ts.VAR1, new HsControlServer(ConfigurationManager.AppSettings[ts.VAR1]));
                }
                return flag;
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"输送机{ts.SSJID}条码{ts.TRAYCODE}异常，异常信息为{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取输送机信息
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public bool HsWcsReadSSJ(ref TransportStr si)
        {
            bool flag = false;
            try
            {
                //当前输送机对应电机对应开始字节，读取电机信息（主要到位空闲信息）DB54
                int djstart = Convert.ToInt32(si.VAR4);
                byte[] djbty = new byte[12];
                if (HsPLCList[si.VAR1].HsRead($"DB{djdbnumber}.{djstart}", 12, ref djbty)) 
                {
                    #region 读取DB54信息
                    //将电机对应输送机id的第一个字节存储到buffer中，并获取bit
                    int djint = djbty[8];
                    byte[] buffer = BitConverter.GetBytes(djint);
                    BitArray arr = new BitArray(buffer);
                    si.DWXH = Convert.ToInt32(arr[0]).ToString();   //到位信号
                    si.KXBZ = Convert.ToInt32(arr[1]).ToString();   //空闲信息
                    #endregion

                    #region 读取DB55信息
                    //读取当前输送机条码信息等DB55
                    si.TRAYCODE = string.Empty;
                    int start = Convert.ToInt32(si.VAR3);
                    byte[] bty = new byte[20];
                    if (HsPLCList[si.VAR1].HsRead($"DB{dbnumber}.{start}", 18, ref bty))  
                    {
                        //将读取到的信息更新到实体类
                        string rwh = BitConverter.ToString(bty, 0, 2).Replace("-", string.Empty).ToLower();
                        //任务号
                        si.ZXRWH = int.Parse(rwh, System.Globalization.NumberStyles.HexNumber).ToString();
                        //条码
                        si.TRAYCODE = (bty[2] * 256 * 256 * 256 + bty[3] * 256 * 256 + bty[4] * 256 + bty[5]).ToString();
                        if (Convert.ToInt32(si.TRAYCODE) == 0)
                        {
                            si.TRAYCODE = "";
                        }
                        else if (si.TRAYCODE.Length != 8) 
                        {
                            logWrite.WriteLog($"输送机{si.SSJID}条码{si.TRAYCODE}长度非8位");
                            NotifyEvent?.Invoke($"输送机{si.SSJID}条码{si.TRAYCODE}长度非8位");
                            return false;
                        }
                        byte[] tmp = new byte[1];
                        tmp[0] = bty[12];
                        //获取12字节的位数
                        si.Arr = new BitArray(tmp);

                        //计算重量
                        byte[] btyweight = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            btyweight[i] = bty[i + 14];
                        }
                        btyweight = btyweight.Reverse().ToArray();
                        si.Weight = Math.Round(BitConverter.ToSingle(btyweight, 0), 2);
                        //更新数据库中的输送机
                        DataTrans.D_UpDevicesSSJ(si);
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    #endregion
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                logWrite.WriteLog($"输送线{si.SSJID}读取DB块异常，状态为：{ex.Message}");
            }
            return flag;
        }


        private void LedSendStr(string ssjid,string strmsg,int nAlignment)
        {
            DataSet ds = new DataSet();
            ds = DataTrans.D_GetLEDInfo(ssjid);
            if (ds != null) 
            {
                if (ds.Tables[0].Rows.Count > 0) 
                {
                    //定义通讯参数结构体变量用于设定的LED通讯
                    Leddll.COMMUNICATIONINFO communicationInfo = new Leddll.COMMUNICATIONINFO
                    {
                        SendType = 0,   //设定为固定IP通讯模式，即TCP通讯
                        IpStr = ds.Tables[0].Rows[0]["IPADRESS"].ToString().Trim(),     //给IpStr复制LED控制卡的IP
                        LedNumber = 1,  //LED屏号为1，请注意socket通讯和232通讯不识别屏号，默认复赋值1，485必需根据屏的实际屏号进行赋值
                    };

                    //节日句柄
                    //根据传的参数创建节日句柄
                    //64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色必须与设置屏参屏宽高及颜色一致，否则报错
                    int hProgram = Leddll.CreateProgram(256, 96, 1);

                    //添加一个节目
                    int nResult = Leddll.AddProgram(hProgram, 1, 0, 1);

                    //错误
                    if (nResult != 0) 
                    {
                        string ErrStr = Leddll.LS_GetError(nResult);
                        NotifyEvent?.Invoke($"{ssjid}对应的LED发送出现错误：{ErrStr}");
                        logWrite.WriteLog($"{ssjid}对应的LED发送出现错误：{ErrStr}");
                        return;
                    }

                    //区域坐标属性结构体变量
                    Leddll.AREARECT areaRect = new Leddll.AREARECT
                    {
                        left = 0,
                        top = 0,
                        width = 256,
                        height = 96
                    };

                    Leddll.AddImageTextArea(hProgram, 1, 1, ref areaRect, 0);

                    Leddll.FONTPROP fontProp = new Leddll.FONTPROP
                    {
                        FontName = "宋体",
                        FontSize = Convert.ToInt32(ds.Tables[0].Rows[0]["FONTSIZE"].ToString()),
                        FontColor = Leddll.COLOR_RED,
                        FontBold = 0
                    };

                    Leddll.PLAYPROP playProp = new Leddll.PLAYPROP
                    {
                        InStyle = 0,
                        DelayTime = 3,
                        Speed = 4
                    };

                    string str = string.IsNullOrEmpty(strmsg) ? ds.Tables[0].Rows[0]["DEFAULTTEXT"].ToString().Trim().Replace("\\r", "\r") : strmsg;
                    //通过字符串添加一个多行文本到图文区
                    Leddll.AddStaticTextToImageTextArea(hProgram, 1, 1, Leddll.ADDTYPE_STRING, str, ref fontProp, 1, nAlignment, 1);
                    //发送
                    nResult = Leddll.Send(ref communicationInfo, hProgram);
                    //删除节目内存对象
                    Leddll.DeleteProgram(hProgram);

                    //失败
                    if (nResult != 0) 
                    {
                        string errStr = Leddll.LS_GetError(nResult);
                        NotifyEvent?.Invoke($"{ssjid}对应的LED发送出现错误：{errStr}");
                        logWrite.WriteLog($"{ssjid}对应的LED发送出现错误：{errStr}");
                    }
                    else
                    {
                        NotifyEvent?.Invoke($"给输送机{ssjid}对应的LED发送成功");
                        logWrite.WriteLog($"给输送机{ssjid}对应的LED发送成功");
                        SaveSSJLedStr(ssjid, strmsg);
                    }
                }
            }
        }


        /// <summary>
        /// 保存输送机对应已发送给LED的字符串
        /// </summary>
        /// <param name="ssjid"></param>
        /// <param name="strmsg"></param>
        public void SaveSSJLedStr(string ssjid,string strmsg)
        {
            if (dcsaveledstr.ContainsKey(ssjid)) 
            {
                dcsaveledstr[ssjid] = strmsg;
            }
            else
            {
                dcsaveledstr.Add(ssjid, strmsg);
            }
        }
    }
}
