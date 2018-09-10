using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Drive.Models.EF {
    public partial class User {
        public void SetPassword(string password) {
            Password = (Id + password).ToHashString<MD5>();
        }

        public bool MatchPassword(string password) {
            return (Id + password).ToHashString<MD5>() == Password;
        }
    }
}
