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

        public Timer CheckTime
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
        }

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

    }
}
