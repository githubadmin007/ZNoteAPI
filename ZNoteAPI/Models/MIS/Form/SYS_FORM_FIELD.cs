using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM_FIELD : MISBase
    {
        /// <summary>
        /// 字段编号
        /// </summary>
        public string FieldBH { set; get; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        public string CName { set; get; }
        /// <summary>
        /// 字段（控件）类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public string FieldOrder { set; get; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { set; get; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public string AccessType { set; get; }


        /// <summary>
        /// 获取字段列表
        /// </summary>
        /// <param name="formcode"></param>
        /// <returns></returns>
        static public List<SYS_FORM_FIELD> GetList(string formcode) {
            using (DatabaseHelper dbHelper = GetDBHelper()) {
                string sql = $"select * from sys_form_field where formcode=@formcode order by fieldorder";
                var list = dbHelper.ExecuteReader_ToList(sql, new DbParam("formcode", formcode));
                return list.Select(dict => dict.ToObject<SYS_FORM_FIELD>()).ToList();
            }
        }
    }
}
