namespace Models.Dto.Responses
{
    public class AuthResponse
    {
        public bool IsLogin { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
