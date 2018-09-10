using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Drive.Base.Swagger {
    /// <summary>
    /// Swagger Authorize操作過濾器
    /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter {
        /// <summary>
        /// Swagger過濾邏輯
        /// </summary>
        /// <param name="operation">操作</param>
        /// <param name="context">內容</param>
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context) {
            if ((context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>().Count() > 0 ||
                context.MethodInfo.DeclaringType.GetCustomAttributes<AuthorizeAttribute>().Count() > 0) &&
                context.MethodInfo.GetCustomAttributes<AllowAnonymousAttribute>().Count() == 0 &&
                context.MethodInfo.DeclaringType.GetCustomAttributes<AllowAnonymousAttribute>().Count() == 0
                ) {
                operation.Summary = "🔐" + operation.Summary;
            }
        }
    }
}
