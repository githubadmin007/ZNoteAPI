using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.DB;

namespace ZNoteAPI.Models.MIS
{
    /// <summary>
    /// 所有MIS相关的类都要继承此类
    /// </summary>
    public class MISBase : IDisposable
    {
        /// <summary>
        /// 获取一个数据库帮助类示例
        /// </summary>
        /// <returns></returns>
        protected static DatabaseHelper GetDBHelper()
        {
            return DatabaseHelper.CreateByConnName("MISDB");
        }

        private DatabaseHelper _dbHelper = null;
        protected DatabaseHelper dbHelper {
            get {
                if (_dbHelper == null) {
                    _dbHelper = GetDBHelper();
                }
                return _dbHelper;
            }
        }

        public void Dispose()
        {
            if (_dbHelper != null)
            {
                _dbHelper.Dispose();
            }
        }
    }
}
