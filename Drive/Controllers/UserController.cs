using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Drive.Base.Mvc;
using Drive.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drive.Controllers {
    /// <summary>
    /// 使用者控制器
    /// </summary>
    public class UserController : ManageBaseController {
        public UserController(DriveLogicManager manager) : base(manager) {
        }

        [AllowAnonymous]
        [HttpGet]
        public string Get(string aa) {
            return aa;
        }
    }
}
