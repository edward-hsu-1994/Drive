using System;
using System.Collections.Generic;
using System.Text;
using Drive.Models.EF;
using XWidget.EFLogic;

namespace Drive.Logic {
    public class UserLogic : LogicBase<DriveContext, User, string> {
        public UserLogic(LogicManagerBase<DriveContext> logicManager) : base(logicManager) {
            CreateIgnoreIdentity = false;
        }
    }
}
