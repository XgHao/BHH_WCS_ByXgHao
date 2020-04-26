using COMRW;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLRGVControl
    {
        public TcpClient RgvCliSocket;
        public TcpClient WMSCliSocket;
        public TcpClient HeartCliSocket;
        public TcpClient GetLocationSocket;

        private const int byteLen = 256;
        bool g_brun = false;
        bool wg_brun = false;
        bool rg_brun = false;
        Task ComTask;
        readonly CancellationTokenSource TokenSource_ComTask = new CancellationTokenSource();
        Task RGVComTask;
        readonly CancellationTokenSource TokenSource_RGVComTask = new CancellationTokenSource();
        Task LocationTask;

        private readonly object qLock = new object();
        private int timeOut = int.Parse(ConfigurationManager.AppSettings["TimeOut"]);
        private int heartTimeOut = int.Parse(ConfigurationManager.AppSettings["HeartTimeOut"]);

        public delegate void NotifyCommandHandle(string command, object state);
        /// <summary>
        /// 发送消息内容委托事件
        /// </summary>
        public event NotifyCommandHandle NotifyEvent;

        public delegate void WCSResponse();
        public event WCSResponse WCSRpEvent;

        /// <summary>
        /// 关闭通信
        /// </summary>
        /// <param name="socket"></param>
        public void Close(TcpClient socket)
        {
            try
            {
                if (ComTask != null) 
                {
                    if (ComTask.Status == TaskStatus.Running)
                    {
                        TokenSource_ComTask.Cancel();
                    }
                    Thread.Sleep(50);
                }

                if (socket != null)
                {
                    socket.Client.Shutdown(SocketShutdown.Both);
                    socket.Client.Close();
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                NotifyEvent?.Invoke("Break", $"释放异常！{ex.Message}");
            }
        }

        /// <summary>
        /// rgv连接
        /// </summary>
        /// <param name="strServer"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool RGVConnect(string strServer, string port)
        {
            try
            {
                rg_brun = false;
                if (string.IsNullOrEmpty(strServer) || port == "0") 
                {
                    return false;
                }
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(strServer), int.Parse(port));
                RgvCliSocket = TimeOutSocket.Connect(ipe, timeOut);

                if (RgvCliSocket != null)
                {
                    NotifyEvent?.Invoke("RGVConnect", "连接成功！");

                    rg_brun = true;
                    RGVComTask = new Task(Rcv, TokenSource_RGVComTask.Token);
                    return true;
                }
                else
                {
                    NotifyEvent?.Invoke("Break", "连接失败!");
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
        /// WMS连接
        /// </summary>
        /// <param name="strServer"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool WMSConnect(string strServer, string port)
        {
            try
            {
                wg_brun = false;
                if (string.IsNullOrEmpty(strServer) || port == "0")
                {
                    return false;
                }

                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(strServer), int.Parse(port));
                WMSCliSocket = TimeOutSocket.Connect(ipe, timeOut);

                if (WMSCliSocket != null)
                {
                    NotifyEvent?.Invoke("WMSConnect", "连接成功");

                    wg_brun = true;
                    ComTask = new Task(RGVRcv, TokenSource_ComTask.Token);

                    return true;
                }
                else
                {
                    NotifyEvent?.Invoke("Break", "连接失败!");
                    return false;
                }
            }
            catch (SocketException ex)
            {
                NotifyEvent?.Invoke("Break", $"连接失败，异常：{ex.Message}");
                return false;
            }
        }


        private void RGVRcv()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取报文信息
        /// </summary>
        private void Rcv()
        {
            byte[] bytBuffer = new byte[byteLen];
            while (!TokenSource_RGVComTask.IsCancellationRequested) 
            {
                try
                {
                    if (RgvCliSocket != null && RgvCliSocket.Client.Connected)
                    {
                        for (int i = 0; i < byteLen; i++)
                        {
                            bytBuffer[i] = 0;
                        }
                        //读取接收到的报文赋值给bytBuffer
                        int nLen = RgvCliSocket.Client.Receive(bytBuffer);
                        if (nLen > 0)
                        {
                            NotifyEvent?.Invoke("RecvData", bytBuffer);
                            if (RgvCliSocket != null)
                            {
                                Close(RgvCliSocket);
                            }
                        }
                        else
                        {
                            Close(RgvCliSocket);
                            NotifyEvent?.Invoke("Break", "接收到的报文长度为0！");
                            break;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (RgvCliSocket != null)
                    {
                        Close(RgvCliSocket);
                    }
                    NotifyEvent?.Invoke("Break", $"连接中断!{ex.Message}");
                }
            }
        }
    }
}
