using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZNoteAPI.Models.Filters
{
    /// <summary>
    /// 用于标记跳过全局过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipGlobalActionFilterAttribute : Attribute
    {
    }
}
