using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COMRW
{
    public class TimeOutSocket
    {
        private static bool IsConnectionSuccessful = false;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public static TcpClient Connect(IPEndPoint remoteEndPoint, int timeoutMsec)
        {
            TimeoutObject.Reset();

            string serverip = remoteEndPoint.ToString();
            int serverPort = remoteEndPoint.Port;
            TcpClient tcpClient = new TcpClient();

            tcpClient.BeginConnect(serverip, serverPort, new AsyncCallback(CallBackMethod), tcpClient);

            if (TimeoutObject.WaitOne(timeoutMsec, false)) 
            {
                return IsConnectionSuccessful ? tcpClient : null;
            }
            else
            {
                tcpClient.Close();
                return null;
            }
        }

        private static void CallBackMethod(IAsyncResult ar)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpClient = ar.AsyncState as TcpClient;

                if (tcpClient.Client != null) 
                {
                    tcpClient.EndConnect(ar);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }
}
