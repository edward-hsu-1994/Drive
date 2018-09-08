using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Drive.Models.EF {
    /// <summary>
    /// 使用者資訊
    /// </summary>
    [Table("User")]
    public partial class User {
        /// <summary>
        /// 帳號
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 密碼雜湊
        /// </summary>
        [MinLength(32)]
        [MaxLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 是否為系統管理員
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
