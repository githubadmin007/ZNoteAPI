using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.Text;
using ZNoteAPI.Models.Filters;
using ZNoteAPI.Models.MIS;

namespace ZNoteAPI.Controllers
{
    public class FormController : _BaseController
    {
        private FormHelper CreateFormHelper() {
            string formcode = GetRequestParam("formcode");
            string initcode = GetRequestParam("initcode");
            string ctrlcode = GetRequestParam("ctrlcode");
            string @readonly = GetRequestParam("readonly");
            string keyname = GetRequestParam("keyname");
            string keyvalue = GetRequestParam("keyvalue");
            FormHelper form = new FormHelper() {
                FormCode = formcode,
                InitCode = initcode,
                CtrlCode = ctrlcode,
                ReadOnly = @readonly == "true",
                KeyNames = keyname,
                KeyValues = keyvalue,
                Controller = this
            };
            return form;
        }

        /// <summary>
        /// 获取表单信息
        /// </summary>
        /// <param name="formcode">表单编号</param>
        /// <returns></returns>
        public JsonResult GetFormInfo() {
            Result result;
            try
            {
                using (FormHelper form = CreateFormHelper())
                {
                    result = ResultCode.Success.GetResult("成功", form.FormInfo);
                }
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
        public JsonResult GetFields() {
            Result result;
            try
            {
                using (FormHelper form = CreateFormHelper())
                {
                    result = ResultCode.Success.GetResult("成功", form.Fields);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="formcode"></param>
        /// <param name="data"></param>
        /// <param name="keyname"></param>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public JsonResult Save(string data) {
            Result result;
            try
            {
                using (FormHelper form = CreateFormHelper())
                {
                    result = form.SaveData(data) ? Result.Success : Result.Defeat;
                }
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