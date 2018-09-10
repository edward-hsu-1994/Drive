using System;
using System.Collections.Generic;
using System.Text;
using Drive.Models.EF;
using XWidget.EFLogic;

namespace Drive.Logic {
    public class DriveLogicManager : LogicManagerBase<DriveContext> {
        public UserLogic UserLogic { get; set; }
        public DriveLogicManager(DriveContext dbcontext) : base(dbcontext) { }
    }
}
