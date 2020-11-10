using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;
using ZNoteAPI.Models.Filters;

namespace ZNoteAPI.Controllers
{
    public class SystemController : Controller
    {
        [SkipGlobalActionFilter]
        public JsonResult Index()
        {
            return Json(new { aba = 334, sfas = "2020-04-27 21:58:00", lst = "sdsafsafs,safasfsaf,fsafasfasd,dsadwsssssssfsf".Split(",") });
        }

        public JsonResult GetSystem() {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB")) {
                using (IDataReader reader = helper.ExecuteReader("select * from sys_system"))
                {
                    var data = DBConvert.IDataReader_to_DictList(reader);
                    return Json(ResultCode.Success.GetResult("成功", data));
                }
            }
        }
    }
}