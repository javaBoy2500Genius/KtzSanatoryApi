namespace KTZSanatoriumServiceApi.Data.Repository
{
    public interface IDbSetting
    {
        string Url { get; set; }
        string DatabaseName { get; set; }
    }
}
