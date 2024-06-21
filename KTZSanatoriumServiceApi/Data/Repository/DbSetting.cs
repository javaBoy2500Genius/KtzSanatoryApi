namespace KTZSanatoriumServiceApi.Data.Repository
{
    public class DbSetting : IDbSetting
    {
        public string Url { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
