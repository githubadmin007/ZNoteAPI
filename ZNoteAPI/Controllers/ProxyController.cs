using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ZJH.BaseTools.Net;
using ZJH.BaseTools.Text;

namespace ZNoteAPI.Controllers
{
    public class ProxyController : Controller
    {
        static string appid = ZJH.BaseTools.GlobalConfig.AppCfg.getValueByPath("configuration/BaiDuTranslate/Appid");
        static string secretKey = ZJH.BaseTools.GlobalConfig.AppCfg.getValueByPath("configuration/BaiDuTranslate/SecretKey");
        static string apiUrl = ZJH.BaseTools.GlobalConfig.AppCfg.getValueByPath("configuration/BaiDuTranslate/ApiUrl");

        public IActionResult Translate(string q, string from, string to)
        {
            Result result;
            try {
                string salt = DateTime.Now.Ticks.ToString();
                string sign = Encrypt.MD5Encrypt(appid + q + salt + secretKey).ToLower();
                string url = $"{apiUrl}?q={q}&from={from}&to={to}&appid={appid}&salt={salt}&sign={sign}";
                string reponse = ZWebClient.DownloadString(url);
                JObject reObj = JObject.Parse(reponse);
                result = Result.CreateSuccess("", reObj);
            }
            catch (Exception ex) {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }
    }
}