using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.Text;

namespace ZNoteAPI.Controllers.Basic
{
    public class SysDictController : Controller
    {
        /// <summary>
        /// 获取字典候选项
        /// </summary>
        /// <param name="DictBH"></param>
        /// <returns></returns>
        public JsonResult GetItems(string DictBH)
        {
            Result result;
            try
            {
                DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("ZNoteDB");// SQLiteDatabaseHelper
                string sql = $"select ITEMNAME,ITEMVALUE from sys_dictitem where dictbh='{DictBH}'";
                DataTable table = dbHelper.GetDataTable(sql);
                result = ResultCode.Success.GetResult("成功", table);
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }
    }
}