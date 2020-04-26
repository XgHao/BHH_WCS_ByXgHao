using Bap.LibNodave;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLSRMControl
    {
        private readonly TcpClient cliSocket;
        private const int byteLen = 256;
        bool g_brun = false;
        Task ComTask;
        private readonly object qLock = new object();
        private readonly int timeOut = int.Parse(ConfigurationManager.AppSettings["TimeOut"]);

        public delegate void NotifyCommandHandle(string command, object state);
        /// <summary>
        /// 发送消息内容委托事件
        /// </summary>
        public event NotifyCommandHandle NotifyEvent;

        /// <summary>
        /// 取消标记
        /// </summary>
        private CancellationTokenSource CancelTS = new CancellationTokenSource();

        public bool Connect(string strServer, string port)
        {
            try
            {
                if (string.IsNullOrEmpty(strServer) || port == "0") 
                {
                    return false;
                }
                PlcTcpClient plink = new PlcTcpClient(strServer, int.Parse(port));
                int n = plink.ConnectPLC();

                if (n == 0)
                {
                    NotifyEvent?.Invoke("Connect", "连接成功！");
                    g_brun = true;
                    ComTask = Task.Run(Rcv, CancelTS.Token);
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
        /// 发送报文信息
        /// </summary>
        /// <param name="btyData"></param>
        /// <returns></returns>
        public bool Send(byte[] btyData)
        {
            try
            {
                lock (qLock)
                {
                    if (cliSocket.Client.Connected)
                    {
                        cliSocket.Client.Send(btyData);
                        return true;
                    }
                    else
                    {
                        Close();
                        NotifyEvent?.Invoke("Break", "SendData Fail！");
                        return false;
                    }
                }
            }
            catch (SocketException ex)
            {
                Close();
                NotifyEvent?.Invoke("Break", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭通信
        /// </summary>
        public void Close()
        {
            g_brun = false;
            if (ComTask != null) 
            {
                if (ComTask.Status == TaskStatus.Running) 
                {
                    CancelTS.Cancel();
                }
                Thread.Sleep(50);
            }

            if (cliSocket != null) 
            {
                cliSocket.Client.Shutdown(SocketShutdown.Both);

                cliSocket.Client.Close();
            }
            GC.Collect();
        }

        /// <summary>
        /// 获取报文信息
        /// </summary>
        private void Rcv()
        {
            byte[] byteBuffer = new byte[byteLen];
            while (g_brun) 
            {
                try
                {
                    if (cliSocket != null && cliSocket.Client.Connected)  
                    {
                        for (int i = 0; i < byteLen; i++)
                        {
                            byteBuffer[i] = 0;
                        }
                        //读取接收到的报文赋值给bytebuffer
                        int nLen = cliSocket.Client.Receive(byteBuffer);
                        if (nLen > 0)
                        {
                            NotifyEvent?.Invoke("RecvData", byteBuffer);
                        }
                        else
                        {
                            Close();
                            NotifyEvent?.Invoke("Break", "接收到的报文长度为0！");
                            break;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Close();
                    NotifyEvent?.Invoke("Break", $"连接中断！{ex.Message}");
                }
            }
        }
    }
}
