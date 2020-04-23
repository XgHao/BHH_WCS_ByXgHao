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
using System.Text;
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

        CRLTRANSControl crl;

        public void Run()
        {
            
        }


        private void OpenServices()
        {
            try
            {
                
            }
            catch (Exception)
            {

                throw;
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
    }
}
