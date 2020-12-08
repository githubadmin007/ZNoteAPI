using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM: MISBase
    {
        public string FormCode { set; get; }
        public string Name { set; get; }
        public string Title { set; get; }
        public string TableName { set; get; }

        public SYS_FORM(string formcode) {
            string sql = "select * from sys_form where formcode=@formcode";
            var dict = dbHelper.ExecuteReader_ToDict(sql, new DbParam("formcode", formcode));
            dict.SetValueToObj(this);
        }
    }
}
