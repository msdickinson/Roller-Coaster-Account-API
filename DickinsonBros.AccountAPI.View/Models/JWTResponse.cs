
namespace DickinsonBros.AccountAPI.View.Models
{
    public class JWTResponse
    {
        public string? AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
        public string? TokenType { get; set; }
    }
}
