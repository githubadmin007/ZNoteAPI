using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.DB.Extend;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;
using ZNoteAPI.Models.MIS.Form;

namespace ZNoteAPI.Models.MIS
{
    public class FormHelper : IDisposable
    {
        DatabaseHelper dbHelper = DatabaseHelper.CreateByConnName("MISDB");
        public string FormCode { get; }
        public string KeyNames { get; }
        public string KeyValues { get; }
        /// <summary>
        /// 表单基本信息
        /// </summary>
        SYS_FORM _FormInfo = null;
        /// <summary>
        /// 字段信息
        /// </summary>
        List<SYS_FORM_FIELD> _Fields = null;
        /// <summary>
        /// 表单数据
        /// </summary>
        DataRow _FormData = null;
        /// <summary>
        /// 是否为新增数据
        /// </summary>
        bool _IsNew = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formcode"></param>
        /// <param name="keyname"></param>
        /// <param name="keyvalue"></param>
        public FormHelper(string formcode, string keyname = "", string keyvalue = "")
        {
            FormCode = formcode;
            KeyNames = keyname;
            KeyValues = keyvalue;
        }

        public void Dispose()
        {
            if (dbHelper != null) {
                dbHelper.Dispose();
            }
        }


        /// <summary>
        /// 表单基本信息
        /// </summary>
        public SYS_FORM FormInfo
        {
            get
            {
                if (_FormInfo == null)
                {
                    string sql = "select * from sys_form where formcode=@formcode";
                    var dict = dbHelper.ExecuteReader_ToDict(sql, new DbParam("formcode", FormCode));
                    _FormInfo = dict.ToObject<SYS_FORM>();
                }
                return _FormInfo;
            }
        }
        /// <summary>
        /// 是否为新增数据
        /// </summary>
        public bool IsNew {
            get {
                // FormData必不为null，只是为了调用一次他的get方法，以初始化_IsNew对象。
                return FormData != null && _IsNew;
            }
        }
        /// <summary>
        /// 表单数据
        /// </summary>
        public DataRow FormData
        {
            get {
                if (_FormData == null) {
                    DataTable table = null;
                    // TODO:存在SQL注入风险
                    if (KeyNames.IsNullOrWhiteSpace() || KeyValues.IsNullOrWhiteSpace())
                    {
                        table = dbHelper.GetDataTable($"select * from {FormInfo.TableName} where 1=2");
                    }
                    else {
                        string sql = $"select * from {FormInfo.TableName} where {KeyNames}=@KeyValues";
                        table = dbHelper.GetDataTable(sql, new DbParam("KeyValues", KeyValues));
                    }
                    // 获取第一行数据，或者创建一行数据
                    if (table == null || table.Rows.Count == 0)
                    {
                        _IsNew = true;
                        _FormData = table.NewRow();
                        table.Rows.Add(_FormData);
                    }
                    else {
                        _FormData = table.Rows[0];
                    }
                }
                return _FormData;
            }
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        public List<SYS_FORM_FIELD> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    string sql = $"select FieldBH,Name,CName,Type,`Order` from sys_form_field where formcode=@formcode order by `order`";
                    var list = dbHelper.ExecuteReader_ToList(sql, new DbParam("formcode", FormCode));
                    _Fields = list.Select(dict => dict.ToObject<SYS_FORM_FIELD>()).ToList();
                    // 如果不是新数据，获取值
                    if (!IsNew)
                    {
                        foreach (var field in _Fields)
                        {
                            field.Value = FormData[field.Name];
                        }
                    }
                }
                return _Fields;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="DataJSON"></param>
        /// <returns></returns>
        public bool SaveData(string DataJSON) {
            try {
                if (FormData.SetData(DataJSON)) {
                    if (IsNew)
                    {
                        return dbHelper.InsertDataRow(FormData, FormInfo.TableName);
                    }
                    else {
                        return dbHelper.UpdateDataRow(FormData, FormInfo.TableName);
                    }
                }
            }
            catch (Exception ex) {
                Logger.log("FormHelper.SaveData", ex);
            }
            return false;
        }
    }
}
