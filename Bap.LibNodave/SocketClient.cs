using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    class TimeOutSocket
    {
        private bool IsConnectionSuccessful = false;
        private Exception socketException;
        /// <summary>
        /// 通知一个或多个正在等待的线程已发生事件
        /// true 若要设置的初始状态信号; false 初始状态设置为终止状态。
        /// </summary>
        private ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public Socket Connect(IPEndPoint remoteEndPoint,int timeoutMsec)
        {
            //将事件状态设置为非终止，从而导致线程受阻
            TimeoutObject.Reset();
            socketException = null;

            string serverIP = remoteEndPoint.Address.ToString();
            int serverPort = remoteEndPoint.Port;

            Socket tcpclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = remoteEndPoint;
            tcpclient.BeginConnect(remoteEndPoint, new AsyncCallback(CallBackMethod), tcpclient);

            //阻止当前线程，直到当前的WaitHandle收到信号为止，
            //millisecondsTimeout:等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待。
            //exitContext:如果等待之前先退出上下文的同步域（如果在同步上下文中），并在稍后重新获取它，则为 true；否则为 false。
            if (TimeoutObject.WaitOne(timeoutMsec, false)) 
            {
                return IsConnectionSuccessful ? tcpclient : null;
            }
            else
            {
                tcpclient.Close();
                return null;
            }
        }

        /// <summary>
        /// 【回调函数】Socket连接操作完成时
        /// </summary>
        /// <param name="asyncResult"></param>
        private void CallBackMethod(IAsyncResult asyncResult)
        {
            try
            {
                IsConnectionSuccessful = false;
                Socket Client = asyncResult.AsyncState as Socket;

                if (Client != null) 
                {
                    Client.EndConnect(asyncResult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception e)
            {
                IsConnectionSuccessful = false;
                socketException = e;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }
}
