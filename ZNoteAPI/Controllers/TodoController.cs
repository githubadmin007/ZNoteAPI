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

namespace ZNoteAPI.Controllers
{
    public class TodoController : _BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 新增待办项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <param name="deadline"></param>
        /// <param name="pid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IActionResult Add(string name, string content, string type, string deadline, string state = "进行中", string parentbh = "")
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string todobh = Guid.NewGuid().ToString();
                    string userbh = TokenHelper.GetUserBH();
                    if (deadline.IsNullOrWhiteSpace()) deadline = "9999-12-31";
                    deadline += " 23:59:59";
                    string sql = $"insert into d_todo(todobh,name,content,type,deadline,parentbh,state,userbh) values('{todobh}','{name}','{content}','{type}','{deadline}','{parentbh}','{state}','{userbh}')";
                    int r = helper.ExecuteNonQuery(sql);
                    if (r > 0)
                    {
                        result = Result.Success;
                    }
                    else {
                        result = ResultCode.DB_InsertFail.GetResult();
                    }
                }
            }
            catch (Exception ex) {
                Logger.log("Todo.Add", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改待办项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <param name="deadline"></param>
        /// <param name="todobh"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IActionResult Modify(string name, string content, string type, string deadline, string todobh) {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string id = Guid.NewGuid().ToString();
                    string userbh = TokenHelper.GetUserBH();
                    if (deadline.IsNullOrWhiteSpace()) deadline = "9999-12-31";
                    deadline += " 23:59:59";
                    string sql = $"update d_todo set name='{name}',content='{content}',type='{type}',deadline='{deadline}' where todobh='{todobh}' and userbh='{userbh}'";
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
                Logger.log("Todo.Add", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取待办项信息
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        public IActionResult GetInfo(string todobh) {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    string sql = $"SELECT * FROM v_todo where userbh='{userbh}' and todobh='{todobh}'";
                    var dict = helper.ExecuteReader_ToDict(sql);
                    if (dict != null)
                    {
                        result = ResultCode.Success.GetResult(dict);
                    }
                    else {
                        result = Result.CreateDefeat("查无此待办项");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo.GetToDoList", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取待办项
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentbh"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IActionResult GetList(string type, string parentbh, string state)
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    string sql = $"SELECT * FROM v_todo where userbh='{userbh}'";
                    sql += $" and parentbh='{parentbh.ToString("")}'";
                    if (!type.IsNullOrWhiteSpace()) sql += $" and type='{type}'";
                    if (!state.IsNullOrWhiteSpace()) sql += $" and state in ({ state.Split(',').Select(s => $"'{s}'").Join(",") })";
                    sql += " order by deadline";
                    DataTable table = helper.GetDataTable(sql);
                    result = ResultCode.Success.GetResult(table);
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo.GetList", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 完成待办项
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        public IActionResult Finish(string todobh) {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    string sql = $"SELECT count(*) FROM d_todo where userbh='{userbh}' and parentbh='{todobh}' and state='进行中'";
                    if (helper.ExecuteScalar(sql).ToInt32() == 0)
                    {
                        string finishtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        sql = $"update d_todo set state='完成',finishtime='{finishtime}' where userbh='{userbh}' and todobh='{todobh}'";
                        if (helper.ExecuteNonQuery(sql) > 0)
                        {
                            result = Result.Success;
                        }
                        else
                        {
                            result = ResultCode.DB_UpdateFail.GetResult();
                        }
                    }
                    else {
                        result = ResultCode.Defeat.GetResult("有未完成的子待办项");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo.Finish", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 放弃待办项
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        public IActionResult Abandon(string todobh)
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    // 获取所有子项
                    string userbh = TokenHelper.GetUserBH();
                    var lst = _GetAllSubTodoBH(todobh);
                    lst.Insert(0, todobh);
                    string todobhs = lst.Select(bh => $"'{bh}'").Join(",");
                    // 设置为今日完成
                    string abandontime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string sql = $"update d_todo set state='放弃',abandontime='{abandontime}' where userbh='{userbh}' and todobh in ({ todobhs })";
                    if (helper.ExecuteNonQuery(sql) > 0)
                    {
                        result = Result.Success;
                    }
                    else
                    {
                        result = ResultCode.DB_UpdateFail.GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo.Abandon", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }

        /// <summary>
        /// 将待办项设置为今日完成
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        public IActionResult FinishToday(string todobh)
        {
            Result result;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    // 获取所有子项
                    string userbh = TokenHelper.GetUserBH();
                    var lst = _GetAllSubTodoBH(todobh);
                    lst.Insert(0, todobh);
                    string todobhs = lst.Select(bh => $"'{bh}'").Join(",");
                    // 设置为今日完成
                    string deadline = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                    string sql = $"update d_todo set deadline='{deadline}' where userbh='{userbh}' and todobh in ({ todobhs })";
                    if (helper.ExecuteNonQuery(sql) > 0)
                    {
                        result = Result.Success;
                    }
                    else
                    {
                        result = ResultCode.DB_UpdateFail.GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo.Finish", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }
        /// <summary>
        /// 获取父节点编号
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        public IActionResult GetParentBH(string todobh) {
            Result result;
            try
            {
                string parentbh = _GetParentBH(todobh);
                result = ResultCode.Success.GetResult("", parentbh);
            }
            catch (Exception ex)
            {
                Logger.log("Todo.GetParentBH", ex);
                result = Result.CreateDefeat(ex.Message);
            }
            return Json(result);
        }






        /// <summary>
        /// 获取待办事项下属的所有子待办事项编号
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        List<string> _GetAllSubTodoBH(string todobh) {
            List<string> lst = new List<string>();
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    string todobhs = todobh.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(bh => $"'{bh}'").Join(",");
                    string sql = $"SELECT todobh FROM d_todo where userbh='{userbh}' and parentbh in ({ todobhs })";
                    // 查找传入的todobh的子待办事项编号
                    lst = helper.ExecuteReader_ToList(sql).Select(d => d["todobh"].ToString()).ToList();
                    // 如果存在子待办事项，继续查找子待办事项的子待办事项
                    if (lst.Count > 0) {
                        string sub_todobhs = lst.Join(",");
                        var sLst = _GetAllSubTodoBH(sub_todobhs);
                        lst.AddRange(sLst);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo._GetAllSubTodoBH", ex);
            }
            return lst;
        }
        /// <summary>
        /// 获取待办事项的父节点编号
        /// </summary>
        /// <param name="todobh"></param>
        /// <returns></returns>
        string _GetParentBH(string todobh) {
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
                {
                    string userbh = TokenHelper.GetUserBH();
                    string sql = $"SELECT parentbh FROM d_todo where userbh='{userbh}' and todobh='{todobh}'";
                    return helper.ExecuteScalar(sql).ToString("");
                }
            }
            catch (Exception ex)
            {
                Logger.log("Todo._GetParentBH", ex);
                return "";
            }
        }
    }
}