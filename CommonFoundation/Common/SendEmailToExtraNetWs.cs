using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonFoundation.Common
{

    [System.Diagnostics.DebuggerStepThrough(), System.ComponentModel.DesignerCategory("code"), System.Web.Services.WebServiceBinding(Name = "", Namespace = "")]
    public class SendEmailToExtraNetWs : SendEmailToExtraNet
    {
        public SendEmailToExtraNetWs()
            : base()
        {
            //设置默认webService的地址
            this.Url = "http://192.168.100.179/SendEmail/SendEmailToExtraNet.asmx";
        }

        public SendEmailToExtraNetWs(string webUrl)
            : base()
        {
            this.Url = webUrl;
        }
    }
}