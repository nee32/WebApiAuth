using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebApiAuth.Model;

namespace WebApiAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthAttribute : ActionFilterAttribute
    {
        //权限编码
        private readonly string[] _authCode;

        private readonly ILogger<AuthAttribute> _logger;

        private readonly Configs _config;

        public AuthAttribute(IOptions<Configs> setting, ILogger<AuthAttribute> logger, string[] authCode = null)
        {
            this._authCode = authCode;
            this._config = setting.Value;
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("[Test Log]Getting items.");

            var nowTime = DateTime.Now;
            var request = context.HttpContext.Request;
            //string method = request.Method;
            string app_key = String.Empty, timestamp = string.Empty, sign = string.Empty;// method = string.Empty

            if (request.Headers.ContainsKey("app_key"))// app key
            {
                app_key = HttpUtility.UrlDecode(request.Headers["app_key"]);
            }
            if (request.Headers.ContainsKey("timestamp"))//当前时间
            {
                timestamp = HttpUtility.UrlDecode(request.Headers["timestamp"]);
            }
            //if (request.Headers.ContainsKey("method"))//方法名称
            //{
            //    method = HttpUtility.UrlDecode(request.Headers["method"]);
            //}

            if (request.Headers.ContainsKey("sign"))
            {
                sign = HttpUtility.UrlDecode(request.Headers["sign"]);
            }

            //判断请求头是否包含以下参数
            if (string.IsNullOrEmpty(app_key) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(sign))
            {
                context.Result = new ContentResult() { Content = "请求参数无效", ContentType = "application/json; charset=utf-8", StatusCode = StatusCodes.Status200OK };
                base.OnActionExecuting(context);
                return;
            }

            //判断timestamp是否过期
            DateTime requestTime = DateTime.Now;
            bool isTime = DateTime.TryParse(timestamp, out requestTime);

            //if ((nowTime - requestTime).TotalSeconds > _config.ApiRequestExpireTime || !isTime)
            //{
            //    context.Result = new JsonResult("请求时间已过期");
            //    base.OnActionExecuting(context);
            //    return;
            //}

            //判断app_key是否有效
            var user = _config.ApiUser.FirstOrDefault(c => c.app_key == app_key);
            if (user == null)
            {
                context.Result = new JsonResult("app_key无效");
                base.OnActionExecuting(context);
                return;
            }

            //根据请求类型拼接参数
           string data = request.QueryString.Value;

            //比较 sign 是否正确
            var requestParames = new Dictionary<string, string>
            {
                { "app_key",user.app_key},
                { "timestamp",timestamp},
                //{ "method",method},
            };

            var newsign = Signature(requestParames, user.secret);
            if (newsign != sign)
            {
                context.Result = new JsonResult("签名不正确");
                base.OnActionExecuting(context);
                return;
            }
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }


        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        private string Signature(Dictionary<string, string> keyValuePairs,string secret)
        {
            // 第一步：把字典按Key的字母顺序排序
            var sortedParams = new SortedDictionary<string, string>(keyValuePairs, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }

            // 第三步：使用HMAC加密
            HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString().ToLower();
        }
    }
}
