using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Drive.Models.EF;
using XWidget.EFLogic;

namespace Drive.Logic {
    public class UserLogic : LogicBase<DriveContext, User, string> {
        public UserLogic(LogicManagerBase<DriveContext> logicManager) : base(logicManager) {
            CreateIgnoreIdentity = false;
        }

        public override Task BeforeCreate(User entity, params object[] parameters) {
            entity.SetPassword(entity.Password);
            return base.BeforeCreate(entity, parameters);
        }

        public override async Task BeforeUpdate(User entity, params object[] parameters) {
            var obj = await GetAsync(entity.Id);
            if (entity.Password != obj.Password) {
                entity.SetPassword(entity.Password);
            }
            await base.BeforeUpdate(entity, parameters);
        }
    }
}
