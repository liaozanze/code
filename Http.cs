using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lhn
{
    public class Http
    {
        public string WebHttp(HttpParam hp)
        {
            string rt = "";
            HttpWebResponse myResponse = null;
            StreamReader reader = null;
            CookieContainer myCookieContainer = new CookieContainer();
            try
            {
                // Prepare web request...
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(hp.Url);
                //method
                if (string.IsNullOrEmpty(hp.Method) || hp.Method.ToUpper() == "GET")
                    hp.Method = "GET";
                else
                    hp.Method = "POST";
                request.Method = hp.Method;
                Encoding encodeing;
                if (string.IsNullOrEmpty(hp.Encoding) || hp.Encoding.ToUpper() == "UTF8" || hp.Encoding.ToUpper() == "UTF-8")
                    encodeing = Encoding.UTF8;
                else
                    encodeing = Encoding.Default;
                //ua
                if (!string.IsNullOrEmpty(hp.UserAgent))
                {
                    string[] arrUa = hp.UserAgent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ua in arrUa)
                    {
                        if (ua.Contains("Content-Type"))
                            request.ContentType = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("User-Agent"))
                            request.UserAgent = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("Referer"))
                            request.Referer = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("Accept"))
                            request.Accept = ua.Substring(ua.IndexOf(":") + 1);
                    }
                }
                if (string.IsNullOrEmpty(request.ContentType))
                    request.ContentType = "application/x-www-form-urlencoded;";
                if (string.IsNullOrEmpty(request.UserAgent))
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                if (string.IsNullOrEmpty(request.Referer))
                    request.Referer = request.RequestUri.ToString();
                //cookie
                if (hp.Cookie == null)
                    hp.Cookie = "";
                myCookieContainer = this.AddCookieToContainer(hp.Cookie, myCookieContainer, request.RequestUri.Host);
                request.CookieContainer = myCookieContainer;
                //data
                byte[] data = { };
                if (hp.Method == "POST")
                {
                    if (hp.PostDataType1 == HttpParam.PostDataType.byte1)
                        data = hp.PostDataByte;
                    else
                        data = encodeing.GetBytes(hp.PostData);
                }
                request.ContentLength = data.Length;

                if (request.Method == "POST")
                {
                    Stream newStream = request.GetRequestStream();
                    // Send the data.
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                //redirect
                if (hp.Disredirect == true)
                    request.AllowAutoRedirect = false;
                // Get response
                myResponse = (HttpWebResponse)request.GetResponse();
                //GetCookie
                myResponse.Cookies = myCookieContainer.GetCookies(request.RequestUri);
                hp.Cookie = "";
                foreach (Cookie item in myResponse.Cookies)
                {
                    hp.Cookie += item.Name + "=" + item.Value + ";";
                }
                //GetHtml
                reader = new StreamReader(myResponse.GetResponseStream(), encodeing);
                rt = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (myResponse != null)
                    myResponse.Close();
            }
            return rt;
        }
        public CookieContainer AddCookieToContainer(string cookie, CookieContainer cc, string domain)
        {
            if (cookie.Contains("Cookie:"))
                cookie = GetRight(cookie, "Cookie:").Trim();
            string[] tempCookies = cookie.Split(';');
            string tempCookie = null;
            int Equallength = 0;//  =的位置 
            string cookieKey = null;
            string cookieValue = null;
            //qg.gome.com.cn  cookie 
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];

                    Equallength = tempCookie.IndexOf("=");

                    if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=; 
                    {

                        cookieKey = tempCookie.Substring(0, Equallength).Trim();
                        //cookie=

                        if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=; 
                        {
                            cookieValue = "";
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                        }
                    }

                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = "";
                    }

                    cc.Add(new Cookie(cookieKey, cookieValue, "", domain));

                }

            }

            return cc;
        }
        public byte[] WebHttp(HttpParam hp, bool returnByte)
        {
            MemoryStream ms = new MemoryStream();
            HttpWebResponse response = null;
            Stream stream = null;
            CookieContainer myCookieContainer = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(hp.Url);

            try
            {
                if (string.IsNullOrEmpty(hp.Method) || hp.Method.ToUpper() == "GET")
                    hp.Method = "GET";
                else
                    hp.Method = "POST";
                request.Method = hp.Method;
                Encoding encodeing;
                if (string.IsNullOrEmpty(hp.Encoding) || hp.Encoding.ToUpper() == "UTF8" || hp.Encoding.ToUpper() == "UTF-8")
                    encodeing = Encoding.UTF8;
                else
                    encodeing = Encoding.Default;
                //ua
                if (!string.IsNullOrEmpty(hp.UserAgent))
                {
                    string[] arrUa = hp.UserAgent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ua in arrUa)
                    {
                        if (ua.Contains("Content-Type"))
                            request.ContentType = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("User-Agent"))
                            request.UserAgent = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("Referer"))
                            request.Referer = ua.Substring(ua.IndexOf(":") + 1);
                        else if (ua.Contains("Accept"))
                            request.Accept = ua.Substring(ua.IndexOf(":") + 1);
                    }
                }
                if (string.IsNullOrEmpty(request.ContentType))
                    request.ContentType = "application/x-www-form-urlencoded;";
                if (string.IsNullOrEmpty(request.UserAgent))
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                if (string.IsNullOrEmpty(request.Referer))
                    request.Referer = request.RequestUri.ToString();
                //cookie
                if (hp.Cookie == null)
                    hp.Cookie = "";
                myCookieContainer = this.AddCookieToContainer(hp.Cookie, myCookieContainer, request.RequestUri.Host);
                request.CookieContainer = myCookieContainer;
                //data
                byte[] data = { };
                if (hp.Method == "POST")
                {
                    if (hp.PostDataType1 == HttpParam.PostDataType.byte1)
                        data = hp.PostDataByte;
                    else
                        data = encodeing.GetBytes(hp.PostData);
                }
                request.ContentLength = data.Length;

                if (request.Method == "POST")
                {
                    Stream newStream = request.GetRequestStream();
                    // Send the data.
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                //redirect
                if (hp.Disredirect == true)
                    request.AllowAutoRedirect = false;

                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                stream.CopyTo(ms, 1024);

                //GetCookie
                response.Cookies = myCookieContainer.GetCookies(request.RequestUri);
                hp.Cookie = "";
                foreach (Cookie item in response.Cookies)
                {
                    hp.Cookie += item.Name + "=" + item.Value + ";";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (stream != null)
                    stream.Close();
            }
            return ms.ToArray();
        }
        public static string GetRight(string str0, string str1, params int[] skipNum)
        {
            try
            {
                string strTemp = "";
                int index = -1;
                index = str0.IndexOf(str1);
                if (skipNum.Length > 0)//长度为0说明此参数省略
                {
                    if (skipNum[0] > 0)
                    {
                        for (int i = 0; i < skipNum[0]; i++)
                        {
                            index = str0.IndexOf(str1, index + 1);
                            if (index == -1)
                            {
                                return "";
                            }
                        }
                    }
                }
                if (index != -1)
                {
                    strTemp = str0.Substring(index + str1.Length, str0.Length - index - str1.Length);
                    return strTemp;
                }
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

    }
    public class HttpParam
    {
        public enum PostDataType
        {
            str1,
            byte1
        }
        string url, method, postData, cookie, userAgent, encoding;
        bool disredirect;
        byte[] postDataByte;
        PostDataType postDataType;
        public string Url { get => url; set => url = value; }
        public string Method { get => method; set => method = value; }
        public string PostData { get => postData; set => postData = value; }
        public string Cookie { get => cookie; set => cookie = value; }
        public string UserAgent { get => userAgent; set => userAgent = value; }
        public bool Disredirect { get => disredirect; set => disredirect = value; }
        public string Encoding { get => encoding; set => encoding = value; }
        public byte[] PostDataByte { get => postDataByte; set => postDataByte = value; }
        public PostDataType PostDataType1 { get => postDataType; set => postDataType = value; }
    }
}
