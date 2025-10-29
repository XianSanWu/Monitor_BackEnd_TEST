namespace Models.Entities.Requests
{
    public class AuthEntityRequest
    {
        public class LoginEntityRequest
        {
            /// <summary> 帳號 </summary>
            public string? UserName { get; set; } = "";

            /// <summary> 密碼 </summary>
            public string? Password { get; set; } = "";
        }
    }
}
