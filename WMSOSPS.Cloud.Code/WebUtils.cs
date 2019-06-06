using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace WMSOSPS.Cloud.Code
{
    public static class WebUtils
    {
        #region Session

        /// <summary>
        /// 得到Session
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>string</returns>
        public static string GetSessionReturnString(string key)
        {
            var obj = HttpContext.Current.Session[key];
            return obj == null ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 得到Session
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>object</returns>
        public static object GetSessionReturnObject(string key, object defaultValue = null)
        {
            return HttpContext.Current.Session[key] ?? defaultValue;
        }

        /// <summary>
        /// 写Session
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        /// <returns>bool</returns>
        public static void SetSession(string key, object value)
        {
            HttpContext.Current.Session.Remove(key);
            HttpContext.Current.Session.Add(key, value);
        }

        /// <summary>
        /// 清空所有的Session
        /// </summary>
        /// <returns></returns>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
        }

        /// <summary>
        /// 删除一个指定的Session
        /// </summary>
        /// <param name="name">Session名称</param>
        /// <returns></returns>
        public static void RemoveSession(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }

        /// <summary>
        /// 设置Session的过期时间
        /// </summary>
        /// <param name="iExpires">调动有效期（分钟）</param>
        public static void SetTimeout(int iExpires)
        {
            HttpContext.Current.Session.Timeout = iExpires;
        }

        #endregion Session

        #region URL、网站目录

        public static string GetCharSet()
        {
            return HttpContext.Current.Request.ContentEncoding.BodyName;
        }

        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIp()
        {
            var result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }

            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(result) || !Regex.IsMatch(result, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                return "127.0.0.1";
            }

            return result;
        }

        /// <summary>
        /// 将键值对转换成URL参数
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null)
            {
                return string.Empty;
            }
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return string.Join("&", array);
        }

        /// <summary>
        /// 获得Email的域名
        /// </summary>
        /// <param name="strEmail">Email</param>
        /// <returns></returns>
        /// <remarks>冯瑞 2011-11-02 17:35:17</remarks>
        public static string GetEmailHostName(string strEmail)
        {
            return strEmail.IndexOf("@", StringComparison.Ordinal) < 0 ?
                string.Empty :
                strEmail.Substring(strEmail.LastIndexOf("@", StringComparison.Ordinal)).ToLower();
        }

        /// <summary>
        /// 获取域名
        /// </summary>
        /// <param name="strHtmlPagePath"></param>
        /// <returns></returns>
        public static string GetUrlDomainName(string strHtmlPagePath)
        {
            const string p = @"http://[^\.]*\.(?<domain>[^/]*)";
            var reg = new Regex(p, RegexOptions.IgnoreCase);
            var m = reg.Match(strHtmlPagePath);
            return m.Groups["domain"].Value;
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="path">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string path)
        {
            var strPath = path;
            if (path.ToLower().StartsWith("http://") || path.ToLower().StartsWith("https://"))
            {
                return path;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            //非web程序引用

            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }




        /// <summary>
        /// 得到Web的真实路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebPath()
        {
            var webPath = HttpContext.Current.Request.Path;
            if (webPath.LastIndexOf("/", StringComparison.Ordinal) !=
                webPath.IndexOf("/", StringComparison.Ordinal))
            {
                webPath = webPath.Substring(webPath.IndexOf("/", StringComparison.Ordinal),
                                            webPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
            }
            else
            {
                webPath = "/";
            }

            return webPath;
        }

        /// <summary>
        /// 返回URL中结尾的文件名
        /// </summary>
        public static string GetFilename(string url)
        {
            if (url == null)
            {
                return string.Empty;
            }
            var strs1 = url.Split('/');
            return strs1[strs1.Length - 1].Split('?')[0];
        }

        /// <summary>
        /// 得到网站的真实路径
        /// </summary>
        /// <returns></returns>
        public static string GetTrueSitePath()
        {
            var sitePath = HttpContext.Current.Request.Path;
            if (sitePath.LastIndexOf("/", StringComparison.Ordinal) !=
                sitePath.IndexOf("/", StringComparison.Ordinal))
            {
                sitePath = sitePath.Substring(sitePath.IndexOf("/", StringComparison.Ordinal),
                                              sitePath.LastIndexOf("/", StringComparison.Ordinal) + 1);
            }
            else
            {
                sitePath = "/";
            }
            return sitePath;
        }

        /// <summary>
        /// 取得网站的根目录的URL
        /// </summary>
        /// <returns></returns>
        public static string GetRootUri()
        {
            var httpCurrent = HttpContext.Current;
            return httpCurrent == null ? string.Empty : GetRootUri(httpCurrent.Request);
        }

        /// <summary>
        /// 取得网站的根目录的URL
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static string GetRootUri(HttpRequest req)
        {
            var appPath = string.Empty;
            if (req == null) { return appPath; }

            var urlAuthority = req.Url.GetLeftPart(UriPartial.Authority);
            if (req.ApplicationPath == null || req.ApplicationPath == "/")
            {
                //直接安装在   Web   站点
                appPath = urlAuthority;
            }
            else
            {
                //安装在虚拟子目录下
                appPath = urlAuthority + req.ApplicationPath;
            }
            return appPath;
        }

        /// <summary>
        /// 取得网站根目录的物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            string appPath;
            var httpCurrent = HttpContext.Current;
            if (httpCurrent != null)
            {
                appPath = httpCurrent.Server.MapPath("~");
            }
            else
            {
                appPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Regex.Match(appPath, @"\\$", RegexOptions.Compiled).Success)
                {
                    appPath = appPath.Substring(0, appPath.Length - 1);
                }
            }
            return appPath;
        }

        /// <summary>
        /// 获取站点根目录URL
        /// </summary>
        /// <returns></returns>
        public static string GetRootUrl(string forumPath)
        {
            var port = HttpContext.Current.Request.Url.Port;
            return string.Format("{0}://{1}{2}{3}",
                                 HttpContext.Current.Request.Url.Scheme,
                                 HttpContext.Current.Request.Url.Host.ToString(CultureInfo.InvariantCulture),
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 forumPath);
        }

        /// <summary>
        /// 本地路径转换成URL相对路径
        /// </summary>
        /// <param name="imagesurl1"></param>
        /// <returns></returns>
        public static string UrlConvertor(string imagesurl1)
        {
            var httpCurrent = HttpContext.Current;
            if (HttpContext.Current.Request.ApplicationPath == null) { return string.Empty; }
            var tmpRootDir = httpCurrent.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath.ToString(CultureInfo.InvariantCulture));//获取程序根目录
            var imagesurl2 = imagesurl1.Replace(tmpRootDir, ""); //转换成相对路径
            imagesurl2 = imagesurl2.Replace(@"\", @"/");
            return imagesurl2;
        }

        /// <summary>
        /// 相对路径转换成服务器本地物理路径
        /// </summary>
        /// <param name="imagesurl1"></param>
        /// <returns></returns>
        public static string UrlConvertorLocal(string imagesurl1)
        {
            var httpCurrent = HttpContext.Current;
            if (HttpContext.Current.Request.ApplicationPath == null) { return string.Empty; }
            var tmpRootDir = httpCurrent.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath.ToString(CultureInfo.InvariantCulture));//获取程序根目录
            var imagesurl2 = tmpRootDir + imagesurl1.Replace(@"/", @"\"); //转换成绝对路径
            return imagesurl2;
        }

        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码集</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str, Encoding encoding)
        {
            return HttpUtility.UrlEncode(str, encoding);
        }

        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlPathEncode(string str)
        {
            return HttpUtility.UrlPathEncode(str);
        }

        /// <summary>
        /// 将已经为在 URL 中传输而编码的字符串转换为解码的字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 将已经为在 URL 中传输而编码的字符串转换为解码的字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding);
        }

        #endregion URL、网站目录

        #region Http请求

        public static string GetInputStream()
        {
            if (HttpContext.Current.Request.InputStream.Length <= 0) return string.Empty;

            var reader = new StreamReader(HttpContext.Current.Request.InputStream);
            return reader.ReadToEnd();
        }


        /// <summary>
        /// url文件路径转换为流
        /// </summary>
        /// <param name="urlPath">文件路径</param>
        /// <returns></returns>
        public static Stream GetResponseStream(string urlPath)
        {
            try
            {
                if (string.IsNullOrEmpty(urlPath))
                {
                    return Stream.Null;
                }
                return WebRequest.Create(urlPath).GetResponse().GetResponseStream();
            }
            catch (Exception)
            {
                return Stream.Null;
            }
        }

        /// <summary>
        /// 读取URL地址得到HTML(Utf-8)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url)
        {
            return GetResponse(url, Encoding.UTF8);
        }

        /// <summary>
        /// 读取URL地址得到HTML
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetResponse(string url, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return string.Empty;
                }
                var stream = WebRequest.Create(url).GetResponse().GetResponseStream();
                return stream != null ? new StreamReader(stream, encoding).ReadToEnd() : string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Similar to GetResponse(string uriArg), but uses a user/pw to log in.
        /// </summary>
        /// <param name="uriArg">e.g. "http://192.168.2.1"</param>
        /// <param name="userArg">e.g. "root"</param>
        /// <param name="pwArg">e.g. "admin"</param>
        /// <returns>string containing the http response.</returns>
        /// <example>
        /// // Example to get a response with DHCP table from my LinkSys router.
        /// string s = GetResponse( "http://192.168.2.1/DHCPTable.htm", "root", "admin" );
        /// </example>
        public static string GetResponse(string uriArg, string userArg, string pwArg)
        {
            var uri = new Uri(uriArg);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            var creds = new CredentialCache();

            // See http://msdn.microsoft.com/en-us/library/system.directoryservices.protocols.authtype.aspx for list of types.
            const string authType = "basic";

            creds.Add(uri, authType, new NetworkCredential(userArg, pwArg));
            req.PreAuthenticate = true;
            req.Credentials = creds.GetCredential(uri, authType);

            var responseStream = req.GetResponse().GetResponseStream();

            if (responseStream == null) { return string.Empty; }

            var reader = new StreamReader(responseStream);

            return reader.ReadToEnd();
        }

        /// <summary>
        ///  http POST请求(UTF-8)
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns></returns>
        public static string PostResponse(string url)
        {
            return PostResponse(url, string.Empty);
        }

        /// <summary>
        /// http POST请求url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="postData">参数</param>
        /// <param name="encode">参数返回值编码</param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData, string encode = "UTF-8")
        {
            var data = Encoding.GetEncoding(encode).GetBytes(postData);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Timeout = 20000;

            HttpWebResponse response = null;
            Stream stream = null;
            try
            {
                var swRequestWriter = request.GetRequestStream();
                swRequestWriter.Write(data, 0, data.Length);
                swRequestWriter.Close();

                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) { return string.Empty; }
                var reader = new StreamReader(stream, Encoding.GetEncoding(encode));
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //LogHelper.Error("请求发送失败\r\nurl:" + url + "\r\ndata:" + postData, ex);
                return string.Empty;
            }
            finally
            {
                if (stream != null) { stream.Close(); }
                if (response != null) { response.Close(); }
            }
        }

        public static string HttpUploadFile(string url, string filePath)
        {
            try
            {
                if (IsUrl(filePath))
                {
                    return HttpUploadFile(url, filePath, GetResponseStream(filePath));
                }
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                if (request == null) return String.Empty;

                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                var fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
                //请求头部信息
                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                fs.Read(bArr, 0, bArr.Length);
                fs.Close();
                var postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(bArr, 0, bArr.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();
                //发送请求并获取相应回应数据

                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                var instream = request.GetResponse().GetResponseStream();
                if (instream == null) return string.Empty;

                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                return content;
            }
            catch (Exception exp)
            {
                //LogHelper.Error("上传素材出错", exp);
                return "";
            }
        }
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        public static string HttpUploadFile(string url, string fileName, Stream stream)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                if (request == null) return string.Empty;

                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                var boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                var itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                var endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                //请求头部信息
                var sbHeader = new StringBuilder(
                    string.Format("Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                var postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                var postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                stream.CopyTo(postStream);
                //postStream.Write(bArr, 0, bArr.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();

                //发送请求并获取相应回应数据
                var instream = request.GetResponse().GetResponseStream();
                if (instream == null) return string.Empty;

                var sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                var content = sr.ReadToEnd();
                return content;
            }
            catch (Exception exp)
            {
                //LogHelper.Error("上传素材出错", exp);
                throw;
            }
        }

        /// <summary>
        /// 通过更新web.config文件方式来重启IIS进程池 （注：iis中web园数量须大于1,且为非虚拟主机用户才可调用该方法）
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static void RestartIISProcess()
        {
            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(GetMapPath("~/web.config"));
                var writer = new XmlTextWriter(GetMapPath("~/web.config"), null)
                {
                    Formatting = Formatting.Indented
                };
                xmldoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 域名处理
        /// </summary>
        /// <param name="domain">URL</param>
        /// <param name="area">3级域名</param>
        /// <returns></returns>
        public static string DomainPro(string domain, string area)
        {
            if (Regex.IsMatch(area, @"[\d]+") && area != "")
            {
                MatchCollection mc = Regex.Matches(domain, @"[\d]+\.");
                if (mc.Count > 0)
                {
                    Match m = mc[0];
                    domain = new Regex(m.Value).Replace(domain, "", 1, m.Index);
                }
            }
            return domain;
        }

        #endregion Http请求

        #region 微信http请求

        /// <summary>
        /// 带证书微信支付请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonVal">Xml相关数据</param>
        /// <param name="certFile">文件路径</param>
        /// <param name="certPwd">密码</param>
        /// <returns></returns>
        public static string SendCall(string url, string jsonVal, string certFile, string certPwd)
        {
            try
            {
                HttpWebRequest hp = WebRequest.Create(url) as HttpWebRequest;
                //var cer = new X509Certificate(certFile, certPwd, X509KeyStorageFlags.MachineKeySet);
                //X509Certificate cer = new X509Certificate(certFile, certPwd);
                X509Certificate2 cer = new X509Certificate2(certFile, certPwd, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                if (!string.IsNullOrEmpty(certFile) && !string.IsNullOrEmpty(certPwd))
                {
                    hp.ClientCertificates.Add(cer);
                }
                hp.Method = "POST";
                hp.ContentType = "application/x-www-form-urlencoded";
                if (!string.IsNullOrEmpty(jsonVal))
                {
                    byte[] postBytes = Encoding.UTF8.GetBytes(jsonVal);
                    hp.ContentLength = postBytes.Length;
                    Stream stream = hp.GetRequestStream();
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)hp.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            catch (Exception exp)
            {
                //LogHelper.Error("微信红包扣钱出错！exp:", exp);
                return "";
            }

        }
        /// <summary>
        /// 验证服务器证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>
        /// post方式获得json字符串
        /// </summary>
        /// <param name="url">接口URL</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static string HttpPostJson(string url, string parameter)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "text/xml";
                if (parameter != "")
                {
                    byte[] postBytes = Encoding.UTF8.GetBytes(parameter);
                    request.ContentLength = postBytes.Length;
                    Stream stream = request.GetRequestStream();
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            catch (Exception exp)
            {
                //LogHelper.Error("webUtils（HttpPostJson）", exp);
                return "";
            }
        }
        #endregion

        #region html、Url 编码、解码

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="urlDecode"></param>
        /// <returns></returns>
        public static string GetUrlDecode(string urlDecode)
        {
            return urlDecode == "" ? "" : HttpContext.Current.Server.UrlDecode(urlDecode);
        }
        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="urlDecode"></param>
        /// <returns></returns>
        public static string GetUrlEncode(string urlEncode)
        {
            return urlEncode == "" ? "" : HttpContext.Current.Server.UrlEncode(urlEncode);
        }

        /// <summary>
        /// html解码
        /// </summary>
        /// <param name="urlDecode"></param>
        /// <returns></returns>
        public static string GetHtmlDecode(string htmlDecode)
        {
            return htmlDecode == "" ? "" : HttpContext.Current.Server.HtmlDecode(htmlDecode);
        }
        /// <summary>
        /// html编码
        /// </summary>
        /// <param name="urlDecode"></param>
        /// <returns></returns>
        public static string GetHtmlEncode(string htmlEncode)
        {
            return htmlEncode == "" ? "" : HttpContext.Current.Server.HtmlEncode(htmlEncode);
        }
        #endregion

        #region 导出文件

        public static void ExportFileByStream(string fileName, MemoryStream stream, string contentType, string charset)
        {
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.Buffer = true;
            //设置Http的头信息,编码格式
            curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlPathEncode(fileName));
            curContext.Response.ContentType = contentType;//"application/ms-excel";
            //设置编码
            curContext.Response.Charset = charset;
            curContext.Response.ContentEncoding = Encoding.GetEncoding(charset);
            curContext.Response.BinaryWrite(stream.GetBuffer());
            curContext.Response.End();
        }

        #endregion

        #region UserAgent

        public static bool IsWeChat()
        {
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (string.IsNullOrEmpty(userAgent)) return false;
            return userAgent.ToLower().Contains("micromessenger");
        }

        public static IPrincipal GetCurrentUser()
        {
            return HttpContext.Current.User;
        }

        #endregion UserAgent

        #region 获取请求参数

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            var str = GetQueryString(strName);
            return string.IsNullOrEmpty(str)
                ? GetFormString(strName)
                : GetQueryString(strName);
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        private static string GetQueryString(string strName)
        {
            return HttpContext.Current.Request.QueryString[strName] ?? string.Empty;
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        private static string GetFormString(string strName)
        {
            return HttpContext.Current.Request.Form[strName] ?? string.Empty;
        }

        #endregion

        public static string PostMoths(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

    }
}
