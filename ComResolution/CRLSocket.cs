using LOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;
using COMRW;
using TimeSpanCount;
using System.Data;
using BaseData;
using System.Security.Cryptography;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace ComResolution
{
    public class CRLSocket : CRLBase
    {
        //将异常内容反馈给窗体
        public delegate void NotifyShow(string type, string msg);
        public event NotifyShow NotifyShowEvent;

        //获取接收到的字节事件
        public delegate void RcvStrDG(object state);
        public event RcvStrDG RcvStrEvent;

        private readonly CRCSCKObject crsk;
        private readonly CRLSocketConnect crConnect = new CRLSocketConnect();
        private string SCName;
        private string IP;
        private int Port;
        private int connectStatus;
        Log logWrite = new Log("系统日志", @".\Socket日志\");

        private Timer checkTime = null;

        int taskid = 0;
        const int autoSdbnumber = 112;
        const int autoRdbnumber = 113;
        const int dbNumber = 50;
        const int dbLastgo = 27;

        /// <summary>
        /// 用于存储道口产品信息
        /// </summary>
        Hashtable hasallotInfo = new Hashtable();

        /// <summary>
        /// 用于储存正在作业的产品
        /// </summary>
        Hashtable jobProductInfo = new Hashtable();

        public System.Timers.Timer CheckTime
        {
            get
            {
                if (checkTime == null) 
                {
                    checkTime = new Timer();
                }
                return checkTime;
            }
        }

        public void TransDeviceInit()
        {
            string _ip = ConfigurationManager.AppSettings["B"].ToString();
            if (!HsPLCList.ContainsKey("B")) 
            {
                HsPLCList.Add("B", new HsControlServer(_ip));
                PLCFlag.Add("B", false);
                HsPLCList["B"].HsServerConnect();
            }
        }

        public void Run()
        {
            crConnect.NotifyCommandEvent += new CRLSocketConnect.NotifyCommandDG(SocketNotify);
            SocketConnect();
            Task.Run(TimeScanBox);
            Task.Run(TimeScanPick);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        private bool SocketConnect()
        {
            bool flag = false;
            try
            {
                crConnect.ServerSck();
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"连接服务器{SCName}发生异常，异常信息为{ex.Message}");
                NotifyShowEvent?.Invoke("C", $"连接服务器{SCName}发生异常，异常信息为{ex.Message}");
            }
            return flag;
        }

        /// <summary>
        /// 读取道口信息并进行处理
        /// </summary>
        private void TimeScanBox()
        {
            while (true)
            {
                AutoOPC();
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 给拆拣机械臂发送拆拣信息以及获取完成信息
        /// </summary>
        private void TimeScanPick()
        {
            while (true)
            {
                PickRebortSend();
                PickAutoOPC();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 如果连接失败，定时检测网络，网络正常则自动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            CRCSCKObject findcob = CrcSckList.Find(info => info.SCName == SCName && info.Port == Port);

            if (SocketConnect())
            {
                checkTime.Stop();
                if (findcob != null) 
                {
                    findcob.ConnectStatus = 1;
                }
                logWrite.WriteLog($"重新连接成功，设备编号为{crsk.SCName}IP地址为{crsk.IpAddress}端口号为{crsk.Port}");
            }
            else
            {
                if (findcob != null) 
                {
                    findcob.ConnectStatus = 0;
                }
                logWrite.WriteLog($"重新连接失败，设备编号为{crsk.SCName}IP地址为{crsk.IpAddress}端口号为{crsk.Port}");
            }
        }


        /// <summary>
        /// 码垛区OPC通信
        /// </summary>
        private void AutoOPC()
        {
            for (int i = 1; i <= 3; i++)
            {
                TransportStr devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "301");
                if (devicessj != null) 
                {
                    SSJTaskAllot(ref devicessj);
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                var devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "302");
                if (devicessj != null) 
                {
                    SSJTaskAllot(ref devicessj);
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                var devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "303");
                if (devicessj != null) 
                {
                    SSJTaskAllot(ref devicessj);
                }
            }
        }

        /// <summary>
        /// 完成给输送机下发任务
        /// </summary>
        /// <param name="si"></param>
        private void SSJTaskAllot(ref TransportStr si)
        {

        }

        /// <summary>
        /// 读取码垛道口信息
        /// </summary>
        /// <param name="sdstart"></param>
        /// <param name="box_arrive"></param>
        /// <returns></returns>
        private bool WcsReadBoxSSJ(int sdstart,ref string box_arrive)
        {
            try
            {
                //当前输送机对应电机对应开始字节，读取电机信息（主要到位空闲信号）DB112
                byte[] djbty = new byte[4];
                if (HsPLCList.Count > 0 && HsPLCList.ContainsKey("B")) 
                {
                    if (HsPLCList["B"].HsRead($"DB{autoSdbnumber}.{sdstart}", 4, ref djbty))
                    {
                        //将电机对应输送机id的第一个字节存储到buffer中，并获取bit
                        int djint = djbty[0];
                        byte[] buffer = BitConverter.GetBytes(djint);
                        BitArray arr = new BitArray(buffer);
                        //尾箱信号
                        box_arrive = Convert.ToInt32(arr[0]).ToString();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"读取码垛道口信息异常，异常信息：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取DB50信息
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        private bool WcsReadDb50(ref TransportStr si)
        {
            try
            {
                si.TRAYCODE = string.Empty;
                //读取当前输送机条码信息等DB50
                int start = Convert.ToInt32(si.VAR3);
                byte[] bty = new byte[20];
                if (HsPLCList.ContainsKey(si.VAR1)) 
                {
                    if (HsPLCList[si.VAR1].HsRead($"DB{dbNumber}.{start}", 20, ref bty)) 
                    {
                        //将读取到的信息更新到实体类
                        si.TRAYCODE = (bty[2] * 256 * 256 * 256 + bty[3] * 256 * 256 + bty[4] * 256 + bty[5]).ToString();   //条码
                        if (Convert.ToInt32(si.TRAYCODE) > 0) 
                        {
                            si.TRAYCODE = si.TRAYCODE.PadLeft(8, '0');
                        }
                        else
                        {
                            si.TRAYCODE = string.Empty;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"码垛区读取DB50信息异常，异常信息{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Socket获取报文
        /// </summary>
        /// <param name="command"></param>
        /// <param name="state"></param>
        private void SocketNotify(string command, object state)
        {
            if (command.Equals("RecvData"))
            {
                //获取PLC发送过来的信息
                RcvStrEvent?.Invoke(state);

                //获取子报文头部，根据头部判断有多少个子报文任务
                byte[] buffer = new byte[290];
                buffer = state as byte[];
                string getByte = BitConverter.ToString(buffer, 0).Replace("-", string.Empty).ToUpper();

                logWrite.WriteLog($"接受报文{getByte}");

                //内部最多五个子报文
                for (int i = 0; i < 5; i++)
                {
                    byte[] tmpBty = new byte[2];
                    for (int j = 0; j < 2; j++)
                    {
                        tmpBty[j] = buffer[i * 50 + 36 + j];
                    }
                    string tmpstart = BitConverter.ToString(tmpBty, 0).Replace("-", string.Empty).ToUpper();
                    if (tmpstart != "FCFC")
                    {
                        break;
                    }
                    else
                    {
                        byte[] childBty = new byte[50];
                        for (int k = 0; k < 50; k++)
                        {
                            childBty[k] = buffer[i * 50 + 36 + k];
                        }
                        ChildInfo(buffer, childBty);
                    }
                }
            }
            else if (command == "Break" || command == "Connect") 
            {
                NotifyShowEvent?.Invoke("C", state.ToString());
                logWrite.WriteLog($"{command}:{state}");
            }
        }

        /// <summary>
        /// 子报文处理，并发送给PLC
        /// </summary>
        /// <param name="bty"></param>
        /// <param name="buffer"></param>
        public void ChildInfo(byte[] bty, byte[] buffer)
        {
            #region 获取到PLCsocket发送过来的报文
            string startStr = string.Empty; //起始符(0-1)
            string msgName = string.Empty;  //报文名称(2-5)
            string msgLen = string.Empty;   //报文长度(6-7)
            string BCR_Nr = string.Empty;   //读码器编号(8-9)
            string ParcelID = string.Empty; //货物ID号或目的地(10-11)
            string Task_ID = string.Empty;  //任务号ID(12-13)，或者实际道口号
            string Destination = string.Empty;  //目的地(14-15)
            string PasteResult = string.Empty;  //是否为贴标(16-17)
            string remark1 = string.Empty;      //备注1(18-19)
            string remark2 = string.Empty;      //备注2(20-21)
            string remark3 = string.Empty;      //备注3(22-23)
            string Barcode = string.Empty;      //条码(24-47)
            string endStr = string.Empty;       //结束符(48-49)
            byte[] bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i];
            }
            startStr = BitConverter.ToString(bf, 0).Replace("-", string.Empty).ToUpper();
            bf = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bf[i] = buffer[i + 2];
            }
            msgName = Encoding.Default.GetString(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 6];
            }
            msgLen = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 8];
            }
            BCR_Nr = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 10];
            }
            ParcelID = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 12];
            }
            Task_ID = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 14];
            }
            Destination = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 16];
            }
            PasteResult = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 18];
            }
            remark1 = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 20];
            }
            remark2 = ByteToInt(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 22];
            }
            remark3 = ByteToInt(bf);
            bf = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                bf[i] = buffer[i + 25];
            }
            Barcode = Encoding.Default.GetString(bf);
            bf = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                bf[i] = buffer[i + 48];
            }
            endStr = BitConverter.ToString(bf, 0).Replace("-", string.Empty).ToUpper();
            #endregion

            #region 目的地处任务-给PLC发送任务
            if (msgName == "P2DR")
            {
                PerformanceTimer timer = new PerformanceTimer();

                if (string.IsNullOrEmpty(Barcode.Replace("\0", "")))
                {
                    return;
                }
                timer.Start();
                DateTime startTime = DateTime.Now;
                msgName = "W2DS";
                if (BCR_Nr == "30010")
                {
                    string iffqc = "N";
                    if (iffqc == "Y")
                    {
                        if (Barcode.Substring(0, 6) == "NoRead")
                        {
                            Destination = "1";
                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                string productCod = ds.Tables[0].Rows[0]["PRODUCTCODE"].ToString();
                                string lotinfo = ds.Tables[0].Rows[0]["CMOLOTCODE"].ToString();
                                ProductCountInfo pci = ProcInfoList.Find(tmp => tmp.ProductCode == productCod && tmp.LotInfo == lotinfo);

                                if (pci == null)
                                {
                                    pci = new ProductCountInfo
                                    {
                                        ProductCode = productCod,
                                        LotInfo = lotinfo,
                                        GetNum = 1,
                                    };
                                    ProcInfoList.Add(pci);
                                    Destination = "2";
                                }
                                else
                                {
                                    pci.GetNum += 1;
                                    if (pci.GetNum == 2 || pci.GetNum == 3)
                                    {
                                        Destination = "2";
                                    }
                                    else if (pci.GetNum > 50 && pci.GetNum % 50 == 1)
                                    {
                                        Destination = "2";
                                    }
                                    else
                                    {
                                        Destination = "1";
                                    }
                                }
                            }
                            else
                            {
                                Destination = "1";
                            }
                        }
                    }
                    else
                    {
                        //抽检
                        Destination = "1";
                    }
                }
                else if (BCR_Nr == "30039")
                {
                    if (Barcode.Substring(0, 6) == "NoRead")
                    {
                        Destination = "7";
                    }
                    else
                    {
                        GetShelvfWay(Barcode);
                        Destination = recdetail.WAYID;
                    }
                }
                else
                {
                    if (Barcode.Substring(0, 6) == "NoRead")
                    {
                        Destination = "2";
                    }
                    else
                    {
                        //根据药监码生成自动收获详细记录
                        if (GenRecDetail(Barcode, out string msg))
                        {
                            Destination = "1";
                        }
                        else
                        {
                            Destination = msg == "0" ? "3" : "2";
                        }
                    }
                }
                //task_id从1开始到30000，大于30000重置开始
                if (taskid >= 30000)
                {
                    taskid = 0;
                }
                Task_ID = taskid++.ToString();

                byte[] bfsnd = new byte[50];
                byte[] bfstart = new byte[2];
                //16进制转为byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(startStr.Substring(i * 2, 2), 16);
                    bfstart[i] = (byte)k;
                }
                byte[] bfmsgname = new byte[4];
                bfmsgname = Encoding.Default.GetBytes(msgName);
                byte[] bfmsglen = new byte[2];
                bfmsglen = IntToByte(Convert.ToInt32(msgLen));
                byte[] bfBCR = new byte[2];
                bfBCR = IntToByte(Convert.ToInt32(BCR_Nr));
                byte[] bfparid = new byte[2];
                bfparid = IntToByte(Convert.ToInt32(ParcelID));
                byte[] bftskid = new byte[2];
                bftskid = IntToByte(Convert.ToInt32(Task_ID));
                byte[] bfdes = new byte[2];
                bfdes = IntToByte(Convert.ToInt32(Destination));
                byte[] bfpsresult = new byte[2];
                bfpsresult = IntToByte(Convert.ToInt32(PasteResult));
                byte[] bfrm1 = new byte[2];
                bfrm1 = IntToByte(Convert.ToInt32(remark1));
                byte[] bfrm2 = new byte[2];
                bfrm2 = IntToByte(Convert.ToInt32(remark2));
                byte[] bfrm3 = new byte[2];
                bfrm3 = IntToByte(Convert.ToInt32(remark3));
                byte[] bfcode = new byte[24];
                bfcode = Encoding.Default.GetBytes(Barcode);
                byte[] bfend = new byte[2];
                //16进制转byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(endStr.Substring(i * 2, 2), 16);
                    bfend[i] = (byte)k;
                }
                for (int i = 0; i < bfstart.Length; i++)
                {
                    buffer[i] = bfstart[i];
                }
                for (int i = 0; i < bfmsgname.Length; i++)
                {
                    buffer[i + 2] = bfmsgname[i];
                }
                for (int i = 0; i < bfmsglen.Length; i++)
                {
                    buffer[i + 6] = bfmsglen[i];
                }
                for (int i = 0; i < bfBCR.Length; i++)
                {
                    buffer[i + 8] = bfBCR[i];
                }
                for (int i = 0; i < bfparid.Length; i++)
                {
                    buffer[i + 10] = bfparid[i];
                }
                for (int i = 0; i < bftskid.Length; i++)
                {
                    buffer[i + 12] = bftskid[i];
                }
                for (int i = 0; i < bfdes.Length; i++)
                {
                    buffer[i + 14] = bfdes[i];
                }
                for (int i = 0; i < bfpsresult.Length; i++)
                {
                    buffer[i + 16] = bfpsresult[i];
                }
                for (int i = 0; i < bfrm1.Length; i++)
                {
                    buffer[i + 18] = bfrm1[i];
                }
                for (int i = 0; i < bfrm2.Length; i++)
                {
                    buffer[i + 20] = bfrm2[i];
                }
                for (int i = 0; i < bfrm3.Length; i++)
                {
                    buffer[i + 22] = bfrm3[i];
                }
                for (int i = 0; i < bfcode.Length; i++)
                {
                    buffer[i + 24] = bfcode[i];
                }
                for (int i = 0; i < bfend.Length; i++)
                {
                    buffer[i + 48] = bfend[i];
                }

                //给PLC发送指令
                if (crConnect.Send(ByteSplit(bty, buffer)))
                {
                    string sendbty = BitConverter.ToString(ByteSplit(bty, buffer), 0).Replace("-", string.Empty).ToUpper();
                    logWrite.WriteLog($"发送报文{sendbty}");
                    if (Barcode.Substring(0, 6) == "NoRead")
                    {
                        NotifyShowEvent?.Invoke("R", $"设备{BCR_Nr}未读取到药监码信息分{Destination}道口");
                        logWrite.WriteLog($"设备{BCR_Nr}未读取到药监码信息分{Destination}道口");
                    }
                    else
                    {
                        if (BCR_Nr == "30039")
                        {
                            if (recdetail.IsLast != "1" && Convert.ToDouble(recdetail.Num) >= 1)
                            {
                                int n = 0;
                                if (n > 0)
                                {
                                    NotifyShowEvent?.Invoke("R", $"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                                    logWrite.WriteLog($"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                                }
                            }
                            else
                            {
                                NotifyShowEvent?.Invoke("R", $"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                                logWrite.WriteLog($"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                            }
                        }
                        else
                        {
                            NotifyShowEvent?.Invoke("R", $"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                            logWrite.WriteLog($"设备{BCR_Nr}给药监码{Barcode}分配{Destination}道口成功");
                        }
                    }
                }
                DateTime stoptime = DateTime.Now;
                timer.Stop();
                logWrite.WriteLog($"条码{Barcode}设备{BCR_Nr}下发任务成功，用时{timer.Duration}开始时间{startTime}结束时间{stoptime}");
            }
            #endregion

            #region 重分道口
            else if (msgName == "P3DR")
            {
                ModifyShelvfWay(Barcode, BCR_Nr);
                msgName = "W3DS";
                Destination = recdetail.WAYID;
                byte[] bfsnd = new byte[50];
                byte[] bfstart = new byte[2];
                //16进制转byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(startStr.Substring(i * 2, 2), 16);
                    bfstart[i] = (byte)k;
                }
                byte[] bfmsgname = new byte[4];
                bfmsgname = Encoding.Default.GetBytes(msgName);
                byte[] bfmsglen = new byte[2];
                bfmsglen = IntToByte(Convert.ToInt32(msgLen));
                byte[] bfBCR = new byte[2];
                bfBCR = IntToByte(Convert.ToInt32(BCR_Nr));
                byte[] bfparid = new byte[2];
                bfparid = IntToByte(Convert.ToInt32(ParcelID));
                byte[] bftskid = new byte[2];
                bftskid = IntToByte(Convert.ToInt32(Task_ID));
                byte[] bfdes = new byte[2];
                bfdes = IntToByte(Convert.ToInt32(Destination));
                byte[] bfpsresult = new byte[2];
                bfpsresult = IntToByte(Convert.ToInt32(PasteResult));
                byte[] bfrm1 = new byte[2];
                bfrm1 = IntToByte(Convert.ToInt32(remark1));
                byte[] bfrm2 = new byte[2];
                bfrm2 = IntToByte(Convert.ToInt32(remark2));
                byte[] bfrm3 = new byte[2];
                bfrm3 = IntToByte(Convert.ToInt32(remark3));
                byte[] bfcode = new byte[24];
                bfcode = Encoding.Default.GetBytes(Barcode);
                byte[] bfend = new byte[2];
                //16进制转byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(endStr.Substring(i * 2, 2), 16);
                    bfend[i] = (byte)k;
                }
                for (int i = 0; i < bfstart.Length; i++)
                {
                    buffer[i] = bfstart[i];
                }
                for (int i = 0; i < bfmsgname.Length; i++)
                {
                    buffer[i + 2] = bfmsgname[i];
                }
                for (int i = 0; i < bfmsglen.Length; i++)
                {
                    buffer[i + 6] = bfmsglen[i];
                }
                for (int i = 0; i < bfBCR.Length; i++)
                {
                    buffer[i + 8] = bfBCR[i];
                }
                for (int i = 0; i < bfparid.Length; i++)
                {
                    buffer[i + 10] = bfparid[i];
                }
                for (int i = 0; i < bftskid.Length; i++)
                {
                    buffer[i + 12] = bftskid[i];
                }
                for (int i = 0; i < bfdes.Length; i++)
                {
                    buffer[i + 14] = bfdes[i];
                }
                for (int i = 0; i < bfpsresult.Length; i++)
                {
                    buffer[i + 16] = bfpsresult[i];
                }
                for (int i = 0; i < bfrm1.Length; i++)
                {
                    buffer[i + 18] = bfrm1[i];
                }
                for (int i = 0; i < bfrm2.Length; i++)
                {
                    buffer[i + 20] = bfrm2[i];
                }
                for (int i = 0; i < bfrm3.Length; i++)
                {
                    buffer[i + 22] = bfrm3[i];
                }
                for (int i = 0; i < bfcode.Length; i++)
                {
                    buffer[i + 24] = bfcode[i];
                }
                for (int i = 0; i < bfend.Length; i++)
                {
                    buffer[i + 48] = bfend[i];
                }
                //给PLC发送指令
                if (crConnect.Send(ByteSplit(bty, buffer)))
                {
                    NotifyShowEvent?.Invoke("R", $"设备{BCR_Nr}药监码{Barcode}重新分配{Destination}道口成功");
                    logWrite.WriteLog($"设备{BCR_Nr}药监码{Barcode}重新分配{Destination}道口成功");
                }
            }
            #endregion

            #region 分拣信息
            else if (msgName == "P2SR")
            {
                BoxOPC(Task_ID);
                //进入道口根据分拣报告，将收获详细记录从list中移除
                RECDETAIL rcdetail = lsrecDetail.Find(rc => rc.Code == Barcode);
                if (rcdetail != null)
                {
                    int last = 0;
                    double hasmdnum = 0;
                    if (rcdetail.IsLast != "1" && Convert.ToDouble(rcdetail.Num) < 1)
                    {
                        //分拣成功给产品计数，并下发尾箱
                        last = IsLastBox(recdetail, ref hasmdnum) ? 1 : 0;
                    }
                    else
                    {
                        last = recdetail.IsLast == "1" ? 1 : 0;
                    }
                    if (!hascount.Contains(Task_ID))
                    {
                        if (WriteToDb27(Task_ID, last))
                        {
                            hascount.Add(Task_ID, Barcode);
                            lsrecDetail.Remove(rcdetail);
                        }
                    }
                    else if (hascount.Contains(Task_ID) && hascount[Task_ID].ToString() != Barcode)
                    {
                        if (WriteToDb27(Task_ID, last))
                        {
                            hascount[Task_ID] = Barcode;
                            lsrecDetail.Remove(rcdetail);
                        }
                    }
                }
                else
                {
                    DataSet dsauto = new DataSet();
                    if (dsauto != null && dsauto.Tables[0].Rows.Count > 0)
                    {
                        RECDETAIL recdetail = new RECDETAIL
                        {
                            Code = Barcode,
                            ProductID = dsauto.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                            LotInfo = dsauto.Tables[0].Rows[0]["LOTINFO"].ToString(),
                            ProductCode = dsauto.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                            PoID = dsauto.Tables[0].Rows[0]["POID"].ToString(),
                            PoDetailID = dsauto.Tables[0].Rows[0]["PODETAILID"].ToString(),
                            Num = dsauto.Tables[0].Rows[0]["NUM"].ToString(),
                            ProcudtName = dsauto.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                            WarehouseID = dsauto.Tables[0].Rows[0]["RESERVE1"].ToString(),
                            IsCount = dsauto.Tables[0].Rows[0]["RESERVE2"].ToString(),
                            ScanNum = dsauto.Tables[0].Rows[0]["RESERVE3"].ToString(),
                            IsLast = dsauto.Tables[0].Rows[0]["RESERVE4"].ToString(),
                        };
                        lsrecDetail.Add(recdetail);
                    }
                }
            }
            #endregion

            #region 分拣通道设定（码垛区）
            else if (msgName == "W4CS")
            {

            }
            #endregion

            #region 分拣通道设定（拆垛区）
            else if (msgName == "W4PS")
            {

            }
            #endregion

            #region 通道设定回复
            else if (msgName == "P4AS") 
            {
                msgName = "P4PS";
                byte[] bfsnd = new byte[50];
                byte[] bfstart = new byte[2];
                //16进制转byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(startStr.Substring(i * 2, 2), 16);
                    bfstart[i] = (byte)k;
                }
                byte[] bfmsgname = new byte[4];
                bfmsgname = Encoding.Default.GetBytes(msgName);
                byte[] bfmsglen = new byte[2];
                bfmsglen = IntToByte(Convert.ToInt32(msgLen));
                byte[] bfBCR = new byte[2];
                bfBCR = IntToByte(Convert.ToInt32(BCR_Nr));
                byte[] bfparid = new byte[2];
                bfparid = IntToByte(Convert.ToInt32(ParcelID));
                byte[] bftskid = new byte[2];
                bftskid = IntToByte(Convert.ToInt32(Task_ID));
                byte[] bfdes = new byte[2];
                bfdes = IntToByte(Convert.ToInt32(Destination));
                byte[] bfpsresult = new byte[2];
                bfpsresult = IntToByte(Convert.ToInt32(PasteResult));
                byte[] bfrm1 = new byte[2];
                bfrm1 = IntToByte(Convert.ToInt32(remark1));
                byte[] bfrm2 = new byte[2];
                bfrm2 = IntToByte(Convert.ToInt32(remark2));
                byte[] bfrm3 = new byte[2];
                bfrm3 = IntToByte(Convert.ToInt32(remark3));
                byte[] bfcode = new byte[24];
                bfcode = Encoding.Default.GetBytes(Barcode);
                byte[] bfend = new byte[2];
                //16进制转byte类型
                for (int i = 0; i < 2; i++)
                {
                    int k = Convert.ToInt32(endStr.Substring(i * 2, 2), 16);
                    bfend[i] = (byte)k;
                }
                for (int i = 0; i < bfstart.Length; i++)
                {
                    buffer[i] = bfstart[i];
                }
                for (int i = 0; i < bfmsgname.Length; i++)
                {
                    buffer[i + 2] = bfmsgname[i];
                }
                for (int i = 0; i < bfmsglen.Length; i++)
                {
                    buffer[i + 6] = bfmsglen[i];
                }
                for (int i = 0; i < bfBCR.Length; i++)
                {
                    buffer[i + 8] = bfBCR[i];
                }
                for (int i = 0; i < bfparid.Length; i++)
                {
                    buffer[i + 10] = bfparid[i];
                }
                for (int i = 0; i < bftskid.Length; i++)
                {
                    buffer[i + 12] = bftskid[i];
                }
                for (int i = 0; i < bfdes.Length; i++)
                {
                    buffer[i + 14] = bfdes[i];
                }
                for (int i = 0; i < bfpsresult.Length; i++)
                {
                    buffer[i + 16] = bfpsresult[i];
                }
                for (int i = 0; i < bfrm1.Length; i++)
                {
                    buffer[i + 18] = bfrm1[i];
                }
                for (int i = 0; i < bfrm2.Length; i++)
                {
                    buffer[i + 20] = bfrm2[i];
                }
                for (int i = 0; i < bfrm3.Length; i++)
                {
                    buffer[i + 22] = bfrm3[i];
                }
                for (int i = 0; i < bfcode.Length; i++)
                {
                    buffer[i + 24] = bfcode[i];
                }
                for (int i = 0; i < bfend.Length; i++)
                {
                    buffer[i + 48] = bfend[i];
                }
                //给PLC发送指令
                crConnect.Send(ByteSplit(bty, buffer));
            }
            #endregion
        }


        public bool WriteToDb27(string shelvf,int islast)
        {
            bool flag = false;
            try
            {
                int start = (Convert.ToInt32(shelvf) - 1) * 4;
                byte[] bty = new byte[4];
                byte[] tmp = new byte[1];
                tmp[0] = bty[2];
                BitArray arr = new BitArray(tmp);
                if (islast == 1)
                {
                    bty[1] = 1;
                }
                arr[0] = true;
                //将位转为byte
                int m = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (arr != null && arr[i] == true) 
                    {
                        m += Convert.ToInt32(Math.Pow(2, i));
                    }
                }
                bty[2] = (byte)m;
                if (HsPLCList.Count > 0 && HsPLCList.ContainsKey("B")) 
                {
                    flag = HsPLCList["B"].HsWrite($"DB{dbLastgo}.{start}", bty);
                }
            }
            catch (Exception ex)
            {
                flag = false;
                logWrite.WriteLog($"{shelvf}号道口分拣报告处写入DB27异常信息为{ex.Message}");
            }
            return flag;
        }

        /// <summary>
        /// 码垛区读取巷道到位信息，并进行逻辑处理
        /// </summary>
        /// <param name="Destination"></param>
        private void BoxOPC(string Destination)
        {

        }

        #region 药品批次计数，用于判断尾箱信号
        //用于对应道口的条码
        Hashtable hascount = new Hashtable();
        private bool IsLastBox(RECDETAIL recdetail,ref double hasdnum)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// byte类型转为int类型
        /// </summary>
        /// <param name="bty"></param>
        /// <returns></returns>
        public string ByteToInt(byte[] bty)
        {
            string str = (bty[0] * 256 + bty[1]).ToString();
            return str;
        }

        /// <summary>
        /// int型转换为byte数组
        /// </summary>
        /// <returns></returns>
        public byte[] IntToByte(int num)
        {
            byte[] bty = new byte[2];
            bty[0] = (byte)(num / 256);
            bty[1] = (byte)(num % 256);
            return bty;
        }

        /// <summary>
        /// 发送PLC字节重组 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="childbyte"></param>
        /// <returns></returns>
        public byte[] ByteSplit(byte[] source,byte[] childbyte)
        {
            int a = 90;
            byte[] len = new byte[2];
            len = IntToByte(a);
            Array.Copy(len, 0, source, 8, 2);
            byte[] desbyte = new byte[90];
            Array.Copy(source, desbyte, 36);
            Array.Copy(childbyte, 0, desbyte, 36, 50);
            Array.Copy(source, 286, desbyte, 86, 4);
            return desbyte;
        }

        RECDETAIL recdetail = new RECDETAIL();
        /// <summary>
        /// 用于记录收获详细信息
        /// </summary>
        List<RECDETAIL> lsrecDetail = new List<RECDETAIL>();


        /// <summary>
        /// 根据条码信息获取产品信息然后分道口
        /// </summary>
        /// <param name="code"></param>
        private void GetShelvfWay(string code)
        {
            try
            {
                recdetail = lsrecDetail.Find(rc => rc.Code == code);
                if (recdetail == null) 
                {
                    DataSet dsauto = new DataSet();
                    if (dsauto != null && dsauto.Tables[0].Rows.Count > 0)  
                    {
                        recdetail = new RECDETAIL
                        {
                            Code = code,
                            ProductID = dsauto.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                            LotInfo = dsauto.Tables[0].Rows[0]["LOTINFO"].ToString(),
                            ProductCode = dsauto.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                            PoID = dsauto.Tables[0].Rows[0]["POID"].ToString(),
                            PoDetailID = dsauto.Tables[0].Rows[0]["PODETAILID"].ToString(),
                            Num = dsauto.Tables[0].Rows[0]["NUM"].ToString(),
                            ProcudtName = dsauto.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                            WarehouseID = dsauto.Tables[0].Rows[0]["RESERVE1"].ToString(),
                            IsCount = dsauto.Tables[0].Rows[0]["RESERVE2"].ToString(),
                            ScanNum = dsauto.Tables[0].Rows[0]["RESERVE3"].ToString(),
                            IsLast = dsauto.Tables[0].Rows[0]["RESERVE4"].ToString(),
                            WAYID = dsauto.Tables[0].Rows[0]["WAYID"].ToString()
                        };
                        lsrecDetail.Add(recdetail);
                    }
                    else
                    {
                        #region 药监码联合查询的药监码信息
                        DataSet ds = new DataSet();
                        if (ds == null || ds.Tables[0].Rows.Count == 0) 
                        {
                            NotifyShowEvent?.Invoke("R", $"根据药监码{code}为获取到有关信息");
                            logWrite.WriteLog($"根据药监码{code}为获取到有关信息");
                        }
                        else
                        {
                            recdetail = new RECDETAIL
                            {
                                Code = code,
                                ProductCode = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTCODE"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                                LotInfo = Convert.IsDBNull(ds.Tables[0].Rows[0]["CMOLOTCODE"]) ? "" : ds.Tables[0].Rows[0]["CMOLOTCODE"].ToString(),
                                Num = Convert.IsDBNull(ds.Tables[0].Rows[0]["NUM"]) ? "" : ds.Tables[0].Rows[0]["NUM"].ToString(),
                                ProductID = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTID"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                                ProcudtName = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTNAME"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                                Spec = Convert.IsDBNull(ds.Tables[0].Rows[0]["SPEC"]) ? "" : ds.Tables[0].Rows[0]["SPEC"].ToString(),
                                PoID = Convert.IsDBNull(ds.Tables[0].Rows[0]["POID"]) ? "" : ds.Tables[0].Rows[0]["POID"].ToString(),
                                //EUNIT = Convert.IsDBNull(ds.Tables[0].Rows[0]["EUNIT"]) ? "" : ds.Tables[0].Rows[0]["EUNIT"].ToString(),
                                EUnit = "件",
                                ProductStatus = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTSTATUS"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTSTATUS"].ToString(),
                                ProductDate = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTDATE"]) ? "" : Convert.ToDateTime(ds.Tables[0].Rows[0]["PRODUCTDATE"]).ToString("yyyy-MM-dd"),
                                PoDetailID = Convert.IsDBNull(ds.Tables[0].Rows[0]["PODETAILID"]) ? "" : ds.Tables[0].Rows[0]["PODETAILID"].ToString(),
                                WarehouseID = Convert.IsDBNull(ds.Tables[0].Rows[0]["warehouseid"]) ? "" : ds.Tables[0].Rows[0]["warehouseid"].ToString(),
                                IsCount = Convert.IsDBNull(ds.Tables[0].Rows[0]["NUM"]) ? "" : ds.Tables[0].Rows[0]["NUM"].ToString(),
                                ScanNum = "",
                            };

                            if (string.IsNullOrEmpty(recdetail.ProductID)) 
                            {
                                NotifyShowEvent?.Invoke("R", $"根据产品编码{recdetail.ProductCode}从物品表中为获取到数据");
                                logWrite.WriteLog($"根据产品编码{recdetail.ProductCode}从物品表中未获取到数据");
                            }
                            if (string.IsNullOrEmpty(recdetail.PoID))
                            {
                                NotifyShowEvent?.Invoke("R", $"根据产品编码{recdetail.ProductCode}批次信息{recdetail.LotInfo}从通知单详细表中为获取到数据");
                                logWrite.WriteLog($"根据产品编码{recdetail.ProductCode}批次信息{recdetail.LotInfo}从通知单详细表中为获取到数据");
                            }
                        }
                        #endregion

                        #region 插入收获详细记录
                        if (recdetail != null) 
                        {

                        }
                        #endregion
                    }
                }
                if (recdetail != null) 
                {
                    //通过产品编码和批次号进行道口查询并分配
                    int wayid = AllotSHELVFWAY(ref recdetail);
                    if (wayid == 0)
                    {
                        NotifyShowEvent?.Invoke("R", "暂时无多余道口分配");
                        recdetail.WAYID = "7";
                    }
                    else if (recdetail.IsLast == "1" || Convert.ToDouble(recdetail.Num) < 1)
                    {
                        recdetail.WAYID = wayid.ToString();
                    }
                    else if (wayid == 8) 
                    {
                        recdetail.WAYID = "7";
                    }
                    else
                    {
                        recdetail.WAYID = wayid.ToString();
                        recdetail.ScanNum = recdetail.Num;
                    }
                }
                else
                {
                    recdetail = new RECDETAIL();
                    recdetail.WAYID = "7";
                }
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"条码{code}分道口异常，异常信息为:{ex.Message}");
                recdetail.WAYID = "7";
            }
        }

        /// <summary>
        /// 根据条码信息获取产品信息然后重分道口
        /// </summary>
        /// <param name="code"></param>
        /// <param name="deviceid"></param>
        private void ModifyShelvfWay(string code, string deviceid)
        {
            recdetail = lsrecDetail.Find(rc => rc.Code == code);
            if (recdetail == null) 
            {
                DataSet dsauto = new DataSet();
                if (dsauto != null && dsauto.Tables[0].Rows.Count > 0)
                {
                    recdetail = new RECDETAIL
                    {
                        Code = code,
                        ProductID = dsauto.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                        LotInfo = dsauto.Tables[0].Rows[0]["LOTINFO"].ToString(),
                        ProductCode = dsauto.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                        PoID = dsauto.Tables[0].Rows[0]["POID"].ToString(),
                        PoDetailID = dsauto.Tables[0].Rows[0]["PODETAILID"].ToString(),
                        Num = dsauto.Tables[0].Rows[0]["NUM"].ToString(),
                        ProcudtName = dsauto.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                        WarehouseID = dsauto.Tables[0].Rows[0]["RESERVE1"].ToString(),
                        ScanNum = dsauto.Tables[0].Rows[0]["RESERVE3"].ToString(),
                        IsLast = dsauto.Tables[0].Rows[0]["RESERVE4"].ToString(),
                        WAYID = dsauto.Tables[0].Rows[0]["WAYID"].ToString(),
                    };
                    lsrecDetail.Add(recdetail);
                }
            }
            if (recdetail != null) 
            {
                if (recdetail != null) 
                {
                    recdetail.WAYID = "7";
                }
                else
                {
                    //通过产品编码和批次号进行道口查询并分配
                    int wayid = ModifyAllotSHELVFWAY(recdetail, deviceid);
                    if (wayid == 0)
                    {
                        NotifyShowEvent?.Invoke("R", "暂时无多余道口分配");
                        recdetail.WAYID = "7";
                    }
                    else
                    {
                        recdetail.WAYID = wayid.ToString().Trim();
                    }
                }
            }
            else
            {
                recdetail = new RECDETAIL();
                recdetail.WAYID = "7";
            }
        }

        /// <summary>
        /// 分配道口
        /// </summary>
        /// <param name="podetailid">产品编号</param>
        /// <param name="lotinfo">批次号</param>
        /// <returns></returns>
        public int AllotSHELVFWAY(ref RECDETAIL recdetail)
        {
            int n = 0;

            return n;

        }

        /// <summary>
        /// 变更道口
        /// </summary>
        /// <param name="recdetail"></param>
        /// <param name="transportid"></param>
        /// <returns></returns>
        public int ModifyAllotSHELVFWAY(RECDETAIL recdetail,string transportid)
        {
            int n = 0;
            return n;
        }

        #region 生成收获详细记录表新逻辑
        /// <summary>
        /// 根据药监码生成一条收获详细记录表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GenRecDetail(string code,out string msg)
        {
            //用于判断是否小件  0=小件  1=正常件
            msg = "1";
            try
            {
                recdetail = lsrecDetail.Find(rc => rc.Code == code);
                if (recdetail != null) 
                {
                    return true;
                }
                DataSet dsauto = new DataSet();
                if (dsauto != null && dsauto.Tables[0].Rows.Count > 0) 
                {
                    recdetail = new RECDETAIL
                    {
                        Code = code,
                        ProductID = dsauto.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                        LotInfo = dsauto.Tables[0].Rows[0]["LOTINFO"].ToString(),
                        ProductCode = dsauto.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                        PoID = dsauto.Tables[0].Rows[0]["POID"].ToString(),
                        PoDetailID = dsauto.Tables[0].Rows[0]["PODETAILID"].ToString(),
                        Num = dsauto.Tables[0].Rows[0]["NUM"].ToString(),
                        ProcudtName = dsauto.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                        WarehouseID = dsauto.Tables[0].Rows[0]["RESERVE1"].ToString(),
                    };
                    lsrecDetail.Add(recdetail);
                    return true;
                }

                #region 药监码联合查询的药监码信息
                DataSet ds = new DataSet();
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    NotifyShowEvent?.Invoke("R", $"根据药监码{code}为获取到有关信息");
                    logWrite.WriteLog($"根据药监码{code}为获取到有关信息");
                    return false;
                }
                else
                {
                    if (ds.Tables[0].Rows[0]["reservel"].ToString() == "0") 
                    {
                        msg = "0";
                        NotifyShowEvent?.Invoke("R", $"小件剔除，药监码{code}");
                        return false;
                    }
                    recdetail = new RECDETAIL
                    {
                        Code = code,
                        ProductCode = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTCODE"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTCODE"].ToString(),
                        LotInfo = Convert.IsDBNull(ds.Tables[0].Rows[0]["CMOLOTCODE"]) ? "" : ds.Tables[0].Rows[0]["CMOLOTCODE"].ToString(),
                        Num = Convert.IsDBNull(ds.Tables[0].Rows[0]["NUM"]) ? "" : ds.Tables[0].Rows[0]["NUM"].ToString(),
                        ProductID = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTID"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTID"].ToString(),
                        ProcudtName = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTNAME"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTNAME"].ToString(),
                        Spec = Convert.IsDBNull(ds.Tables[0].Rows[0]["SPEC"]) ? "" : ds.Tables[0].Rows[0]["SPEC"].ToString(),
                        PoID = Convert.IsDBNull(ds.Tables[0].Rows[0]["POID"]) ? "" : ds.Tables[0].Rows[0]["POID"].ToString(),
                        EUnit = "件",
                        ProductStatus = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTSTATUS"]) ? "" : ds.Tables[0].Rows[0]["PRODUCTSTATUS"].ToString(),
                        ProductDate = Convert.IsDBNull(ds.Tables[0].Rows[0]["PRODUCTDATE"]) ? "" : Convert.ToDateTime(ds.Tables[0].Rows[0]["PRODUCTDATE"]).ToString("yyyy-MM-dd"),
                        PoDetailID = Convert.IsDBNull(ds.Tables[0].Rows[0]["PODETAILID"]) ? "" : ds.Tables[0].Rows[0]["PODETAILID"].ToString(),
                        WarehouseID = Convert.IsDBNull(ds.Tables[0].Rows[0]["warehouseid"]) ? "" : ds.Tables[0].Rows[0]["warehouseid"].ToString(),
                    };

                    if (recdetail.ProductID.Length == 0) 
                    {
                        NotifyShowEvent?.Invoke("R", $"根据产品编码{recdetail.ProductCode}从物品表中未获取到数据");
                        logWrite.WriteLog($"根据产品编码{recdetail.ProductCode}从物品表中未获取到数据");
                        return false;
                    }

                    if (recdetail.PoID.Length == 0) 
                    {
                        NotifyShowEvent?.Invoke("R", $"根据产品编码{recdetail.ProductCode}批次信息{recdetail.LotInfo}从通知单详细表中未获取到数据");
                        logWrite.WriteLog($"根据产品编码{recdetail.ProductCode}批次信息{recdetail.LotInfo}从通知单详细表中未获取到数据");
                        return false;
                    }
                }
                #endregion
                #region 插入收获详细记录
                return false;
                #endregion
            }
            catch (Exception ex)
            {
                NotifyShowEvent?.Invoke("R", $"根据药监码{code}生成收货记录异常，异常信息为:{ex.Message}");
                logWrite.WriteLog($"根据药监码{code}生成收货记录异常，异常信息为:{ex.Message}");
                return false;
            }
        }
        #endregion


        #region 机器拣选
        /// <summary>
        /// 给机器人发送信息，产品数量，分拣数量等信息
        /// </summary>
        private void PickRebortSend()
        {
            for (int i = 0; i < 2; i++)
            {
                int sdstart = 144 + (i * 144);
                string arrive = "0";
                if (WcsReadBoxSSJ(sdstart, ref arrive)) 
                {
                    if (arrive == "1") 
                    {
                        var deviceid1 = lsTransport.Find(tmp => tmp.DTYPE == "210" && tmp.BTID == (i + 1).ToString());
                        if (deviceid1 == null || deviceid1.TRAYCODE == "999999999")
                        {
                            continue;
                        }
                        var deviceid = lsTransport.Find(tmp => tmp.DTYPE == "208" && tmp.BTID == (i + 1).ToString());
                        if (deviceid != null) 
                        {
                            if (!WcsReadDb50(ref deviceid))
                            {
                                return;
                            }
                            DataSet ds = new DataSet();
                            if (ds != null && ds.Tables[0].Rows.Count > 0) 
                            {
                                string taskid = ds.Tables[0].Rows[0]["TASKID"].ToString();
                                DataSet dsjobdetail = null;
                                if (dsjobdetail != null && dsjobdetail.Tables[0].Rows.Count > 0) 
                                {
                                    string productcode = dsjobdetail.Tables[0].Rows[0]["PRODUCTCODE"].ToString();
                                    //如果已经包含了就跳出循环
                                    if (HashPickInfo.Contains(deviceid.BTID) && HashPickInfo[deviceid.BTID].ToString() == productcode) 
                                    {
                                        continue;
                                    }
                                    //根据产品编码查询产品码垛方式
                                    DataSet dsproinfo = null;
                                    if (dsproinfo != null && dsproinfo.Tables[0].Rows.Count > 0) 
                                    {
                                        string stackstyle = dsproinfo.Tables[0].Rows[0]["STACKTYPE"].ToString();
                                        if (string.IsNullOrEmpty(stackstyle)) 
                                        {
                                            NotifyShowEvent?.Invoke("R", $"{deviceid.BTID}号分拣机器人对应产品{productcode}为维护码垛方式，请确认");
                                            logWrite.WriteLog($"{deviceid.BTID}号分拣机器人对应产品{productcode}为维护码垛方式，请确认");
                                            continue;
                                        }
                                        int jobnum = Convert.ToInt32(dsjobdetail.Tables[0].Rows[0]["JOBNUM"]);
                                        int assignnum = Convert.ToInt32(dsjobdetail.Tables[0].Rows[0]["ASSIGNNUM"]);
                                        deviceid.Assignnum = assignnum;
                                        int surplusnum = jobnum - assignnum;
                                        int picknum = Math.Min(assignnum, surplusnum);

                                        //将产品信息写到DB块
                                        if (WriteToPickSSJ(sdstart, jobnum, picknum, stackstyle)) 
                                        {
                                            HashPickInfo.Remove(deviceid.BTID);
                                            HashPickInfo.Add(deviceid.BTID, productcode);
                                            NotifyShowEvent?.Invoke("R", $"{deviceid.BTID}号分拣机器人对应产品{productcode}写入成功");
                                            logWrite.WriteLog($"{deviceid.BTID}号分拣机器人对应产品{productcode}写入成功");
                                        }
                                        else
                                        {
                                            NotifyShowEvent?.Invoke("R", $"{deviceid.BTID}号分拣机器人对应产品{productcode}写入失败");
                                            logWrite.WriteLog($"{deviceid.BTID}号分拣机器人对应产品{productcode}写入失败");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将产品信息写入分拣线DB112块
        /// </summary>
        /// <param name="start"></param>
        /// <param name="jobnum"></param>
        /// <param name="picknum"></param>
        /// <param name="proinfo"></param>
        /// <returns></returns>
        public bool WriteToPickSSJ(int start,int jobnum,int picknum,string proinfo)
        {
            try
            {
                start = start + 2;
                byte[] buffer = new byte[6];
                int[] info = new int[6];

                byte[] bufprocode = new byte[2];
                bufprocode[0] = (byte)Convert.ToInt32(Convert.ToInt32(proinfo) / 256);
                bufprocode[1] = (byte)Convert.ToInt32(Convert.ToInt32(proinfo) % 256);
                byte[] bufjobnum = new byte[2];
                bufjobnum[0] = (byte)Convert.ToInt32(Convert.ToInt32(jobnum) / 256);
                bufjobnum[1] = (byte)Convert.ToInt32(Convert.ToInt32(jobnum) % 256);
                byte[] bufpicknum = new byte[2];
                bufpicknum[0] = (byte)Convert.ToInt32(Convert.ToInt32(picknum) / 256);
                bufpicknum[1] = (byte)Convert.ToInt32(Convert.ToInt32(picknum) % 256);
                for (int i = 0; i < 6; i++)
                {
                    info[i] = 0;
                }
                for (int i = 0; i < bufprocode.Length; i++)
                {
                    info[i] = int.Parse(bufprocode[i].ToString());
                }
                for (int i = 0; i < bufjobnum.Length; i++)
                {
                    info[i + 2] = int.Parse(bufjobnum[i].ToString());
                }
                for (int i = 0; i < bufpicknum.Length; i++)
                {
                    info[i + 4] = int.Parse(bufpicknum[i].ToString());
                }
                for (int i = 0; i < info.Length; i++)
                {
                    buffer[i] = (byte)info[i];
                }
                bool flag = false;
                if (HsPLCList.Count > 0 && HsPLCList.ContainsKey("B")) 
                {
                    flag = HsPLCList["B"].HsWrite($"DB{autoSdbnumber}.{start}", buffer);
                }
                return flag;
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"机器拣选写入产品信息异常，异常信息为{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 拆垛区OPC通信
        /// </summary>
        private void PickAutoOPC()
        {
            for (int i = 1; i <= 2 ; i++)
            {
                var devicessj = lsTransport.Find(s => s.BTID == i.ToString() && s.DTYPE == "208");
                if (devicessj != null) 
                {
                    WcsPickReadSSJ(ref devicessj);
                    if (devicessj.Isfinish == "1") 
                    {
                        int sdstart = 144 + (i - 1) * 14;
                        if (WriteToPickSSJ(sdstart, 0, 0, "0")) 
                        {
                            NotifyShowEvent?.Invoke("R", $"{devicessj.BTID}号机器人清空拣选信息成功");
                        }
                    }
                }
            }
        }


        private bool WcsPickReadSSJ(ref TransportStr si)
        {
            try
            {
                //读取拆垛输送机码垛信息DB113
                int rcvstart = 174 + (Convert.ToInt32(si.BTID) - 1) * 18;
                byte[] bty = new byte[10];
                if (HsPLCList.Count > 0 && HsPLCList.ContainsKey(si.VAR1)) 
                {
                    if (HsPLCList[si.VAR1].HsRead($"DB{autoRdbnumber}.{rcvstart}", 18, ref bty)) 
                    {
                        //将电机对应输送机id的第一个字节存储到buffer中，并获取int
                        int djint = bty[0];
                        byte[] buffer = BitConverter.GetBytes(djint);
                        BitArray arr = new BitArray(buffer);
                        //完成信号
                        si.Isfinish = Convert.ToInt32(arr[0]).ToString();
                        //当前拆垛托盘箱数
                        si.Boxnum = bty[2] * 256 + bty[3];
                        //目标托盘数量
                        si.Picknum = bty[4] * 256 + bty[5];
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logWrite.WriteLog($"读取拣选机器人完成信号异常，异常信息为{ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
