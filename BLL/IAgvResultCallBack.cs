using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 指示接口或类在Windows Communication Foundation(WCF)应用程序中定义服务协定
    /// </summary>
    [ServiceContract(Name = " ")]
    public interface IAgvResultCallBack
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agvCallBack"></param>
        /// <returns></returns>
        [OperationContract] //提示方法定义一个操作，该操作时WCF应用程序中服务协定的一部分
        [WebInvoke(Method = "POST", UriTemplate = "Agv/AgvCallBackService/agvCallback", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]   //表示一个属性，指示服务操作在逻辑上是调用操作，可以调用WCF REST变成模型
        SendToAgvResult CallBackResult(ReciveAgeCallBack agvCallBack);
    }
}
