
using KTZSanatoriumServiceApi.Models;

namespace KTZSanatoriumServiceApi.Services
{
    public class TokenSetiing : ITokenSetting
    {
        public DateTime TokenLifeTime { get; set; }
        public string UserId { get; set; }=string.Empty;
        public UserRole UserRole { get; set; }
    }
}
