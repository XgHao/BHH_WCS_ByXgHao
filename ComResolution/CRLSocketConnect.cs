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
    public class CRLSocketConnect
    {
        //用来传输命令和获取到的对象
        public delegate void NotifyCommandDG(string command, object state);
        public event NotifyCommandDG NotifyCommandEvent;

        private const int byteLen = 290;

        private TcpClient tcpClient;
        private Task comTask;
        private bool g_brun;
        private static readonly object qLock = new object();
        private int TimeOut = int.Parse(ConfigurationManager.AppSettings["TimeOut"].ToString());
        #region 服务端IP端口
        private static readonly string ipp= ConfigurationManager.AppSettings["AIP"].ToString();
        private static readonly int port = Convert.ToInt32(ConfigurationManager.AppSettings["APort"].ToString());
        private static readonly IPAddress ip = IPAddress.Parse(ipp);

        IPEndPoint endPoint = new IPEndPoint(ip, port);
        Socket serverSck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> Sockets = new List<Socket>();
        #endregion

        /// <summary>
        /// 判断通信是否还连接
        /// </summary>
        public bool Connetced
        {
            get
            {
                if (tcpClient != null) 
                {
                    return tcpClient.Connected;
                }
                return false;
            }
        }

        Socket tSocket;
        public CRLSocketConnect()
        { 
            g_brun = false;
        }

        /// <summary>
        /// 绑定地址监听客户端
        /// </summary>
        public void ServerSck()
        {
            try
            {
                serverSck.Bind(endPoint);
                serverSck.Listen(100);
                //获取客户端连接
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            tSocket = serverSck.Accept();
                            if (tSocket.Connected)
                            {
                                string point = tSocket.RemoteEndPoint.ToString();
                                NotifyCommandEvent?.Invoke("Connect", $"{point}连接成功");
                                Task.Factory.StartNew(Recv, tSocket);
                                Thread.Sleep(500);
                            }
                        }
                        catch (Exception ex)
                        {
                            NotifyCommandEvent?.Invoke("Break", $"客户端与服务端连接异常{ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                NotifyCommandEvent?.Invoke("Break", $"通信异常信息为{ex.Message}");
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        public bool Send(byte[] bytData)
        {
            bool flag = false;
            try
            {
                if (tSocket != null) 
                {
                    lock (qLock)
                    {
                        if (tSocket.Connected)
                        {
                            int n = tSocket.Send(bytData);
                            flag = true;
                        }
                        else
                        {
                            NotifyCommandEvent?.Invoke("Break", "SendData fail");
                            flag = false;
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                NotifyCommandEvent?.Invoke("Break", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="state"></param>
        private void Recv(object state)
        {
            Socket client = state as Socket;
            byte[] bytebuffer = new byte[byteLen];
            lock (qLock)
            {
                while (true)
                {
                    try
                    {
                        if (tSocket != null) 
                        {
                            if (tSocket.Connected)
                            {
                                for (int i = 0; i < 50; i++)
                                {
                                    bytebuffer[i] = 0;
                                }
                                int nLen = tSocket.Receive(bytebuffer);
                                if (nLen > 0) 
                                {
                                    NotifyCommandEvent?.Invoke("RecvData", bytebuffer);
                                }
                                else
                                {
                                    NotifyCommandEvent?.Invoke("Break", "接受报文长度为0");
                                }
                            }
                        }
                        else
                        {
                            NotifyCommandEvent?.Invoke("Break", "与PLC断开连接");
                        }
                    }
                    catch (Exception ex)
                    {
                        NotifyCommandEvent?.Invoke("Break", $"连接断开{ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// 关闭线程与通信
        /// </summary>
        public void Close()
        {
            g_brun = false;

            if (comTask != null) 
            {
                if (comTask.Status == TaskStatus.Running) 
                {
                }
            }

            if (tcpClient != null) 
            {
                if (Connetced)
                {
                    tcpClient.Client.Shutdown(SocketShutdown.Both);
                }
                tcpClient.Client.Close();
            }
            GC.Collect();
        }
    }
}
