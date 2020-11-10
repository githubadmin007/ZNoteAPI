using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.Text;

namespace ZNoteAPI.Controllers.Basic
{
    public class FormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取表单信息
        /// </summary>
        /// <param name="formcode">表单编号</param>
        /// <returns></returns>
        public JsonResult GetFormInfo(string formcode) {
            Result result;
            try
            {
                DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("ZNoteDB");
                string sql = $"select * from sys_form where formcode='{formcode}'";
                var dict = dbHelper.ExecuteReader_ToDict(sql);
                result = ResultCode.Success.GetResult("成功", dict);
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取表单字段
        /// </summary>
        /// <param name="formcode">表单编号</param>
        /// <returns></returns>
        public JsonResult GetFields(string formcode, string keyname, string keyvalue) {
            Result result;
            try
            {
                DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("ZNoteDB");
                // 获取字段信息
                string sql = $"select *,'' as value from sys_form_field where formcode='{formcode}' order by `order`";
                var fields = dbHelper.ExecuteReader_ToList(sql);
                // 获取字段值
                if (!keyname.IsNullOrWhiteSpace() && !keyvalue.IsNullOrWhiteSpace()) {
                    // 获取表名
                    string tablename = dbHelper.ExecuteScalarToString($"select tablename from sys_form where formcode='{formcode}'");
                    string fieldnameStr = fields.Select(field => field["name"].ToString()).Join(",");
                    var datas = dbHelper.ExecuteReader_ToDict($"select {fieldnameStr} from {tablename} where {keyname}='{keyvalue}'");
                    // 新增value字段
                    foreach (var field in fields)
                    {
                        field["value"] = datas[field["name"].ToString()];
                    }
                }
                result = ResultCode.Success.GetResult("成功", fields);
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取字段候选项
        /// </summary>
        /// <param name="fieldbh"></param>
        /// <returns></returns>
        public JsonResult GetFieldItems(string fieldbh) {
            Result result;
            try
            {
                DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("ZNoteDB");
                string sql = $"select * from sys_form_field_itemsrc where fieldbh='{fieldbh}'";
                var dict = dbHelper.ExecuteReader_ToDict(sql);
                string type = dict["type"].ToString("");
                object items = null;
                switch (type) {
                    case "dict":
                        string dictbh = dict["dictbh"].ToString("");
                        sql = $"select ITEMNAME,ITEMVALUE from sys_dictitem where dictbh='{dictbh}'";
                        items = dbHelper.GetDataTable(sql);
                        break;
                    case "sql":
                        sql = dict["sql"].ToString("");
                        items = dbHelper.GetDataTable(sql);
                        break;
                    case "api":
                        // todo
                        break;
                }
                result = ResultCode.Success.GetResult("成功", items);
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取字段限制规则
        /// </summary>
        /// <param name="fieldbh"></param>
        /// <returns></returns>
        public JsonResult GetFieldRules(string fieldbh) {
            Result result;
            try
            {
                DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("ZNoteDB");
                string sql = $"select * from sys_form_field_rule where fieldbh='{fieldbh}'";
                var table = dbHelper.GetDataTable(sql);
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