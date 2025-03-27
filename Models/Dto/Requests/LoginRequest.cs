using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Requests
{
    public class LoginRequest
    {
        /// <summary> 帳號 </summary>
        public string? UserName { get; set; } = "";

        /// <summary> 密碼 </summary>
        public string? Password { get; set; } = "";
    }
}
