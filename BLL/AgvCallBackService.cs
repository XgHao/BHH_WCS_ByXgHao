using DataOperate;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    //指定服务协定实现的内部执行行为
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    //应用于WCF服务以指示该服务能否在ASP.NET兼容模式下运行
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AgvCallBackService : BLLAgvBase, IAgvResultCallBack
    {
        public SendToAgvResult CallBackResult(ReciveAgeCallBack agvCallBack)
        {
            DataSet ds = new DataSet();
            ds = DataTrans.D_GetAgvCallBack(agvCallBack.ReqCode, out string msg);
            SendToAgvResult result = new SendToAgvResult();
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    result.Code = "1";
                    result.Message = "请求编号已存在";
                    result.Data = "";
                    result.ReqCode = agvCallBack.ReqCode;
                }
                else
                {
                    string msgInfo = bll.HandleCallBackData(agvCallBack);
                    if (msgInfo == "OK") 
                    {
                        int n = DataTrans.D_InsertCallBack(agvCallBack, "2", out msg);
                        if (n > 0)
                        {
                            result.Code = "0";
                            result.Message = "成功";
                            result.Data = string.Empty;
                            result.ReqCode = agvCallBack.ReqCode;
                        }
                    }
                    else
                    {
                        result.Code = "1";
                        result.Message = "失败";
                        result.Data = string.Empty;
                        result.ReqCode = agvCallBack.ReqCode;
                    }
                }
            }
            else
            {
                result.Code = "1";
                result.Message = "数据处理异常";
                result.Data = string.Empty;
                result.ReqCode = agvCallBack.ReqCode;
            }
            return result;
        }
    }
}
