using BaseData;
using ComResolution;
using DataOperate;
using LOG;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.DesignerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLAgv : CRLBase
    {
        public Log log;
        public delegate void Notify(string type, string msg);
        public event Notify NotifyEvent;
        private readonly static string ip = ConfigurationManager.AppSettings["AGVIP"];
        private readonly static string port = ConfigurationManager.AppSettings["AGVPORT"];
        private readonly static string wcsip = ConfigurationManager.AppSettings["WCSIP"];
        private readonly static string wcsport = ConfigurationManager.AppSettings["WCSPORT"];
        private readonly static string InTaskType = ConfigurationManager.AppSettings["InTaskType"];
        private readonly static string PallentsInTaskType = ConfigurationManager.AppSettings["PallentsInTaskType"];
        private readonly static string OutTaskType = ConfigurationManager.AppSettings["OutTaskType"];
        private readonly static string Arrive = ConfigurationManager.AppSettings["Arrive"];
        private readonly static string Finish = ConfigurationManager.AppSettings["Finish"];
        readonly CRLTRANSControl crl;

        public void Run()
        {
            OpenServices();
            //启动入库口给AGV发送指令线程
            Task.Run(SendToAgvMsg);
            //启动出库口给AGV发送指令线程
            Task.Run(CKSendToAgvMsg);
        }

        /// <summary>
        /// 出库口待执行任务发送给AGV
        /// </summary>
        private void CKSendToAgvMsg()
        {
            while (true)
            {
                for (int i = 1; i <= 7; i++)
                {
                    //查询出库口输送机是否有到位信号
                    var outts = lsTransport.Find(s => s.DTYPE == "103" && s.BTID == i.ToString() && s.DWXH == "1");
                    if (outts == null)
                    {
                        continue;
                    }

                    DataSet ds = DataTrans.D_GetSchByTaskno(outts.ZXRWH);
                    if (ds != null)
                    {
                        //查询目标巷道输送机是否空闲
                        List<TransportStr> ls = lsTransport.FindAll(s => s.DTYPE == "103" && s.BTID == i.ToString() && s.DWXH == "1");
                        if (ls == null) 
                        {
                            continue;
                        }
                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            string desAddress = string.Empty;
                            string ssj = string.Empty;
                            foreach (var item in ls)
                            {
                                var ts = item;
                                crl.HsWcsReadSSJ(ref ts);
                                if (ts.KXBZ != "1")
                                {
                                    continue;
                                }
                                int onjobnum = DataTrans.D_GetOnJobByDes(item.SSJID);
                                if (onjobnum == 0)
                                {
                                    desAddress = item.VAR5;
                                    ssj = item.SSJID;
                                    break;
                                }
                            }
                            //如果没有目的地就说明在途已经占满了出库口，终止循坏
                            if (string.IsNullOrEmpty(desAddress))
                            {
                                continue;
                            }

                            string msg = string.Empty;
                            SendToAgvInfo req = CKToModel(ds, OutTaskType, outts.VAR5, desAddress);
                            SendToAgvResult result = SendTask(req);

                            if (result.Message == "成功") 
                            {
                                string zxrwh = string.Empty;
                                if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["TASKNO"].ToString())) 
                                {
                                    zxrwh = ds.Tables[0].Rows[0]["TASKNO"].ToString();
                                }
                                else
                                {
                                    zxrwh = DataTrans.D_AllotTaskno().ToString();
                                }
                                int m = DataTrans.D_UpdateRkSendToAgv(ds.Tables[0], zxrwh, ds.Tables[0].Rows[0]["SCARGO_ALLEY_ID"].ToString(), ssj, out msg);
                                if (m > 0)
                                {
                                    NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                    log.WriteLog($"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                }
                                else
                                {
                                    string res = string.IsNullOrEmpty(msg) ? "更新失败" : $"更新出现异常！异常信息为{msg}";
                                    NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}{res}");
                                    log.WriteLog($"调度指令{ds.Tables[0].Rows[0]["Taskid"]}{res}");
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 将信息转换为生成任务发送给AGV实体类
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="zytype"></param>
        /// <param name="sourceAddress"></param>
        /// <param name="desAddress"></param>
        /// <returns></returns>
        private SendToAgvInfo CKToModel(DataSet ds,string zytype,string sourceAddress,string desAddress)
        {
            Positioncodepath[] ps = new Positioncodepath[3];
            if (zytype == InTaskType || zytype == PallentsInTaskType)
            {
                ps = new Positioncodepath[]
                {
                    new Positioncodepath{ PositionCode=sourceAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" }
                };
            }
            else
            {
                ps = new Positioncodepath[]
                {
                    new Positioncodepath{ PositionCode=sourceAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                };
            }

            SendToAgvInfo req = new SendToAgvInfo
            {
                ReqCode = Guid.NewGuid().ToString("N"),
                TaskType = zytype,
                PodCode = string.Empty,
                PodDir = "0",
                Priority = ds.Tables[0].Rows[0]["PRIORITY"].ToString(),
                AgvCode = string.Empty,
                TaskCode = ds.Tables[0].Rows[0]["TASKID"].ToString(),
                TokenCode = string.Empty,
                Data = string.Empty,
                PositionCodePaths = ps
            };

            return req;
        }

        /// <summary>
        /// 入库口待执行任务发送给AGV
        /// </summary>
        private void SendToAgvMsg()
        {
            while (true)
            {
                if (log.DataFileName != $"{DateTime.Now:yyyyMMdd}业务逻辑.txt") 
                {
                    log = new Log("业务逻辑", @".\RGV日志\");
                }
                for (int i = 1; i <= 5; i++)
                {
                    if (i == 5)
                    {
                        //查询入口载货台
                        TransportStr rkssj = lsTransport.Find(s => s.DTYPE == "106" && s.BTID == "1");
                        if (rkssj == null) 
                        {
                            continue;
                        }
                        DataSet ds = DataTrans.D_GetInwareTask(rkssj.SSJID);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            //查询目标巷道输送机是否空闲
                            TransportStr ts = lsTransport.Find(si => si.DTYPE == "105" && si.ALLEYID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString() && si.KXBZ == "1");

                            if (ts == null) 
                            {
                                continue;
                            }
                            if (ds.Tables[0].Rows.Count == 1) 
                            {
                                //查入库在途，在途存在，则不发目的地
                                int n = DataTrans.D_GetRkOnJobByDes(ds.Tables[0].Rows[0]["TCAGRO_ALLEY_ID"].ToString());
                                if (n > 0)
                                {
                                    continue;
                                }
                                var dessj = lsTransport.Find(t => t.ALLEYID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString());
                                if (dessj == null)
                                {
                                    continue;
                                }
                                string desaddress = rkssj.VAR5;
                                string msg = string.Empty;
                                SendToAgvInfo req = ToModel(ds, PallentsInTaskType, rkssj.VAR5, dessj.VAR5);
                                SendToAgvResult result = SendTask(req);

                                if (result.Message == "成功") 
                                {
                                    string zxrwh = string.Empty;
                                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["TASKID"].ToString())) 
                                    {
                                        zxrwh = ds.Tables[0].Rows[0]["TASKNO"].ToString();
                                    }
                                    else
                                    {
                                        zxrwh = DataTrans.D_AllotTaskno().ToString();
                                    }
                                    int m = DataTrans.D_UpdateRkSendToAgv(ds.Tables[0], zxrwh, rkssj.SSJID, ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString(), out msg);
                                    if (m > 0)
                                    {
                                        NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                        log.WriteLog($"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                    }
                                    else
                                    {
                                        string res = string.IsNullOrEmpty(msg) ? "更新失败" : $"更新出现异常！异常信息为{msg}";
                                        NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}{res}");
                                        log.WriteLog($"调度指令{ds.Tables[0].Rows[0]["Taskid"]}{res}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //查询入口载货台
                        var rkssj = lsTransport.Find(si => si.DTYPE == "101" && si.BTID == i.ToString());
                        var wxssj = lsTransport.Find(si => si.DTYPE == "107" && si.BTID == i.ToString());
                        if (rkssj == null || wxssj == null) 
                        {
                            continue;
                        }
                        DataSet ds = DataTrans.D_GetInwareTask(rkssj.SSJID);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            //如果称重实际重量未更新不能给AGV下发指令
                            if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["actweight"].ToString()))
                            {
                                continue;
                            }
                            //查询目标巷道输送机是否空闲
                            var ts = lsTransport.Find(si => si.DTYPE == "105" && si.ALLEYID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString() && si.KXBZ == "1");
                            if (ts == null)
                            {
                                continue;
                            }
                            if (ds.Tables[0].Rows.Count == 1)
                            {
                                //查入库在途，在途存在，则不发目的地
                                int n = DataTrans.D_GetRkOnJobByDes(ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString());
                                if (n > 0)
                                {
                                    continue;
                                }
                                string msg = string.Empty;
                                SendToAgvInfo req = ToModel(ds, InTaskType, rkssj.VAR5, wxssj.VAR5);
                                SendToAgvResult result = SendTask(req);
                                if (result.Message == "成功")
                                {
                                    string zxrwh = string.Empty;
                                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["TASKNO"].ToString()))
                                    {
                                        zxrwh = ds.Tables[0].Rows[0]["TASKNO"].ToString();
                                    }
                                    else
                                    {
                                        zxrwh = DataTrans.D_AllotTaskno().ToString();
                                    }
                                    int m = DataTrans.D_UpdateRkSendToAgv(ds.Tables[0], zxrwh, rkssj.SSJID, ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString(), out msg);
                                    if (m > 0)
                                    {
                                        NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                        log.WriteLog($"调度指令{ds.Tables[0].Rows[0]["Taskid"]}更新成功");
                                    }
                                    else
                                    {
                                        string res = string.IsNullOrEmpty(msg) ? "更新失败" : $"更新出现异常！异常信息为{msg}";
                                        NotifyEvent?.Invoke("S", $"调度指令{ds.Tables[0].Rows[0]["Taskid"]}{res}");
                                    }
                                }
                            }
                        }
                        else if (ds.Tables[0].Rows.Count > 1)
                        {
                            string[] arr = Array.ConvertAll(ds.Tables[0].Rows.Cast<DataRow>().ToArray(), r => r["TASKID"].ToString());
                            string taskstr = string.Join(",", arr, 0, arr.Length);
                            NotifyEvent?.Invoke("S", $"入库口{i}存在多个待执行任务，请检查并进行处理！待执行任务id{taskstr}");
                            log.WriteLog($"入库口{i}存在多个待执行任务，请检查并进行处理！待执行任务id{taskstr}");
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 开启REST服务
        /// </summary>
        private void OpenServices()
        {
            try
            {
                AgvCallBackService service = new AgvCallBackService();
                Uri baseAddress = new Uri($"http://{wcsip}:{wcsport}/");
                WebServiceHost _servicesHost = new WebServiceHost(service, baseAddress);

                WebHttpBinding binding = new WebHttpBinding
                {
                    TransferMode = TransferMode.Buffered,
                    MaxBufferPoolSize = (long)2 * 1024 * 1024 * 1024,
                    MaxReceivedMessageSize = (long)2 * 1024 * 1024 * 1024,
                    MaxBufferSize = 2147483647,
                    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                    Security = { Mode = WebHttpSecurityMode.None }
                };

                _servicesHost.AddServiceEndpoint(typeof(IAgvResultCallBack), binding, baseAddress);
                //_servicesHost.Opened += (sender, e) =>
                //{
                //    NotifyEvent?.Invoke("R", "web服务开启...");
                //    log.WriteLog("web服务开启...");
                //};
                _servicesHost.Opened += delegate
                {
                    NotifyEvent?.Invoke("R", "web服务开启...");
                    log.WriteLog("web服务开启...");
                };
                _servicesHost.Open();
            }
            catch (Exception ex)
            {
                NotifyEvent?.Invoke("R", $"web服务异常，异常信息为:{ex.Message}");
                log.WriteLog($"web服务异常，异常信息为:{ex.Message}");
            }
        }

        #region 处理AGV回传数据
        public string HandleCallBackData(ReciveAgeCallBack agvCallBack)
        {
            string msg = string.Empty;
            //外形检测出到达
            if (agvCallBack.Method == Arrive)
            {
                DataSet ds = DataTrans.D_GetSchByTaskIdArrive(agvCallBack.TaskCode);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    #region 读取外形检测是否报警
                    bool flag = false;
                    string ssjid = ds.Tables[0].Rows[0]["SNUMBER"].ToString();
                    TransportStr ts = lsTransport.Find(si => si.SSJID == ssjid);
                    if (ts == null)
                    {
                        return string.Empty;
                    }

                    int btid = Convert.ToInt32(ts.BTID);
                    byte[] bty = new byte[2];
                    BitArray arr = new BitArray(bty);
                    if (!HsWcsReadalarm(out arr))
                    {
                        return "";
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int start = (btid - 1) * 3;
                            flag = arr[start + i];
                            if (flag == true)
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    string desaddress = string.Empty;
                    //如果报警返回
                    if (flag == true)
                    {
                        desaddress = ts.VAR5;
                    }
                    else
                    {
                        TransportStr rkssj = lsTransport.Find(t => t.ALLEYID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString());
                        if (rkssj == null)
                        {
                            return string.Empty;
                        }
                        desaddress = rkssj.VAR5;
                    }

                    ContinueToAgvInfo req = ContinueToModel(ds, desaddress);
                    SendToAgvResult result = ContinueTask(req);
                    if (result.Message == "成功" && flag)
                    {
                        NotifyEvent?.Invoke("R", $"外形检测不合格，给AGV发送返回起始点成功，任务id为：{agvCallBack.TaskCode}目的地{desaddress}");
                        log.WriteLog($"外形检测不合格，给AGV发送返回起始点成功，任务id为：{agvCallBack.TaskCode}目的地{desaddress}");

                        DataTrans.P_UpdatePhase(agvCallBack.TaskCode, "1", "5", string.Empty);
                    }
                    else if (result.Message == "成功" && !flag)
                    {
                        DataTrans.P_UpdatePhase(agvCallBack.TaskCode, "3", "3", string.Empty);
                        NotifyEvent?.Invoke("R", $"外形检测合格，给AGV发送目标巷道成功，任务Id为{agvCallBack.TaskCode}目的地{desaddress}");
                        log.WriteLog($"外形检测合格，给AGV发送目标巷道成功，任务Id为{agvCallBack.TaskCode}目的地{desaddress}");
                        msg = "OK";
                    }
                    else
                    {
                        NotifyEvent?.Invoke("R", $"给AGV发送目标巷道失败，任务id为：{agvCallBack.TaskCode}");
                        log.WriteLog($"给AGV发送目标巷道失败，任务id为：{agvCallBack.TaskCode}");
                        msg = "Lost";
                    }
                }
            }
            //放货完成
            else if (agvCallBack.Method == Finish) 
            {
                DataSet ds = DataTrans.D_GetSchByTaskId(agvCallBack.TaskCode);
                if (ds != null && ds.Tables[0].Rows.Count > 0)  
                {
                    TransportStr ts;
                    if (ds.Tables[0].Rows[0]["TASKTYPE"].ToString() == "1") 
                    {
                        ts = lsTransport.Find(si => si.ALLEYID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString());
                    }
                    else
                    {
                        ts = lsTransport.Find(si => si.SSJID == ds.Tables[0].Rows[0]["TCARGO_ALLEY_ID"].ToString());
                    }
                    if (ts == null) 
                    {
                        return string.Empty;
                    }
                    ts.TRAYCODE = ds.Tables[0].Rows[0]["TRAYCODE"].ToString();
                    ts.ZXRWH = ds.Tables[0].Rows[0]["TASKNO"].ToString();

                    if (crl.HsWriteToSSJ(ts))  
                    {
                        NotifyEvent?.Invoke("S", $"根据AGV反馈任务与id{agvCallBack.TaskCode}放货完成，给输送机{ts.SSJID}下发任务成功");
                        log.WriteLog($"根据AGV反馈任务id{agvCallBack.TaskCode}放货完成，给输送机{ts.SSJID}下发任务成功");
                        if (ds.Tables[0].Rows[0]["TASKTYPE"].ToString() == "1")
                        {
                            DataTrans.P_UpdatePhase(agvCallBack.TaskCode, "1", "3", string.Empty);
                        }
                        else if (ds.Tables[0].Rows[0]["TASKTYPE"].ToString() == "2") 
                        {
                            DataTrans.P_UpdatePhase(agvCallBack.TaskCode, "1", "4", string.Empty);
                        }
                        msg = "OK";
                    }
                    else
                    {
                        NotifyEvent?.Invoke("S", $"根据AGV反馈任务id{agvCallBack.TaskCode}放货完成，给输送机{ts.SSJID}下发任务失败");
                        log.WriteLog($"根据AGV反馈任务id{agvCallBack.TaskCode}放货完成，给输送机{ts.SSJID}下发任务失败");
                        msg = "Lost";
                    }
                }
            }
            return msg;
        }
        #endregion

        /// <summary>
        /// 将信息转化继续执行发送给AGV的实体类
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="desaddress"></param>
        /// <returns></returns>
        public static ContinueToAgvInfo ContinueToModel(DataSet ds, string desaddress)
        {
            ContinueToAgvInfo req = new ContinueToAgvInfo
            {
                ReqCode = Guid.NewGuid().ToString("N"),
                ReqTime = "",
                ClientCode = "",
                TokenCode = "",
                InterFaceName = "",
                WbCode = "",
                PodCode = "",
                AgvCode = "",
                TaskCode = ds.Tables[0].Rows[0]["TaskId"].ToString(),
                TaskSeq = "",
                Data = "",
                NextPositionCode = new Positioncodepath
                {
                    PositionCode = desaddress,
                    Type = "00",
                },
            };
            return req;
        }

        /// <summary>
        /// 继续执行给AGV发送任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public SendToAgvResult ContinueTask(ContinueToAgvInfo req)
        {
            string url = $"http://{ip}:{port}/cms/services/rest/hikRpcService/continueTasl";
            string param = JsonConvert.SerializeObject(req);
            string result = Post(url, param);
            SendToAgvResult sdresult = JsonConvert.DeserializeObject<SendToAgvResult>(result);
            return sdresult;
        }

        /// <summary>
        /// 发送并反馈结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private string Post(string url,string param)
        {
            byte[] dataArray = Encoding.Default.GetBytes(param);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            request.ContentType = "application/json;charset=UTF-8";
            Stream write = null ;
            try
            {
                write = request.GetRequestStream();
                write.Write(dataArray, 0, dataArray.Length);
            }
            catch (Exception)
            {
                write = null;
                NotifyEvent?.Invoke("R", "连接服务器失败");
            }
            finally
            {
                write?.Close();
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
                NotifyEvent?.Invoke("R", ex.Message);
            }

            Stream s = response.GetResponseStream();
            string strData;
            string strValue = string.Empty;
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((strData = Reader.ReadLine()) != null) 
            {
                strValue += strData + Environment.NewLine;
            }
            return strValue;
        }

        /// <summary>
        /// 读取外形检测报警信息
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private bool HsWcsReadalarm(out BitArray arr)
        {
            int start = 0;
            byte[] bty = new byte[2];
            arr = new BitArray(bty);
            try
            {
                if (HsPLCList["A"].HsRead($"DB18.{start}", 2, ref bty)) 
                {
                    arr = new BitArray(bty);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog($"读取DB48异常，状态为：{ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 将信息转换为生成任务发送给AGV的实体类
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="zyType"></param>
        /// <param name="sourceAddress"></param>
        /// <param name="desAddress"></param>
        /// <returns></returns>
        private SendToAgvInfo ToModel(DataSet ds,string zyType,string sourceAddress,string desAddress)
        {
            string snumber = ds.Tables[0].Rows[0]["SNumber"].ToString();
            SendToAgvInfo req = new SendToAgvInfo
            {
                ReqCode = Guid.NewGuid().ToString("N"),
                TaskType = zyType,
                PodCode = string.Empty,
                PodDir = "0",
                Priority = ds.Tables[0].Rows[0]["PRIORITY"].ToString(),
                AgvCode = string.Empty,
                TaskCode = ds.Tables[0].Rows[0]["TASKID"].ToString(),
                TokenCode = string.Empty,
                Data = string.Empty
            };

            if (zyType == InTaskType)
            {
                req.PositionCodePaths = new Positioncodepath[]
                {
                    new Positioncodepath{ PositionCode=sourceAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                };
            }
            else if (zyType == PallentsInTaskType)
            {
                req.PositionCodePaths = new Positioncodepath[]
                {
                    new Positioncodepath{ PositionCode=sourceAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                };
            }
            else
            {
                req.PositionCodePaths = new Positioncodepath[]
                {
                    new Positioncodepath{ PositionCode=sourceAddress,Type="00" },
                    new Positioncodepath{ PositionCode=desAddress,Type="00" },
                };
            }

            return req;
        }

        /// <summary>
        /// 生成任务给AGV发送任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private SendToAgvResult SendTask(SendToAgvInfo req)
        {
            string url = $"http://{ip}:{port}/cms/services/rest/hikRpcService/genAgvSchedulingTask";
            string param = JsonConvert.SerializeObject(req);
            string result = Post(url, param);
            SendToAgvResult sdResult = JsonConvert.DeserializeObject<SendToAgvResult>(result);
            return sdResult;
        }
    }
}
