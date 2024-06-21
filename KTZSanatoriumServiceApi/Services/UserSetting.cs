namespace KTZSanatoriumServiceApi.Services
{
    public class UserSetting : IUserSetting
    {
        public string Key { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;   
    }
}
