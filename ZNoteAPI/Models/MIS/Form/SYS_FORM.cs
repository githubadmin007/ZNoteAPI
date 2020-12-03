using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM
    {
        public string FormBH { set;  get; }
        public string FormCode { set; get; }
        public string Name { set; get; }
        public string Title { set; get; }
        public string TableName { set; get; }

        //public SYS_FORM() { }
        //public SYS_FORM(Dictionary<string, object> dict) {
        //    FormBH = dict["formbh"].ToString("");
        //    FormCode = dict["formcode"].ToString("");
        //    Name = dict["name"].ToString("");
        //    Title = dict["title"].ToString("");
        //    TableName = dict["tablename"].ToString("");
        //}
    }
}
