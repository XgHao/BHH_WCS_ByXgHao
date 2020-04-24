using COMRW;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLRGVConnect
    {
        int byteLength = 256;
        public TcpClient RgvCliSocket;

        bool rg_Brun = false;
        private readonly object qLock = new object();
        private int timeOut = int.Parse(ConfigurationManager.AppSettings["TimeOut"]);
        //发送消息内容委托事件
        public delegate void NotifyCommandHandle(string command, object state);
        public event NotifyCommandHandle NotifyEvent;

        #region rgv客户端
        /// <summary>
        /// RGV连接
        /// </summary>
        /// <param name="strServer"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool RGVConnect(string strServer, string port)
        {
            try
            {
                rg_Brun = false;
                if (string.IsNullOrEmpty(strServer) || port == "0") 
                {
                    return false;
                }

                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(strServer), int.Parse(port));
                RgvCliSocket = TimeOutSocket.Connect(ipe, timeOut);

                if (RgvCliSocket != null && RgvCliSocket.Connected)
                {
                    NotifyEvent?.Invoke("RGVConnect", "连接成功");

                    rg_Brun = true;
                    Task.Run(RGVRcv);
                    return true;
                }
                else
                {
                    NotifyEvent?.Invoke("Break", "连接失败！");
                    return false;
                }
            }
            catch (SocketException ex)
            {
                NotifyEvent?.Invoke("Break", $"连接失败，异常：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取报文信息
        /// </summary>
        private void RGVRcv()
        {
            byte[] btyBuffer = new byte[byteLength];
            while (rg_Brun)
            {
                try
                {
                    if (RgvCliSocket != null)
                    {
                        if (RgvCliSocket.Client.Connected)
                        {
                            for (int i = 0; i < byteLength; i++)
                            {
                                btyBuffer[i] = 0;
                            }
                            //读取接收到的报文赋值给btyBuffer
                            int nLen = RgvCliSocket.Client.Receive(btyBuffer);
                            if (nLen > 0)
                            {
                                NotifyEvent?.Invoke("RecvData", btyBuffer);
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (RgvCliSocket != null)
                    {
                        Close(RgvCliSocket, rg_Brun);
                    }
                    NotifyEvent?.Invoke("Break", $"RGV连接中断！{ex.Message}");
                }
            }
        }
        #endregion

        #region 关闭通信与获取消息
        /// <summary>
        /// 关闭通信
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="flag"></param>
        public void Close(TcpClient socket, bool flag)
        {
            try
            {
                flag = false;
                socket?.Client.Shutdown(SocketShutdown.Both);
                socket?.Client.Close();

                GC.Collect();
            }
            catch (Exception ex)
            {
                NotifyEvent?.Invoke("Break", $"释放异常!{ex.Message}");
            }
        }
        #endregion
    }
}
