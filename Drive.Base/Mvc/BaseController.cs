using Drive.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using XWidget.Web.Exceptions;

namespace Drive.Base.Mvc {
    /// <summary>
    /// 基礎控制器
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BaseController : Controller {
        /// <summary>
        /// 邏輯管理器
        /// </summary>
        public DriveLogicManager Manager { get; private set; }

        /// <summary>
        /// 初始化基礎控制器
        /// </summary>
        /// <param name="database">資料庫實例</param>
        public BaseController(DriveLogicManager manager) {
            this.Manager = manager;
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception != null) {
                if (context.Exception is ExceptionBase ex) {
                    var result = Json(ex);
                    result.StatusCode = ex.StatusCode;
                    context.Result = result;
                    context.ExceptionHandled = true;
                } else {
                    var result = Json(new UnknowException(context.Exception.Message + "," + context.Exception.InnerException?.Message));
                    result.StatusCode = 500;
                    context.Result = result;
                    context.ExceptionHandled = true;
                }
            }
            base.OnActionExecuted(context);
        }
    }
}
