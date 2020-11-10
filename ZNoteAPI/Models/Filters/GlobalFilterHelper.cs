using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZNoteAPI.Models.Filters
{
    public class GlobalFilterHelper
    {
        /// <summary>
        /// 判断是否跳过
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsSkit(FilterContext context)
        {
            var filters = context.Filters;
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is SkipGlobalActionFilterAttribute)
                {
                    return true;
                }
            }

            // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
            // were discovered on controllers and actions. To maintain compat with 2.x,
            // we'll check for the presence of IAllowAnonymous in endpoint metadata.
            var endpoints = context.ActionDescriptor.EndpointMetadata;
            foreach (var endport in endpoints)
            {
                if (endport is SkipGlobalActionFilterAttribute)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
