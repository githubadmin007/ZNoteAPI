using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace ZNoteAPI.Models.Token
{
    public class TokenHelper
    {
        /// <summary>
        /// 创建token
        /// </summary>
        /// <param name="userbh">用户编号</param>
        /// <param name="time">有效期，单位为秒，为负代表无限时</param>
        /// <returns></returns>
        public static string CreateToken(string userbh, long time = -1) {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB")) {
                string tokenbh = Guid.NewGuid().ToString();
                string token = Guid.NewGuid().ToString();
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string expirationtime = time > 0 ? DateTime.Now.AddSeconds(time).ToString("yyyy-MM-dd HH:mm:ss") : "";
                string sql = $"insert into  sys_token(tokenbh,token,createtime,expirationtime,userbh) values('{tokenbh}','{token}','{now}','{expirationtime}','{userbh}')";
                return helper.ExecuteNonQuery(sql) > 0 ? token : "";
            }
        }
        /// <summary>
        /// 检查Token有效性
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CheckToken (string token) {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
            {
                string sql = $"select expirationtime from sys_token where token='{token}'";
                object obj = helper.ExecuteScalar(sql);
                if (obj == null) {
                    return false;
                }
                DateTime expirationtime = (DateTime)obj;
                return expirationtime == DateTime.MinValue || expirationtime > DateTime.Now;
            }
        }
        /// <summary>
        /// 通过token获取用户编号
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetUserBH(string token) {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("ZNoteDB"))
            {
                string sql = $"select userbh from sys_token where token='{token}'";
                string userbh = helper.ExecuteScalar(sql).ToString("");
                return userbh;
            }
        }
    }
}
