namespace Models.Dto.Responses
{
    public class AuthResponse
    {
        public bool IsLogin { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TokenUuid { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAt { get; set; }

        public string RefreshToken { get; set; } = string.Empty; 
        public DateTime RefreshTokenExpiresAt { get; set; }    
    }
}
