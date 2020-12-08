using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Controllers
{
    public class _BaseController : Controller
    {
        /// <summary>
        /// Token帮助类
        /// </summary>
        public TokenHelper TokenHelper {
            get {
                string token =  Request.Headers["Authorization"];
                return TokenHelper.CreateTokenHelper(token);
            }
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetRequestParam(string key) {
            if (Request.Query != null && Request.Query[key].Count > 0) {
                return Request.Query[key];
            }
            if (Request.Form != null && Request.Form[key].Count>0) {
                return Request.Form[key];
            }
            return null;
        }
    }
}