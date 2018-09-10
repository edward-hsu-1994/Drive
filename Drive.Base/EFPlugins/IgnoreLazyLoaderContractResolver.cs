using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Drive.Base.EFPlugins {
    public class IgnoreLazyLoaderContractResolver : CamelCasePropertyNamesContractResolver {

        public IgnoreLazyLoaderContractResolver() {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            if (member is PropertyInfo property) {
                if (property.PropertyType == typeof(ILazyLoader) ||
                       property.PropertyType.GetInterfaces().Contains(typeof(ILazyLoader))) {
                    JsonProperty prop = base.CreateProperty(member, memberSerialization);
                    prop.Ignored = true;

                    return prop;
                }
            }

            return base.CreateProperty(member, memberSerialization);
        }
    }
}
