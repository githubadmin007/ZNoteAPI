using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Controllers
{
    public class _BaseController : Controller
    {
        protected TokenHelper TokenHelper {
            get {
                string token =  Request.Headers["Authorization"];
                return TokenHelper.CreateTokenHelper(token);
            }
        }
    }
}