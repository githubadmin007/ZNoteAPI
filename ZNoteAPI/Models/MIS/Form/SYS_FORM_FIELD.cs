using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZNoteAPI.Models.MIS.Form
{
    public class SYS_FORM_FIELD
    {
        public string FieldBH { set; get; }
        public string Name { set; get; }
        public string CName { set; get; }
        public string Type { set; get; }
        public string Order { set; get; }
        public object Value { set; get; }
    }
}
