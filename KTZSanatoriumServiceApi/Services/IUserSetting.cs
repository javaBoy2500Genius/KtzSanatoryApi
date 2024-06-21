namespace KTZSanatoriumServiceApi.Services
{
    public interface IUserSetting
    {
        string Key { get; set; }
        string SecretKey { get; set; }
    }
}
