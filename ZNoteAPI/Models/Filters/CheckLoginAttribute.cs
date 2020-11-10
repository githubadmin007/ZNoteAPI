using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.Text;
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Models.Filters
{
    public class CheckLoginAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (GlobalFilterHelper.IsSkit(context)) return;
            string token = context.HttpContext.Request.Headers["Authorization"];
            if (token.IsNullOrWhiteSpace())
            {
                // 无Token，未登录
                OkObjectResult res = new OkObjectResult(ResultCode.NotLogin.GetResult());
                context.Result = res;
            }
            else {
                if (!TokenHelper.CheckToken(token)) {
                    // Token无效，登陆超时
                    OkObjectResult res = new OkObjectResult(ResultCode.LoginTimeout.GetResult());
                    context.Result = res;
                }
            }
        }
    }
}
