using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using login1.Models;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace login1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult logIn(logIn userPass)
        {
            string authCode = "";
            string MyUrl = Request.Url.AbsoluteUri;
            if (MyUrl.IndexOf("code=") > -1)
            {
                authCode = MyUrl.Substring(MyUrl.IndexOf("code=") + 5, 36);
                if (authCode != "")
                {
                    string strToken = getToken(authCode);
                    if (strToken != null)
                    {
                        string strUserInfor = getUserInfor(strToken);
                        if (strUserInfor != null)
                        {
                            UserInfor userInforBinding = new UserInfor();
                            ViewData["Contact"] = "Your application description page." + strUserInfor;
                            return View("Contact");
                        }
                    }
                }

            }

            if (userPass.userName == "admin" && userPass.passWord == "admin")
            {
                return View("Contact");
            }
            return View("Index");
        }

        public ActionResult Login_byDVCQG()
        {
            Response.Write("<script>alert('login successful');</script>");
            return Redirect("https://wso2.vnpttiengiang.vn:9443/oauth2/authorize?response_type=code&client_id=pY2cdZRLrMQ29dj5r4m92VfQhEka&scope=openid&redirect_uri=https://localhost:44379");
        }
        public ActionResult logOut()
        {
            // call api logout
            return View("Index");
        }

        static string getToken(string authCode)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://wso2.vnpttiengiang.vn:9443/oauth2/token");

            var postData = "grant_type=" + Uri.EscapeDataString("authorization_code");
            postData += "&code=" + Uri.EscapeDataString(authCode);
            postData += "&redirect_uri=" + Uri.EscapeDataString("https://localhost:44379");
            postData += "&client_id=" + Uri.EscapeDataString("pY2cdZRLrMQ29dj5r4m92VfQhEka");
            postData += "&client_secret=" + Uri.EscapeDataString("mQfiWJuMZyvphFvZcn3a6V5Pd0oa");

            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Console.WriteLine(response + responseString);

            if (responseString != null && responseString.Length > 0)
            {
                JObject jsonResponse = JObject.Parse(responseString);
                string strAccessToken = (string)jsonResponse.SelectToken("access_token");
                return strAccessToken;
            }
            return null;

        }

        static string getUserInfor(string accessToken)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://wso2.vnpttiengiang.vn:9443/oauth2/userinfo");

            var postData = "scope=" + Uri.EscapeDataString("openid");
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (responseString != null && responseString.Length > 0)
            {
                Console.WriteLine(response + responseString);
                return responseString;
            }
            return null;

        }

    }
}