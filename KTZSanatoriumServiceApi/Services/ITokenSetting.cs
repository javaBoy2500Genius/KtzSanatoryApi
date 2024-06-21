using KTZSanatoriumServiceApi.Models;

namespace KTZSanatoriumServiceApi.Services
{
    public interface ITokenSetting
    {
        DateTime TokenLifeTime { get; set; }

        string UserId { get; set; }

        UserRole UserRole { get; set; }
    }
}
