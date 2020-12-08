using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM_CTRL_ACCESS : MISBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string AccessBH { set; get; }
        /// <summary>
        /// 初始化组编号
        /// </summary>
        public string GroupCode { set; get; }
        /// <summary>
        /// 字段编号
        /// </summary>
        public string FieldBH { set; get; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public string AccessType { set; get; }

        /// <summary>
        /// 获取权限配置列表
        /// </summary>
        /// <param name="initcode"></param>
        /// <returns></returns>
        static public List<SYS_FORM_CTRL_ACCESS> GetList(string ctrlcode)
        {
            if (ctrlcode.IsNullOrWhiteSpace())
            {
                return new List<SYS_FORM_CTRL_ACCESS>();
            }
            using (DatabaseHelper dbHelper = GetDBHelper())
            {
                string sql = "select * from sys_form_ctrl_access where groupcode=@ctrlcode";
                var list = dbHelper.ExecuteReader_ToList(sql, new DbParam("ctrlcode", ctrlcode));
                return list.Select(dict => dict.ToObject<SYS_FORM_CTRL_ACCESS>()).ToList();
            }
        }
    }
}
