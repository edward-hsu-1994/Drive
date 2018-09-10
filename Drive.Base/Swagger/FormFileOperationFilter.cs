using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drive.Base.Swagger {
    /// <summary>
    /// Swagger FormFile操作過濾器
    /// </summary>
    public class FormFileOperationFilter : IOperationFilter {
        /// <summary>
        /// Swagger過濾邏輯
        /// </summary>
        /// <param name="operation">操作</param>
        /// <param name="context">內容</param>
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context) {
            var filesParams = context.ApiDescription.ActionDescriptor
                .Parameters
                .Where(x => x.ParameterType == typeof(IFormFileCollection));

            if (operation.Parameters?.Count > 0) {
                var parameters2 = operation.Parameters
                .Where(x => filesParams.Any(y => x.Name == y.Name));

                foreach (NonBodyParameter parameter in parameters2) {
                    parameter.Type = "file";
                }
            }
        }
    }
}
