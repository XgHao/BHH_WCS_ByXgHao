using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LOG;
using System.Threading.Tasks;
using DataOperate;
using COMRW;

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
            }
        }
    }
}
