namespace KTZSanatoriumServiceApi.Services
{
    public interface IUserService
    {
        string Encrypt(string plaintext);
        
        string Decrypt (string plaintext);


        string CreateToken(ITokenSetting tokenSetting);

        string RefreshToken(string token);
        string GetClaimValue(UserClaimEnum userClaimEnum);

    }
}
