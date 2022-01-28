using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;
using ZNoteAPI.Models.Filters;
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Controllers
{
    public class UserController : _BaseController
    {
        /*
         * 1**
        信息，服务器收到请求，需要请求者继续执行操作
        2**
        成功，操作被成功接收并处理
        3**
        重定向，需要进一步的操作以完成请求
        4**
        客户端错误，请求包含语法错误或无法完成请求
        5**
        服务器错误，服务器在处理请求的过程中发生了错误

        200 : 正常返回
        501 : 用户不存在
        502 : 密码错误
        503 : 用户未登录

        401 : 用户权限不足
        403 : 用户名错误或密码错误
        405 : Token 不合法（Token错了）
        406 : 非管理员身份
        */
        [SkipGlobalActionFilter]
        public JsonResult Login(string loginname,string password) {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
            {
                var userInfo = helper.ExecuteReader_ToDict($"SELECT * FROM u_user where login_name=@loginname", new DbParam("loginname", loginname));
                if (userInfo == null)
                {
                    return Json(ResultCode.UserNotExist.GetResult());
                }
                else
                {
                    string _pw = userInfo["password"].ToString();
                    if (_pw == password)
                    {
                        string token = TokenHelper.CreateToken(userInfo["userbh"].ToString(""));
                        userInfo.Add("token", token);
                        return Json(ResultCode.Success.GetResult("登陆成功", userInfo));
                    }
                    else
                    {
                        return Json(ResultCode.UserPwError.GetResult());
                    }
                }
            }
        }

        /// <summary>
        /// 检查密码是否正确
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult CheckPassword(string password)
        {
            Result result;
            try {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    bool pass = helper.HasRecord("u_user", "userbh=@userbh and password=@password", new DbParam("userbh", userbh), new DbParam("password", password));
                    result = Result.CreateSuccess("", pass);
                }
            }
            catch (Exception ex) {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserInfo() {
            var userInfo = TokenHelper.GetUserInfo();
            if (userInfo == null)
            {
                return Json(ResultCode.Defeat.GetResult("获取失败"));
            }
            else {
                return Json(ResultCode.Success.GetResult("获取成功", userInfo));
            }
        }

    }
}