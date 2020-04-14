using LOG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LedDisplay
{
    /// <summary>
    /// 电子显示屏服务端
    /// </summary>
    public class LedServer
    {
        public delegate void RcvMsgDg(string df);
        public event RcvMsgDg RcvMsgEvent;
        private readonly object qLock = new object();
        static string Ip = ConfigurationManager.AppSettings["IP"].ToString();
        static int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
        static IPAddress ip = IPAddress.Parse(Ip);

        IPEndPoint endPoint = new IPEndPoint(ip, port);
        Socket serverSck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Log log = new Log("通讯日志", @".\通讯日志\");
        List<Socket> ls = new List<Socket>();

        public void ServerSck()
        {
            try
            {
                serverSck.Bind(endPoint);
                serverSck.Listen(1000);
                //获取客户端连接
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Socket tSocket = serverSck.Accept();
                            if (tSocket.Connected)
                            {
                                string point = tSocket.RemoteEndPoint.ToString();

                                log.WriteLog($"{point}连接成功");
                                ls.Add(tSocket);
                                //获取消息
                                Task.Factory.StartNew(obj =>
                                {
                                    Socket client = obj as Socket;
                                    while (true)
                                    {
                                        try
                                        {
                                            byte[] buffer = new byte[1024];
                                            int n = client.Receive(buffer);
                                            if (n == 0) 
                                            {
                                                break;
                                            }
                                            string str = Encoding.Default.GetString(buffer, 0, n);
                                            RcvMsgEvent?.Invoke(str);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.WriteLog(ex.Message);
                                            break;
                                        }
                                    }
                                }, tSocket);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.WriteLog($"客户端与服务端连接异常，异常信息为{ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                log.WriteLog($"通信异常，异常信息为{ex.Message}");
            }
        }


        public void SendStr(string str)
        {
            try
            {
                foreach (var client in ls)
                {
                    if (client != null) 
                    {
                        lock (qLock)
                        {
                            if (client.Connected)
                            {
                                byte[] buffer = Encoding.UTF8.GetBytes(str);
                                client.Send(buffer);
                            }
                            else
                            {
                                ls.Remove(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLog($"给客户端发送消息异常，异常信息为{ex.Message}");
            }
        }
    }
}
