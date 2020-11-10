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
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Controllers
{
    public class IncomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取收入记录列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList() {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    string sql = $"select * from d_income where userbh='{userbh}' order by arrivaldate desc";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.GetRecordLst", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 新增收入记录
        /// </summary>
        /// <param name="amount">金额</param>
        /// <param name="type">类型</param>
        /// <param name="arrivaldate">到账日期</param>
        /// <param name="belong_year">所属年</param>
        /// <param name="belong_month">所属月</param>
        /// <param name="source">来源</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public IActionResult Add(double amount, string type, string arrivaldate, string belong_year, string belong_month, string source, string remark)
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string incomebh = Guid.NewGuid().ToString();
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    string sql = $"insert into d_income(incomebh,amount,type,arrivaldate,belong_year,belong_month,source,remark,userbh) values('{incomebh}',{amount},'{type}','{arrivaldate}',{belong_year},{belong_month},'{source}','{remark}','{userbh}')";
                    int r = helper.ExecuteNonQuery(sql);
                    if (r > 0)
                    {
                        result = Result.Success;
                    }
                    else
                    {
                        result = ResultCode.DB_InsertFail.GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.Add", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }


        // 统计
        /// <summary>
        /// 获取所有有数据的年份
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllYear(int year = 0) {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    List<string> wheres = new List<string>();
                    wheres.Add($"userbh='{userbh}'");
                    if (year > 0) wheres.Add($"belong_year={year}");
                    string where = wheres.Join(" and ");
                    string sql = $"SELECT belong_year FROM d_income where {where} group by belong_year order by belong_year";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.GetAllYear", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 按月份统计
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult Stat_Month(int year = 0) {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    List<string> wheres = new List<string>();
                    wheres.Add($"userbh='{userbh}'");
                    wheres.Add("belong_month<>0");
                    if (year > 0) wheres.Add($"belong_year={year}");
                    string where = wheres.Join(" and ");
                    string sql = $"SELECT belong_year,belong_month,sum(amount) as amount FROM d_income where {where} group by belong_year,belong_month order by belong_year,belong_month";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.Stat_Month", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 按年份统计
        /// </summary>
        /// <returns></returns>
        public JsonResult Stat_Year()
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    string sql = $"SELECT belong_year,sum(amount) as amount FROM d_income where userbh='{userbh}' group by belong_year order by belong_year";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.Stat_Year", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 按类型统计
        /// </summary>
        /// <returns></returns>
        public JsonResult Stat_Type(int year = 0)
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH(Request.Headers["Authorization"]);
                    List<string> wheres = new List<string>();
                    wheres.Add($"userbh='{userbh}'");
                    if (year > 0) wheres.Add($"belong_year={year}");
                    string where = wheres.Join(" and ");
                    string sql = $"SELECT type,sum(amount) as amount FROM d_income where {where} group by type";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Income.Stat_Type", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }
    }
}