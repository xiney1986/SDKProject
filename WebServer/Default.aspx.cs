using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebServer
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string channelid = Request.QueryString["channelid"];
            string channeluserid = Request.QueryString["channeluserid"];
            string token = Request.QueryString["token"];
            string productcode = Request.QueryString["productcode"];
            string param = "sdk=" + channelid + "&app=" + productcode + "&uin=" + channeluserid + "&sess=" + token;
            //Response.Write("http://sync.1sdk.cn/login/check.html?" + param);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://sync.1sdk.cn/login/check.html?" + param);
            var response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseJson = reader.ReadToEnd();
                Response.Write(responseJson);
            }
        }
    }
}