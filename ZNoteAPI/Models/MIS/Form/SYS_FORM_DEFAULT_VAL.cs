using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZNoteAPI.Controllers;
using ZNoteAPI.Models.Token;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM_DEFAULT_VAL: MISBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string DefaultValBH { set; get; }
        /// <summary>
        /// 初始化组编号
        /// </summary>
        public string GroupCode { set; get; }
        /// <summary>
        /// 字段编号
        /// </summary>
        public string FieldBH { set; get; }
        /// <summary>
        /// 初始化时间（什么时候应该进行初始化）
        /// </summary>
        public string InitTime { set; get; }
        /// <summary>
        /// 初始化类型
        /// </summary>
        public string InitType { set; get; }
        /// <summary>
        /// 初始化信息
        /// </summary>
        public string InitInfo { set; get; }

        /// <summary>
        /// 获取初始化配置列表
        /// </summary>
        /// <param name="initcode"></param>
        /// <returns></returns>
        static public List<SYS_FORM_DEFAULT_VAL> GetList(string initcode)
        {
            if (initcode.IsNullOrWhiteSpace()) {
                return new List<SYS_FORM_DEFAULT_VAL>();
            }
            using (DatabaseHelper dbHelper = GetDBHelper())
            {
                string sql = "select * from sys_form_default_val where groupcode=@initcode";
                var list = dbHelper.ExecuteReader_ToList(sql, new DbParam("initcode", initcode));
                return list.Select(dict => dict.ToObject<SYS_FORM_DEFAULT_VAL>()).ToList();
            }
        }

        public object GetInitValue(FormController Controller) {
            // GUID、当前时间、固定值、表达式
            switch (InitType) {
                case "GUID": return Guid.NewGuid().ToString();
                case "当前时间": return DateTime.Now;
                case "固定值": return InitInfo;
                case "表达式": return Expression(Controller);
            }
            return null;
        }

        /// <summary>
        /// 【待完善！！！】表达式
        /// </summary>
        /// <returns></returns>
        private string Expression(FormController Controller) {
            string userbh = Controller.TokenHelper.GetUserBH();
            return InitInfo.Replace("{USERBH}", userbh);
        }
    }
}
