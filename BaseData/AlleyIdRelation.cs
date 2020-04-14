using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseData
{
    public static class AlleyIdRelation
    {
        /// <summary>
        /// 堆垛机映射到巷道
        /// </summary>
        /// <param name="btid"></param>
        /// <returns></returns>
        public static string GetAlleyId(string btid)
        {
            //if (1 <= btid && btid <= 10) 
            //{
            //    return "00100100" + btid;
            //}
            //return "-1";
            if (btid == "1")
            {
                return "001001001";
            }
            else if (btid == "2")
            {
                return "001001002";
            }
            else if (btid == "3")
            {
                return "001001003";
            }
            else if (btid == "4")
            {
                return "001001004";
            }
            else if (btid == "5")
            {
                return "001001005";
            }
            else if (btid == "6")
            {
                return "001001006";
            }
            else if (btid == "7")
            {
                return "001001007";
            }
            else if (btid == "8")
            {
                return "001001008";
            }
            else if (btid == "9")
            {
                return "001001009";
            }
            else if (btid == "10")
            {
                return "001001010";
            }
            else
            {
                return "-1";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alleyid"></param>
        /// <returns></returns>
        public static string GetAlleyIdByComplex(string alleyid)
        {
            //char last = alleyid.ToCharArray().Last();
            //if (int.TryParse(last.ToString(), out int id) && 1 <= id && id <= 10)
            //{
            //    return id.ToString();
            //}
            //return "-1";
            if (alleyid == "001001001")
            {
                return "1";
            }
            else if (alleyid == "001001002")
            {
                return "2";
            }
            else if (alleyid == "001001003")
            {
                return "3";
            }
            else if (alleyid == "001001004")
            {
                return "4";
            }
            else if (alleyid == "001001005")
            {
                return "5";
            }
            else if (alleyid == "001001006")
            {
                return "6";
            }
            else if (alleyid == "001001007")
            {
                return "7";
            }
            else if (alleyid == "001001008")
            {
                return "8";
            }
            else if (alleyid == "001001009")
            {
                return "9";
            }
            else if (alleyid == "001001010")
            {
                return "10";
            }
            else
            {
                return "-1";
            }
        }

        /// <summary>
        /// 道口前面输送机编号对应道口号
        /// </summary>
        /// <param name="transportid"></param>
        /// <returns></returns>
        public static string GetWayIdByTransport(string transportid)
        {
            if (transportid == "30039")
            {
                return "1";
            }
            else if (transportid == "30047")
            {
                return "2";
            }
            else if (transportid == "30054")
            {
                return "3";
            }
            else if (transportid == "30066")
            {
                return "4";
            }
            else if (transportid == "30074")
            {
                return "5";
            }
            else if (transportid == "30086")
            {
                return "6";
            }
            else
            {
                return "-1";
            }
        }

        /// <summary>
        /// 一楼出库目标巷道
        /// </summary>
        /// <param name="alleyid"></param>
        /// <param name=""></param>
        public static void GetCKOneDesSpace(string alleyid,ref CraneStr cs)
        {
            switch (alleyid)
            {
                case "1":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "2":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "3":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "4":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "5":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "6":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                case "7":
                    cs.Mbch = "1";
                    cs.Mblh = "1";
                    cs.Mbph = "2";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 二楼出库目标巷道
        /// </summary>
        /// <param name="alleyid"></param>
        /// <param name="cs"></param>
        public static void GetCKTwoDesSpace(string alleyid, ref CraneStr cs)
        {
            switch (alleyid)
            {
                case "1":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "1";
                    break;
                case "2":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "3";
                    break;
                case "3":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "5";
                    break;
                case "4":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "8";
                    break;
                case "5":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "10";
                    break;
                case "6":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "12";
                    break;
                case "7":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "14";
                    break;
                case "8":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "16";
                    break;
                case "9":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "18";
                    break;
                case "10":
                    cs.Mbch = "2";
                    cs.Mblh = "0";
                    cs.Mbph = "20";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 入库源标巷道
        /// </summary>
        /// <param name="alleyid"></param>
        /// <param name="cs"></param>
        public static void GetRKDesSpace(string alleyid, ref CraneStr cs)
        {
            switch (alleyid)
            {
                case "1":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "2":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "3":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "4":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "5":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "6":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                case "7":
                    cs.Dqch = "1";
                    cs.Dqlh = "1";
                    cs.Dqph = "1";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 小车目的地映射
        /// </summary>
        /// <param name="ssjid"></param>
        /// <returns></returns>
        public static string GetDesAddress(string ssjid)
        {
            string desaddress;
            switch (ssjid)
            {
                case "B0110":
                    desaddress = "B0190Z";
                    break;
                case "B0118":
                    desaddress = "B0210Z";
                    break;
                case "B0128":
                    desaddress = "B0230Z";
                    break;
                case "B0136":
                    desaddress = "B0250Z";
                    break;
                case "B0141":
                    desaddress = "B0260Z";
                    break;
                case "B0149":
                    desaddress = "B0280Z";
                    break;
                case "B0154":
                    desaddress = "B0153";
                    break;
                case "B0045":
                    desaddress = "B0044";
                    break;
                default:
                    desaddress = "0";
                    break;
            }
            return desaddress;
        }

        /// <summary>
        /// 根据巷道号映射入库SSJ
        /// </summary>
        /// <param name="alleyid"></param>
        /// <returns></returns>
        public static string GetInWareSSJ(string alleyid)
        {
            string ssjid;
            switch (alleyid)
            {
                case "001001001":
                    ssjid = "B0080Z";
                    break;
                case "001001002":
                    ssjid = "B0080Z";
                    break;
                case "001001003":
                    ssjid = "B0080Z";
                    break;
                case "001001004":
                    ssjid = "B0017";
                    break;
                case "001001005":
                    ssjid = "B0021";
                    break;
                case "001001006":
                    ssjid = "B0025";
                    break;
                case "001001007":
                    ssjid = "B0029";
                    break;
                case "001001008":
                    ssjid = "B0033";
                    break;
                case "001001009":
                    ssjid = "B0037";
                    break;
                case "001001010":
                    ssjid = "B0041";
                    break;
                default:
                    ssjid = "0";
                    break;
            }
            return ssjid;
        }

        /// <summary>
        /// 根据巷道号映射出库SSJ
        /// </summary>
        /// <param name="alleyid"></param>
        /// <returns></returns>
        public static string GetOutWareSSJ(string alleyid)
        {
            string ssjid = "";
            switch (alleyid)
            {
                case "001001001":
                    ssjid = "B0070Z";
                    break;
                case "001001002":
                    ssjid = "B0070Z";
                    break;
                case "001001003":
                    ssjid = "B0070Z";
                    break;
                case "001001004":
                    ssjid = "B0019";
                    break;
                case "001001005":
                    ssjid = "B0023";
                    break;
                case "001001006":
                    ssjid = "B0027";
                    break;
                case "001001007":
                    ssjid = "B0031";
                    break;
                case "001001008":
                    ssjid = "B0035";
                    break;
                case "001001009":
                    ssjid = "B0039";
                    break;
                case "001001010":
                    ssjid = "B0043";
                    break;
                default:
                    ssjid = "0";
                    break;
            }
            return ssjid;
        }

        /// <summary>
        /// 入库转弯处目的地映射
        /// </summary>
        /// <param name="alleyid"></param>
        /// <returns></returns>
        public static string GetInDesAddress(string alleyid)
        {
            string desaddress;
            switch (alleyid)
            {
                case "001001001":
                    desaddress = "2";
                    break;
                case "001001002":
                    desaddress = "4";
                    break;
                case "001001003":
                    desaddress = "6";
                    break;
                default:
                    desaddress = "0";
                    break;
            }
            return desaddress;
        }

        /// <summary>
        /// 获取入库源地址
        /// </summary>
        /// <param name="snumber"></param>
        /// <returns></returns>
        public static string GetRKSourceAddress(string snumber)
        {
            string sourceaddress = "";
            switch (snumber)
            {
                case "1":
                    sourceaddress = "A6";
                    break;
                case "2":
                    sourceaddress = "A7";
                    break;
                case "3":
                    sourceaddress = "A8";
                    break;
                case "4":
                    sourceaddress = "A9";
                    break;
            }
            return sourceaddress;
        }

        /// <summary>
        /// 获取入库目标地址
        /// </summary>
        /// <param name="snumber"></param>
        /// <returns></returns>
        public static string GetRKDesAddress(string snumber)
        {
            string desaddress = "";
            switch (snumber)
            {
                case "1":
                    desaddress = "WX1";
                    break;
                case "2":
                    desaddress = "WX2";
                    break;
                case "3":
                    desaddress = "WX3";
                    break;
                case "4":
                    desaddress = "WX4";
                    break;
            }
            return desaddress;
        }

        /// <summary>
        /// 获取出库源地址
        /// </summary>
        /// <param name="ALLEYID"></param>
        /// <returns></returns>
        public static string GetCKSourceAddress(string ALLEYID)
        {
            string sourceaddress = "";
            switch (ALLEYID)
            {
                case "001001001":
                    sourceaddress = "A6";
                    break;
                case "001001002":
                    sourceaddress = "A7";
                    break;
                case "001001003":
                    sourceaddress = "A8";
                    break;
                case "001001004":
                    sourceaddress = "A8";
                    break;
                case "001001005":
                    sourceaddress = "A8";
                    break;
                case "001001006":
                    sourceaddress = "A8";
                    break;
                case "001001007":
                    sourceaddress = "A8";
                    break;

            }
            return sourceaddress;
        }

        /// <summary>
        /// 获取出库目标地址
        /// </summary>
        /// <param name="snumber"></param>
        /// <returns></returns>
        public static string GetCKDesAddress(string snumber)
        {
            string desaddress = "";
            switch (snumber)
            {
                case "1":
                    desaddress = "WX1";
                    break;
                case "2":
                    desaddress = "WX2";
                    break;
                case "3":
                    desaddress = "WX3";
                    break;
                default:
                    desaddress = "WX4";
                    break;
            }
            return desaddress;
        }
    }
}
