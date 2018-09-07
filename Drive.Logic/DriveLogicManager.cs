using System;
using System.Collections.Generic;
using System.Text;
using Drive.Models.EF;
using XWidget.EFLogic;

namespace Drive.Logic {
    public class DriveLogicManager : LogicManagerBase<DriveContext> {
        public DriveLogicManager(DriveContext dbcontext) : base(dbcontext) { }
    }
}
