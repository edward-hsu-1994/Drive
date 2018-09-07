using Drive.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drive.Base.Mvc {
    /// <summary>
    /// 後台基礎控制器
    /// </summary>
    [Route("api/[area]/[controller]")]
    [Area("manage")]
    [Authorize(/*Roles = "Administrator"*/)]
    [ApiController]
    public class ManageBaseController : BaseController {
        /// <summary>
        /// 初始化基礎控制器
        /// </summary>
        /// <param name="database">資料庫實例</param>
        public ManageBaseController(DriveLogicManager manager) : base(manager) { }
    }
}
