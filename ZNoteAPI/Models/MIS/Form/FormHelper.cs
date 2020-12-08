using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using ZNoteAPI.Controllers;
using ZNoteAPI.Models.MIS.Form;

namespace ZNoteAPI.Models.MIS
{
    public class FormHelper : MISBase
    {
        /// <summary>
        /// 【待优化！！！】控制器，为了获取与会话有关的一些信息。如userbh等等。
        /// </summary>
        public FormController Controller { get; set; }


        /// <summary>
        /// 表单编号
        /// </summary>
        public string FormCode { get; set; }
        /// <summary>
        /// 初始化组编号
        /// </summary>
        public string InitCode { get; set; }
        /// <summary>
        /// 权限控制组编号
        /// </summary>
        public string CtrlCode { get; set; }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// 查找记录的键名
        /// </summary>
        public string KeyNames { get; set; }
        /// <summary>
        /// 查找记录的键值
        /// </summary>
        public string KeyValues { get; set; }


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


        public new void Dispose() {
            base.Dispose();
            FormInfo.Dispose();
            Fields.ForEach(field => field.Dispose());
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
                    _FormInfo = new SYS_FORM(FormCode);
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
                    // 初始化。表单只读时不初始化，确保展现的与数据库中的一致。
                    if (!ReadOnly) {
                        SetDefaultVal();
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
                    _Fields = SYS_FORM_FIELD.GetList(FormCode);
                    // 获取值
                    foreach (var field in _Fields)
                    {
                        field.Value = FormData[field.Name];
                    }
                    // 设置字段权限
                    SetFieldAccess();
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
                    // 初始化
                    SetDefaultVal();
                    if (IsNew)
                    {
                        return dbHelper.InsertDataRow(FormData, FormInfo.TableName);
                    }
                    else {
                        string sql = $"select * from {FormInfo.TableName} where {KeyNames}=@KeyValues";
                        return dbHelper.UpdateDataRow(FormData, sql, new DbParam("KeyValues", KeyValues));
                    }
                }
            }
            catch (Exception ex) {
                Logger.log("FormHelper.SaveData", ex);
            }
            return false;
        }

        /// <summary>
        /// 初始化默认值，调用此函数前要对_IsNew进行初始化
        /// </summary>
        private void SetDefaultVal() {
            if (_FormData == null) // 不能调用FormData否则会陷入循环调用
            {
                return;
            }
            var list = SYS_FORM_DEFAULT_VAL.GetList(InitCode);
            list.ForEach(config => {
                SYS_FORM_FIELD field = Fields.Find(field => field.FieldBH == config.FieldBH);
                if (field == null) {
                    return;
                }
                if (config.InitTime == "总是初始化" 
                    || (config.InitTime == "空值时初始化" && _FormData[field.Name] == DBNull.Value)
                    || (config.InitTime == "新增时初始化" && _IsNew) // 不能调用IsNew否则会陷入循环调用
                ) {
                    _FormData[field.Name] = config.GetInitValue(Controller);
                }
            });
        }

        /// <summary>
        /// 设置字段权限
        /// </summary>
        private void SetFieldAccess() {
            if (_Fields == null) // 不能调用Fields否则会陷入循环调用
            {
                return;
            }
            var list = SYS_FORM_CTRL_ACCESS.GetList(CtrlCode);
            list.ForEach(config => {
                var field = _Fields.Find(field => field.FieldBH == config.FieldBH);
                if (field != null) {
                    field.AccessType = config.AccessType;
                }
            });
        }

    }
}
