using BaseData;
using LOG;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataOperate
{
    public static class DataTrans
    {
        public static Log LogInfo = new Log("数据操作", @".\数据操作\");

        //锁
        private static object qLock = new object();

        #region 根据输送机编号查询对应LED显示屏的基础设置
        public static DataSet D_GetLEDInfo(string ssjid)
        {
            string orastr = $"select * from base_ledinfo where ssjid='{ssjid}'";
            return OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
        }
        #endregion

        #region 获取输送机相关信息
        public static DataSet D_GetDeviceSSJ()
        {
            string orastr = $"select * from rico_devices_ssj";
            return OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
        }
        #endregion

        #region 将从输送机读出的数据写入数据库输送机设备表中
        public static void D_UpDevicesSSJ(TransportStr si)
        {
            //更新输送机设备表中任务号，托盘条码，到位信号，空闲信号
            string orastr = $"update rico_devices_ssj set ZXRWH='{si.ZXRWH}',TRAYCODE='{si.TRAYCODE}',DWXH='{si.DWXH}',KXBZ='{si.KXBZ}' where ssjid='{si.SSJID}'";
            OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
        }
        #endregion

        #region 分配任务号
        public static int D_AllotTaskno()
        {
            lock (qLock)
            {
                try
                {
                    #region 查出最大任务号
                    int newTaskNo = 0;
                    string sqlTaskNo = $"select taskno from tbtaskno";
                    DataTable dt = OracleHelper.ExecuteDataSet(CommandType.Text, sqlTaskNo, null).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        newTaskNo = 1000;
                    }
                    else
                    {
                        int maxtaskno = int.Parse(dt.Compute($"max(taskno)", "").ToString().Trim());
                        if (maxtaskno + 1 > 30000)
                        {
                            #region 如果任务号达到最大值，删除任务表
                            string oraDelTaskNo = $"delete from TBTASKNO ";
                            OracleHelper.ExecuteNonQuery(CommandType.Text, oraDelTaskNo, null);
                            #endregion
                            newTaskNo = 1000;
                        }
                        else
                        {
                            newTaskNo = maxtaskno + 1;
                        }
                    }
                    #endregion
                    //获取到任务将任务提前插入表中
                    if (newTaskNo != 0)
                    {
                        string orastr = $"insert into tbtaskno (taskno) values ('{newTaskNo}')";
                        OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
                    }

                    return newTaskNo;
                }
                catch (Exception ex)
                {
                    LogInfo.WriteLog($"新任务申请异常，异常信息为{ex.Message}");
                    return 0;
                }
            }
        }
        #endregion

        #region 入库口根据输送机获取待执行任务
        public static DataSet D_GetInwareTask1(string ssjid)
        {
            DataSet ds = new DataSet();
            string orastr = $"select a.*,b.reserve1 actweight,c.productcode,c.productid,c.productname,c.lotinfo,c.assignnum from schedule_task a,inout_job b,inout_jobdetail c where a.jobid=b.jobid and a.jobid=c.jobid and tasktype='1' and a.status='2' and a.RESERVE3 is  null and a.snumber='{ssjid}' order by a.taskid desc";
            try
            {
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog($"根据输送机{ssjid}获取调度任务异常，异常信息为：{ex.Message}");
            }
            return ds;
        }
        public static DataSet D_GetInwareTask(string ssjid)
        {
            DataSet ds = new DataSet();
            string orastr = $"select a.*,b.reserve1 actweight,c.productcode,c.productid,c.productname,c.lotinfo,c.assignnum from schedule_task a,inout_job b,inout_jobdetail c where a.jobid=b.jobid and a.jobid=c.jobid and tasktype='1' and a.status='2' and a.snumber='{ssjid}' order by a.taskid desc";
            try
            {
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog($"根据输送机{ssjid}获取调度任务异常，异常信息为：{ex.Message}");
            }
            return ds;
        }
        #endregion

        #region 查询输送机指令表
        public static DataSet D_SSJCommandQuery(string ssjid)
        {
            try
            {
                string orastr = $"select * from rico_device_ssj_command where ssjid='{ssjid}'";
                DataSet ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 将指令写入输送机指令表
        /// <summary>
        /// 将指令写入指令表
        /// </summary>
        /// <param name="si"></param>
        /// <param name="taskid"></param>
        public static bool D_SSJCommandAllotNotask(TransportStr si, string taskid)
        {
            try
            {
                string tptm1 = "";
                string tptm2 = "";
                if (si.TRAYCODE.Length > 4)
                {
                    tptm1 = si.TRAYCODE.Substring(0, 4);
                    tptm2 = si.TRAYCODE.Substring(4, si.TRAYCODE.Length - 4);
                }
                string alleyid = si.ALLEYID;
                if (alleyid.Length > 3)
                {
                    alleyid = Convert.ToInt32(alleyid.Substring(si.ALLEYID.Length - 3, 3)).ToString();
                }
                string orastr = "begin ";
                //orastr += " insert into tbtaskno(taskno) values ('" + si.ZXRWH + "');";
                orastr += " update SCHEDULE_TASK set TASKNO='" + si.ZXRWH + "' where TASKID='" + taskid + "';";
                orastr += "  update rico_device_ssj set ZXRWH='" + si.ZXRWH + "' where SSJID='" + si.SSJID + "';";
                orastr += @" INSERT INTO RICO_DEVICE_SSJ_COMMAND( SSJID,BTID,DTYPE,RWH,TUNNELID,
                                  TPTM1,TPTM2,DOFLAG,CREATETIME,TPTM) 
                                  VALUES ('" + si.SSJID + "','" + si.BTID + "','" + si.DTYPE + "','" + si.ZXRWH + "','" + alleyid
                                            + "','" + tptm1 + "','" + tptm2
                                            + "','N','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "','" + si.TRAYCODE + "');";
                orastr += " end;";
                int n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
                return true;
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("输送机" + si.SSJID + "下发指令" + si.ZXRWH + "异常,异常信息为" + ex.Message);
                return false;
            }
        }
        #endregion

        #region 指令下发OK后处理数据事物
        /// <summary>
        /// 指令下发OK后处理数据事物
        /// </summary>
        /// <param name="ssjid"></param>
        /// <param name="traycode"></param>
        /// <param name="phase"></param>
        /// <param name="status"></param>
        /// <param name="jobstatus">作业状态</param>
        public static void D_SSJCommandOk(string ssjid, string traycode, string taskid, string phase, string status, string jobstatus)
        {
            string orastr = "begin ";
            orastr += @" INSERT INTO RICO_DEVICE_SSJ_COMMAND_HIS
                            ( SSJID,BTID,DTYPE,RWH,TUNNELID,TPTM1,TPTM2,TPTM,JYM,CQBJ,DOFLAG,CREATETIME,SENDTIME) 
                            select SSJID,BTID,DTYPE,RWH,TUNNELID,TPTM1,TPTM2,TPTM,JYM,CQBJ,DOFLAG,CREATETIME, to_char(sysdate,'yyyy/mm/dd hh24:mi:ss')
                            from rico_device_ssj_command where ssjid='" + ssjid + "';";
            orastr += "delete from rico_device_ssj_command where ssjid='" + ssjid + "';";
            orastr += "update schedule_task set phase='" + phase + "',status='" + status + "' where taskid='" + taskid + "';";
            orastr += "update IDX_ASRS_SEND set TASKSTATUS='3' where PALLETID='" + traycode + "' and TASKSTATUS='2';";
            if (jobstatus != "4" && jobstatus != "5")
            {
                orastr += " UPDATE INOUT_JOB SET STATUS='" + jobstatus + "' WHERE TASKID='" + taskid + "' and status !='4' and status!=5;";
            }

            orastr += "end;";
            try
            {

                OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
                //LogInfo.WriteLog("执行语句"+orastr);

            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("删除已下发指令，并移动到历史指令中出错，错误代码:" + ex.Message + "语句为" + orastr);
            }
        }
        #endregion

        #region 通过执行状态和托盘条码获取任务信息
        /// <summary>
        /// 通过执行状态和托盘条码获取任务信息
        /// </summary>
        /// <param name="status">执行状态</param>
        /// <param name="traycode">托盘条码</param>
        /// <returns></returns>
        public static DataSet D_GetTaskInfoByStatus(string status, string traycode)
        {
            DataSet ds = new DataSet();
            try
            {
                string orastr = "select * from SCHEDULE_TASK where STATUS in ('" + status + "')  and TRAYCODE='" + traycode + "'  and TASKTYPE='1'  order by TASKID desc";
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("根据状态" + status + "和托盘条码" + traycode + "获取调度任务异常，异常信息为" + ex.Message);
            }
            return ds;
        }
        #endregion
        #region 更新入库货物称重重量
        /// <summary>
        /// 更新入库货物称重重量
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="actweight"></param>
        public static void P_UpActWeight(string taskid, string actweight)
        {
            string orastr = @"update inout_job set reserve1 = :actweight where taskid = :taskid";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":actweight", actweight), new OracleParameter(":taskid", taskid)
            };
            try
            {
                OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parameters);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("更新调度指令为" + taskid + "的作业称重重量为" + actweight + "出现异常，异常信息为:" + ex.Message + " oracle语句为:" + orastr);
            }
        }
        #endregion

        #region 根据指令编号获取巷道号
        /// <summary>
        /// 根据指令编号获取巷道号
        /// </summary>
        /// <param name="taskno"></param>
        /// <returns></returns>
        public static DataSet D_GetAlleyIdBytaskid(string taskid)
        {
            string orastr = "SELECT * FROM SCHEDULE_TASK  where TASKID='" + taskid + "'";
            DataSet ds = new DataSet();
            try
            {
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("根据调度指令" + taskid + "获取调度任务信息异常，异常信息为:" + ex.Message);
            }
            return ds;
        }
        #endregion

        #region 更新调度指令货位信息
        /// <summary>
        /// 更新调度指令货位信息
        /// </summary>
        /// <param name="spacename">货位名</param>
        /// <param name="spaceplatoon">货位排</param>
        /// <param name="column">货位列</param>
        /// <param name="floor">货位层</param>
        /// <param name="floor">调度指令id</param>
        /// <returns></returns>
        public static int P_UpSchSpaceInfo(string spaceid, string spacename, string spaceplatoon, string column, string floor, string taskid)
        {
            int n = 0;
            try
            {
                string orastr = "begin ";
                orastr += " update schedule_task set TCARGO_SPACE_ID='" + spaceid + "',TPLATOON='" + spaceplatoon
                               + "',TCOLUMN='" + column + "',TFLOOR='" + floor + "',TCARGO_SPACE_NAME='" + spacename + "' where taskid='" + taskid + "';";
                orastr += " update IDX_ASRS_SEND set TASKSTATUS='3' where taskid='" + taskid + "' and TASKSTATUS='2';";
                orastr += " UPDATE INOUT_JOB SET TCARGO_SPACE_ID='" + spaceid + "' WHERE TASKID='" + taskid + "';";
                orastr += " end;";
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("更新调度指令" + taskid + "货位为" + spacename + "异常，异常信息为：" + ex.Message);
            }
            return n;
        }
        #endregion

        #region 堆垛机根据任务号查询调度信息
        /// <summary>
        /// 根据任务号获取调度信息
        /// </summary>
        /// <param name="taskno">任务号</param>
        /// <returns></returns>
        public static DataSet D_GetSchByTaskno(string taskno)
        {
            DataSet ds = new DataSet();
            try
            {
                string orastr = "select * from schedule_task where taskno in (" + taskno + ") and status in ('2','3')";
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("根据任务号" + taskno + "获取调度任务信息异常，异常信息为：" + ex.Message);
            }
            return ds;
        }
        #endregion

        #region 堆垛机任务完成后处理事务
        /// <summary>
        /// 堆垛机完成后事物处理
        /// </summary>
        /// <param name="btid">堆垛机ID</param>
        /// <param name="taskid">调度指令编码</param>
        /// <param name="status">状态</param>
        /// <param name="phase">阶段</param>
        /// <param name="jobstatus">作业状态</param>
        public static void D_CraneCommandTrans(string btid, string taskid, string status, string phase, string jobstatus)
        {
            lock (qLock)
            {
                try
                {
                    StringBuilder orastr = new StringBuilder();
                    orastr.Append("begin ");
                    //更新堆垛机指令为已下发
                    string upcmd = "update rico_device_ddj_command set doflag = 'Y' where btid='" + btid + "';";
                    orastr.Append(upcmd + " ");
                    //将堆垛机指令移入历史表中
                    string delcmd = @" INSERT INTO RICO_DEVICE_DDJ_COMMAND_HIS( BTID,DTYPE,COMMKIND,RWH,SPH,
                             SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME,SENDTIME,TRAYCODE,TASKID) 
                            select BTID,DTYPE,COMMKIND,RWH,SPH,SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME, 
                            to_char(sysdate,'yyyy/mm/dd hh24:mi:ss'),TRAYCODE,TASKID
                            from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';" + "";

                    delcmd += " update RICO_DEVICE_DDJ_COMMAND_HIS set sendtime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where taskid='" + taskid + "';";
                    delcmd += "delete from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';";
                    orastr.Append(delcmd + " ");
                    //更新调度指令状态与阶段
                    string upphase = "update schedule_task set phase='" + phase + "',status='" + status + "' where TASKID='" + taskid + "';";
                    orastr.Append(upphase + " ");
                    string upjobstatus = " UPDATE INOUT_JOB SET STATUS='" + jobstatus + "' WHERE TASKID='" + taskid + "' and status !='4' and status!=5;";
                    //2019年2月21日修改
                    if (jobstatus != "4" && jobstatus != "5")
                    {
                        orastr.Append(upjobstatus);
                    }
                    orastr.Append(" end;");
                    OracleHelper.ExecuteNonQuery(CommandType.Text, orastr.ToString(), null);
                    //LogInfo.WriteLog("堆垛机任务结束，更加任务状态，语句为："+orastr);
                }
                catch (Exception ex)
                {
                    LogInfo.WriteLog("堆垛机" + btid + "完成后处理调度指令" + taskid + "异常，异常信息为" + ex.Message);
                }
            }
        }
        #endregion

        #region 获取WMS重新分配的货位
        /// <summary>
        /// 获取WMS重新分配的货位
        /// </summary>
        /// <param name="taskid">调度id</param>
        /// <param name="messagecode">异常规格</param>
        /// <param name="taskstatus">任务状态</param>
        /// <returns></returns>
        public static DataSet D_GetIDX_ASRS_SEND(string taskid, string messagecode, string taskstatus)
        {
            DataSet ds = new DataSet();
            try
            {
                string orastr = "select * from IDX_ASRS_SEND where taskid='" + taskid + "' and messagecode='" + messagecode + "' and taskstatus in ('" + taskstatus + "') order by sendid desc";
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("调度指令编码" + taskid + "获取重新分配货位信息异常，异常信息为:" + ex.Message);
            }
            return ds;
        }
        #endregion

        #region 通过中间表，向WMS提交申请货位任务
        /// <summary>
        /// 通过中间表，向WMS提交申请任务
        /// </summary>
        /// <param name="messagecode"></param>
        /// <param name="status"></param>
        /// <param name="taskid"></param>
        /// <param name="palletid"></param>
        /// <param name="OLDLOCATION">源货位</param>
        public static int D_InsertIDX_ASRS_SEND(string messagecode, string status, string taskid, string palletid, string OLDLOCATION, string alleyid)
        {
            int n = 0;
            try
            {
                string orastr = @"INSERT INTO IDX_ASRS_SEND(SENDID, MESSAGECODE,CREATETIME,TASKSTATUS,TASKID,PALLETID,OLDLOCATION,ASRSID) VALUES 
                               (SEQ_IDX.nextval,:MESSAGECODE,to_date(:CREATETIME,'yyyy/mm/dd hh24:mi:ss'),:TASKSTATUS,:TASKID,:PALLETID,:OLDLOCATION,:ASRSID)";
                OracleParameter[] parm = new OracleParameter[] {
                                            new OracleParameter(@":MESSAGECODE",OracleType.NVarChar,50),
                                            new OracleParameter(@":CREATETIME",OracleType.NVarChar,50),
                                            new OracleParameter(@":TASKSTATUS",OracleType.NVarChar,50),
                                            new OracleParameter(@":TASKID",OracleType.NVarChar,50),
                                            new OracleParameter(@":PALLETID",OracleType.NVarChar,50),
                                            new OracleParameter(@":OLDLOCATION",OracleType.NVarChar,50),
                                            new OracleParameter(@":ASRSID",OracleType.NVarChar,50)
                };

                parm[0].Value = messagecode;
                parm[1].Value = DateTime.Now.ToString("yyyy-MM-dd");
                parm[2].Value = status;
                parm[3].Value = taskid;
                parm[4].Value = palletid;
                parm[5].Value = OLDLOCATION;
                parm[6].Value = alleyid;
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parm);

            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("调度指令" + taskid + "申请任务" + messagecode + "向WMS申请异常，异常信息为:" + ex.Message);
            }
            return n;
        }
        #endregion

        #region 根据货位号获取货位相关信息
        /// <summary>
        /// 根据货位号获取货位相关信息
        /// </summary>
        /// <returns></returns>
        public static DataSet P_getSpaceInfo(string spaceid)
        {
            DataSet ds = new DataSet();
            try
            {
                string orastr = "select * from BASE_CARGOSPACE where CARGO_SPACE_ID='" + spaceid + "'";
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("根据货位" + spaceid + "获取货位明细异常，异常类型为:" + ex.Message);
            }
            return ds;
        }
        #endregion

        #region 获取到重分货位，然后更新相关表信息
        /// <summary>
        /// 获取到重分货位，然后更新相关表信息
        /// </summary>
        /// <param name="taskid">调度任务id</param>
        /// <param name="status">中间表状态</param>
        /// <param name="sendid">中间表id</param>
        /// <param name="spaceid">货位号</param>
        /// <param name="desplantoon">目的排号</param>
        /// <param name="descolumn">目标列号</param>
        /// <param name="floor">目标层号</param>
        public static void D_GetSpaceUpInfo(string taskid, string btid, string status, string sendid,
            string spaceid, string desplantoon, string descolumn, string desfloor, string spacename)
        {
            string orastr = "begin";
            orastr += @" update schedule_task set TCARGO_SPACE_ID='" + spaceid + "',TCARGO_SPACE_NAME='" + spacename + "',TPLATOON='" + desplantoon + "',TCOLUMN='" + descolumn
                        + "',TFLOOR='" + desfloor + "' where taskid='" + taskid + "';";//更新调度指令表
            orastr += @" update idx_asrs_send set TASKSTATUS='" + status + "' where SENDID='" + sendid + "';";
            orastr += @" update rico_device_ddj set MBPH='" + desplantoon + "',MBlh='" + descolumn + "',MBch='" + desfloor + "' where btid='" + btid + "';";
            orastr += @" update rico_device_ddj_command set EPH='" + desplantoon + "',ELH='" + descolumn + "',ECH='" + desfloor + "' where btid='" + btid + "';";
            orastr += @" update INOUT_JOB set TCARGO_SPACE_ID='" + spaceid + "' where TASKID='" + taskid + "';";
            orastr += " end;";
            try
            {
                OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("堆垛机" + btid + "入库货位有货更新相关表异常，异常信息为" + ex.Message + "ORA语句：" + orastr);
            }
        }
        #endregion

        /// <summary>
        /// 入库有货删除堆垛机指令，作废调度指令
        /// </summary>
        /// <param name="taskid"></param>
        public static int D_RKYHDel(string btid, string taskid)
        {
            try
            {
                StringBuilder orastr = new StringBuilder();
                string oracommand = @" INSERT INTO RICO_DEVICE_DDJ_COMMAND_HIS( BTID,DTYPE,COMMKIND,RWH,SPH,
                             SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME,SENDTIME,TRAYCODE,TASKID) 
                            select BTID,DTYPE,COMMKIND,RWH,SPH,SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME, to_char(sysdate,'yyyy/mm/dd hh24:mi:ss'),TRAYCODE,TASKID
                            from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';";
                oracommand += "delete from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';";
                string orauptask = " update schedule_task set status ='5' where taskid='" + taskid + "';";
                string oraupidx = " update idx_asrs_send set TASKSTATUS = '3' where taskid='" + taskid + "';";
                orastr.Append("begin" + oracommand + " " + orauptask + oraupidx + " end;");
                int n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr.ToString(), null);
                return n;
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("出库无货删除堆垛机" + btid + "指令，作废调度指令" + taskid + "异常，异常信息为：" + ex.Message);
                return 0;
            }
        }
        #region 通过任务类型以及任务类型层号获取出库任务信息
        /// <summary>
        /// 通过任务类型以及任务类型层号获取出库任务信息
        /// </summary>
        /// <param name="tasktype">任务类型</param>
        /// <param name="floor">层号</param>
        /// <param name="status">任务状态</param>
        /// <param name="allayid">巷道号</param>
        /// <param name="taskno">任务号</param>
        /// <returns></returns>
        public static DataSet D_GetOutSchTaskByCrane(string tasktype, string floor, string status, string allayid, string taskno, CraneStr cs)
        {
            DataSet ds = new DataSet();
            string orastr = "select * from schedule_task where 1=1";
            if (tasktype != "")
            {
                orastr += " and tasktype='" + tasktype + "'";
            }
            if (!string.IsNullOrEmpty(floor))
            {
                orastr += " and FLOOR='" + floor + "'";
            }
            else
            {
                orastr += " and FLOOR IS NULL";
            }
            if (!string.IsNullOrEmpty(status))
            {
                orastr += " and status in ( '" + status + "')";
            }
            if (!string.IsNullOrEmpty(allayid))
            {
                orastr += " and SCARGO_ALLEY_ID='" + allayid + "'";
            }
            if (!string.IsNullOrEmpty(taskno))
            {
                orastr += " and taskno='" + taskno + "'";
            }
            orastr += @" order by (case
                        when(select * from(select ceil((sysdate - createtime) * 24 * 60)
                                from schedule_task
                               where 1 = 1
                                 and tasktype = '" + tasktype
                                 + "' and FLOOR = '" + floor
                                 + "' and status in ('" + status
                                 + "') and SCARGO_ALLEY_ID = '" + allayid + "'";
            orastr += " order by case when priority>=3 then to_char(createtime,'yyyy-mm-dd hh24:mi:ss') " +
                "else to_char(PRIORITY) end) where rownum=1) > 30  then  to_char(createtime, 'yyyy-mm-dd hh24:mi:ss') else  to_char(PRIORITY) || ',' ||";
            if (!string.IsNullOrEmpty(cs.Mblh))
            {
                int lh = Convert.ToInt32(cs.Mblh);
                orastr += " abs(SCOLUMN-" + lh + ")||','||";
            }
            if (!string.IsNullOrEmpty(cs.Mbch))
            {
                int ch = Convert.ToInt32(cs.Mbch);
                orastr += " abs(SCOLUMN-" + ch + ")||','||";
            }
            orastr += @"  createtime
                      end) ";
            try
            {

                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("根据任务类型获取任务异常，异常信息为:" + ex.Message + "语句为：" + orastr);
            }
            return ds;
        }
        #endregion

        #region 插入任务号，更新调度指令任务号
        /// <summary>
        /// 插入任务号，更新调度指令任务号
        /// </summary>
        /// <param name="taskno"></param>
        /// <param name="taskid"></param>
        public static bool D_UpSchTask(string taskno, string taskid)
        {
            string orastr = "begin ";
            //orastr += " insert into tbtaskno(taskno) values ('" + taskno + "');";
            orastr += " update SCHEDULE_TASK set TASKNO='" + taskno + "' where TASKID='" + taskid + "';";
            orastr += " end;";
            try
            {

                int n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
                return true;
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("调度指令" + taskid + "任务号更新异常,异常信息" + ex.Message + orastr);
                return false;
            }

        }
        #endregion

        #region  获取堆垛机指令
        /// <summary>
        /// 获取堆垛机指令
        /// </summary>
        /// <param name="craneid">堆垛机id</param>
        /// <returns></returns>
        public static DataSet D_GetCraneCommand(string craneid)
        {
            DataSet ds = new DataSet();
            try
            {
                string orastr = "select * from rico_device_ddj_command where btid='" + craneid + "'";

                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);

            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("获取堆垛机" + craneid + "出现异常，异常信息为:" + ex.Message);
            }
            return ds;
        }
        #endregion

        #region 堆垛机指令表任务残留删除
        /// <summary>
        /// 堆垛机指令表任务残留删除
        /// </summary>
        /// <param name="btid">堆垛机ID</param>

        public static void D_CraneCommanddel(string btid)
        {
            try
            {
                StringBuilder orastr = new StringBuilder();
                orastr.Append("begin ");


                //将堆垛机指令移入历史表中
                string delcmd = @" INSERT INTO RICO_DEVICE_DDJ_COMMAND_HIS( BTID,DTYPE,COMMKIND,RWH,SPH,
                             SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME,SENDTIME,TRAYCODE,TASKID) 
                            select BTID,DTYPE,COMMKIND,RWH,SPH,SLH,SCH,EPH,ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME, 
                            to_char(sysdate,'yyyy/mm/dd hh24:mi:ss'),TRAYCODE,TASKID
                            from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';" + "";
                delcmd += "delete from RICO_DEVICE_DDJ_COMMAND where BTID='" + btid + "';";
                orastr.Append(delcmd + " ");

                orastr.Append(" end;");
                OracleHelper.ExecuteNonQuery(CommandType.Text, orastr.ToString(), null);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("堆垛机" + btid + "残留调度指令删除异常，异常信息为" + ex.Message);
            }
        }
        #endregion

        #region 给堆垛机下发命令
        /// <summary>
        /// 给堆垛机下发命令
        /// </summary>
        /// <param name="si"></param>
        public static void D_InsertCraneCommand(CraneStr crs, DataTable dt)
        {
            try
            {
                string orastr = @"INSERT INTO RICO_DEVICE_DDJ_COMMAND( BTID,DTYPE,COMMKIND,RWH,SPH,SLH,SCH,EPH,
                                  ELH,ECH,TSJH,JYM,DOFLAG,CREATETIME,TRAYCODE,TASKID) VALUES 
                                  (:BTID,:DTYPE,:COMMKIND,:RWH,:SPH,:SLH,:SCH,:EPH,:ELH,:ECH,:TSJH,:JYM,:DOFLAG,:CREATETIME,:TRAYCODE,:TASKID)";
                #region
                OracleParameter[] parms = new OracleParameter[]
                {
                    new OracleParameter(@":BTID",OracleType.NVarChar,50),
                    new OracleParameter(@":DTYPE",OracleType.NVarChar,50),
                    new OracleParameter(@":COMMKIND",OracleType.NVarChar,50),
                    new OracleParameter(@":RWH",OracleType.NVarChar,50),
                    new OracleParameter(@":SPH",OracleType.NVarChar,50),
                    new OracleParameter(@":SLH",OracleType.NVarChar,50),
                    new OracleParameter(@":SCH",OracleType.NVarChar,50),
                    new OracleParameter(@":EPH",OracleType.NVarChar,50),
                    new OracleParameter(@":ELH",OracleType.NVarChar,50),
                    new OracleParameter(@":ECH",OracleType.NVarChar,50),
                    new OracleParameter(@":TSJH",OracleType.NVarChar,50),
                    new OracleParameter(@":JYM",OracleType.NVarChar,50),
                    new OracleParameter(@":DOFLAG",OracleType.NVarChar,50),
                    new OracleParameter(@":CREATETIME",OracleType.NVarChar,50),
                    new OracleParameter(@":TRAYCODE",OracleType.NVarChar,50),
                    new OracleParameter(@":TASKID",OracleType.NVarChar,50)
                };
                #endregion
                parms[0].Value = crs.Btid;//设备id
                parms[1].Value = "1";//设备分类
                parms[2].Value = "";//指令种类
                parms[3].Value = dt.Rows[0]["taskno"];//任务号
                parms[4].Value = dt.Rows[0]["SPLATOON"];//源货位排
                parms[5].Value = dt.Rows[0]["SCOLUMN"];//源货位列
                parms[6].Value = dt.Rows[0]["SFLOOR"];//源货位层
                parms[7].Value = 0;//目标货位排
                parms[8].Value = 0;//目标货位列
                parms[9].Value = 0;//目标货位层
                parms[10].Value = "";//
                parms[11].Value = "";//校验码
                parms[12].Value = "0";//是否下发给PLC
                parms[13].Value = DateTime.Now.ToString();//生成时间
                parms[14].Value = dt.Rows[0]["TRAYCODE"];//托盘条码
                parms[15].Value = dt.Rows[0]["taskid"].ToString();//任务号
                int n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parms);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("插入堆垛机指令异常，异常信息为:" + ex.Message);

            }
        }
        #endregion

        #region 入库给小车下发指令之后更新相关表信息
        public static int D_UpdateRkSendToAgv1(DataTable dt, string zxrwh, string source, string des, out string msg)
        {
            string orastr = "";
            if (dt.Rows[0]["TASKTYPE"].ToString() == "1")
            {
                orastr = "update schedule_task set status='2',phase='3',RESERVE3='1',taskno=:zxrwh,SCARGO_ALLEY_ID=:SCARGO_ALLEY_ID,TCARGO_ALLEY_ID=:TCARGO_ALLEY_ID where taskid=:taskid";
            }

            OracleParameter[] parameter = new OracleParameter[]
            {
                new OracleParameter(":zxrwh",zxrwh),
                new OracleParameter(":TCARGO_ALLEY_ID",des),
                new OracleParameter(":SCARGO_ALLEY_ID",source),
                new OracleParameter(":taskid",dt.Rows[0]["TASKID"].ToString())
            };
            int n = 0;
            try
            {
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parameter);
                msg = "";
            }
            catch (Exception ex)
            {
                msg = "入库给小车发指令之后，更新表schedule_tasky异常,异常信息为" + ex.Message + " orastr语句为:" + orastr;
                LogInfo.WriteLog("入库给小车发指令之后，更新表schedule_tasky异常,异常信息为" + ex.Message + " orastr语句为:" + orastr);
            }
            return n;
        }

        public static int D_UpdateRkSendToAgv(DataTable dt, string zxrwh, string source, string des, out string msg)
        {
            string orastr = "";
            if (dt.Rows[0]["TASKTYPE"].ToString() == "1")
            {
                orastr = "update schedule_task set status='3',phase='3',taskno=:zxrwh,SCARGO_ALLEY_ID=:SCARGO_ALLEY_ID,TCARGO_ALLEY_ID=:TCARGO_ALLEY_ID where taskid=:taskid";
            }
            else
            {
                orastr = "update schedule_task set status='3',phase='3',taskno=:zxrwh,SCARGO_ALLEY_ID=:SCARGO_ALLEY_ID,TCARGO_ALLEY_ID=:TCARGO_ALLEY_ID where taskid=:taskid";
            }
            OracleParameter[] parameter = new OracleParameter[]
            {
                new OracleParameter(":zxrwh",zxrwh),
                new OracleParameter(":TCARGO_ALLEY_ID",des),
                new OracleParameter(":SCARGO_ALLEY_ID",source),
                new OracleParameter(":taskid",dt.Rows[0]["TASKID"].ToString())
            };
            int n = 0;
            try
            {
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parameter);
                msg = "";
            }
            catch (Exception ex)
            {
                msg = "入库给小车发指令之后，更新表schedule_tasky异常,异常信息为" + ex.Message + " orastr语句为:" + orastr;
                LogInfo.WriteLog("入库给小车发指令之后，更新表schedule_tasky异常,异常信息为" + ex.Message + " orastr语句为:" + orastr);
            }
            return n;
        }
        #endregion

        #region 入库给小车发指令，根据目的地查询小车在途任务
        public static int D_GetRkOnJobByDes(string desaddress)
        {
            string orastr = "select count(*) from schedule_task where tasktype='1' and status In ('2','3') and RESERVE3 is not null and tcargo_alley_id=:desaddress";
            OracleParameter[] parameters = new OracleParameter[]{
                new OracleParameter(":desaddress",desaddress)
            };
            int n = 0;
            n = Convert.ToInt32(OracleHelper.ExecuteScalar(CommandType.Text, orastr, parameters));
            return n;
        }
        #endregion
        #region 出库给小车发指令，根据目的地查询小车在途任务
        public static int D_GetOnJobByDes(string desaddress)
        {
            string orastr = "select count(*) from schedule_task where tasktype='2' and status='3' and tcargo_alley_id=:desaddress";
            OracleParameter[] parameters = new OracleParameter[]{
                new OracleParameter(":desaddress",desaddress)
            };
            int n = 0;
            n = Convert.ToInt32(OracleHelper.ExecuteScalar(CommandType.Text, orastr, parameters));
            return n;
        }
        #endregion

        #region 根据TASKID查询调度指令

        public static DataSet D_GetSchByTaskIdArrive(string taskid)
        {
            string orastr = "select * from schedule_task where taskid=:taskid and status='2'";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":taskid", taskid)
            };
            DataSet ds = new DataSet();
            ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, parameters);
            return ds;
        }
        public static DataSet D_GetSchByTaskId(string taskid)
        {
            string orastr = "select * from schedule_task where taskid=:taskid and status='3'";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":taskid", taskid)
            };
            DataSet ds = new DataSet();
            ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, parameters);
            return ds;
        }
        #endregion

        #region 根据任务更新执行阶段，任务号，状态
        public static void P_UpdatePhase(string taskid, string phase, string status, string taskno)
        {
            if (taskid == "" && phase == "" && status == "" && taskno == "")
            {
                return;
            }
            List<OracleParameter> ls = new List<OracleParameter>();
            string orastr = "update schedule_task set  ";
            if (phase != "")
            {
                orastr += "phase=:phase";
                OracleParameter parameter = new OracleParameter(":phase", phase);
                ls.Add(parameter);
            }
            if (status != "")
            {
                orastr += ",status=:status";
                OracleParameter parameter = new OracleParameter(":status", status);
                ls.Add(parameter);
            }
            if (taskno != "")
            {
                orastr += ",taskno=:taskno";
                OracleParameter parameter = new OracleParameter(":taskno", taskno);
                ls.Add(parameter);
            }
            orastr += " where  taskid=:taskid";
            OracleParameter paramete = new OracleParameter(":taskid", taskid);
            ls.Add(paramete);
            OracleParameter[] parameters = ls.ToArray();
            try
            {
                int n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parameters);
            }
            catch (Exception ex)
            {
                LogInfo.WriteLog("更新调度指令执行阶段为" + phase + "任务为" + taskno + "状态为" + status + "出现异常，异常信息为：" + ex.Message + "执行语句为:" + orastr);
            }

        }
        #endregion

        #region 根据托盘条码查询发送给LED的信息
        public static DataSet D_GetJobInfoToLed(string traycode)
        {
            string orastr = @"select *
                                  from(select a.*,
                                               b.reserve1 actweight,
                                               c.productcode,
                                               c.productid,
                                               c.productname,
                                               c.lotinfo,
                                               c.assignnum,
                                               c.JOBNUM
                                          from schedule_task a, inout_job b, inout_jobdetail c
                                         where a.jobid = b.jobid
                                           and a.jobid = c.jobid
                                           and tasktype = '2'
                                           and a.traycode = '" + traycode + "' " +
                                           "order by a.createtime desc) where rownum = 1";
            DataSet ds = new DataSet();
            ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            return ds;
        }
        #endregion

        #region 接受AGV反馈的信息
        /// <summary>
        /// 根据请求编号查询信息
        /// </summary>
        /// <param name="reqcode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DataSet D_GetAgvCallBack(string reqcode, out string msg)
        {
            msg = "";
            DataSet ds = new DataSet();
            string orastr = "select * from RICO_AGV_CALLBACK where REQCODE=:reqcode";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":reqcode", reqcode)
            };
            try
            {
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, parameters);
            }
            catch (Exception ex)
            {
                msg = "根据请求编号" + reqcode + "查询反馈信息异常,异常状态为：" + ex.Message;
                LogInfo.WriteLog("根据请求编号" + reqcode + "查询反馈信息异常,异常状态为：" + ex.Message + "   数据库语句为:" + orastr);
            }
            return ds;
        }
        /// <summary>
        /// 将接受到回调信息插入表中
        /// </summary>
        /// <param name="rcvcallback"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int D_InsertCallBack(ReciveAgeCallBack rcvcallback, string status, out string msg)
        {
            msg = "";
            int n = 0;
            //string status = "1";
            #region 注释
            //string orastr = @"INSERT INTO RICO_AGV_CALLBACK
            //                                  (REQCODE,
            //                                   REQTIME,
            //                                   INTERFACENAME,
            //                                   COOX,
            //                                   COOY,
            //                                   CURRENTPOSITIONCODE,

            //                                   MAPCODE,
            //                                   MAPDATACODE,
            //                                   METHOD,
            //                                   PODCODE,
            //                                   PODDIR,
            //                                   REBOTCODE,
            //                                   TASKCODE,
            //                                   WBCODE,
            //                                   STATUS)
            //                                VALUES
            //                                  (:REQCODE,
            //                                   :REQTIME,
            //                                   :INTERFACENAME,
            //                                   :COOX,
            //                                   :COOY,
            //                                   :CURRENTPOSITIONCODE,

            //                                   :MAPCODE,
            //                                   :MAPDATACODE,
            //                                   :METHOD,
            //                                   :PODCODE,
            //                                   :PODDIR,
            //                                   :REBOTCODE,
            //                                   :TASKCODE,
            //                                   :WBCODE,
            //                                   :STATUS)";
            //OracleParameter[] parameters = new OracleParameter[]
            //{
            //    new OracleParameter(":REQCODE",rcvcallback.ReqCode),
            //    new OracleParameter(":REQTIME",rcvcallback.ReqTime),
            //    new OracleParameter(":INTERFACENAME",rcvcallback.InterfaceName),
            //    new OracleParameter(":COOX",rcvcallback.Coox),
            //    new OracleParameter(":COOY",rcvcallback.CooY),
            //    new OracleParameter(":CURRENTPOSITIONCODE",rcvcallback.CurrentPositionCode),

            //    new OracleParameter(":MAPCODE",rcvcallback.MapCode),
            //    new OracleParameter(":MAPDATACODE",rcvcallback.MapDataCode),
            //    new OracleParameter(":METHOD",rcvcallback.Method),
            //    new OracleParameter(":PODCODE",rcvcallback.PodCode),
            //    new OracleParameter(":PODDIR",rcvcallback.PodDir),
            //    new OracleParameter(":REBOTCODE",rcvcallback.RobotCode),
            //    new OracleParameter(":TASKCODE",rcvcallback.TaskCode),
            //    new OracleParameter(":WBCODE",rcvcallback.WbCode),
            //    new OracleParameter(":STATUS",status)
            //};
            #endregion
            string orastr = @"INSERT INTO RICO_AGV_CALLBACK
                                              (REQCODE,
                                               REQTIME,
                                               INTERFACENAME,
                                               COOX,
                                               COOY,
                                               CURRENTPOSITIONCODE,
                                   
                                               MAPCODE,
                                               MAPDATACODE,
                                               METHOD,
                                               PODCODE,
                                               PODDIR,
                                               REBOTCODE,
                                               TASKCODE,
                                               WBCODE,
                                               STATUS)
                                            VALUES
                                              ('" + rcvcallback.ReqCode
                                               + "','" + rcvcallback.ReqTime
                                               + "','" + rcvcallback.InterfaceName
                                               + "','" + rcvcallback.CooX
                                               + "','" + rcvcallback.CooY
                                               + "','" + rcvcallback.CurrentPositionCode
                                               + "','" + rcvcallback.MapCode
                                               + "','" + rcvcallback.MapDataCode
                                               + "','" + rcvcallback.Method
                                               + "','" + rcvcallback.PodCode
                                               + "','" + rcvcallback.PodDir
                                               + "','" + rcvcallback.RobotCode
                                               + "','" + rcvcallback.TaskCode
                                               + "','" + rcvcallback.WbCode
                                               + "','" + status + "')";
            try
            {
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                msg = "将请求编号" + rcvcallback.ReqCode + "调度" + rcvcallback.TaskCode + "插入表中异常";
                LogInfo.WriteLog(msg + ",异常状态为：" + ex.Message + "异常语句为:" + orastr);
            }
            return n;
        }
        /// <summary>
        /// 任务下发之后更新状态
        /// </summary>
        /// <param name="reqcode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static int D_UpdateCallBack(string reqcode, string status, out string msg)
        {
            msg = "";
            int n = 0;
            string orastr = @"UPDATE RICO_AGV_CALLBACK
                               SET STATUS              = :STATUS,
                                   FINISHTIME          = sysdate
                             WHERE REQCODE = :OLD_REQCODE";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":STATUS", status), new OracleParameter(":OLD_REQCODE", reqcode)
            };
            try
            {
                n = OracleHelper.ExecuteNonQuery(CommandType.Text, orastr, parameters);

            }
            catch (Exception ex)
            {
                msg = "更新请求编号" + reqcode + "异常";
                LogInfo.WriteLog(msg + "，异常信息为" + ex.Message + " 异常语句为：" + orastr);
            }
            return n;
        }
        /// <summary>
        /// 获取未执行的AGV回调任务
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DataSet D_GetUnDoCallBack(out string msg)
        {
            msg = "";
            DataSet ds = new DataSet();
            string orastr = "select * from RICO_AGV_CALLBACK where status='1'";
            try
            {
                ds = OracleHelper.ExecuteDataSet(CommandType.Text, orastr, null);
            }
            catch (Exception ex)
            {
                msg = "获取未处理AGV任务异常";
                LogInfo.WriteLog(msg + "，异常状态为:" + ex.Message + " 异常语句为:" + orastr);
            }
            return ds;
        }
        #endregion
    }
}
