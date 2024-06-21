
using KTZSanatoriumServiceApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KTZSanatoriumServiceApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserSetting _userSetting;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IUserSetting userSetting, IHttpContextAccessor contextAccessor)
        {
            _userSetting = userSetting;
            _contextAccessor = contextAccessor;
        }

        public string CreateToken(ITokenSetting tokenSetting)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_userSetting.SecretKey));
                var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claimsForToken = new List<Claim>();


                foreach (var prop in tokenSetting.GetType().GetProperties())
                {
                    var value = prop.GetValue(tokenSetting)?.ToString();
                    if (value != null && 
                        typeof(UserClaimEnum).GetEnumNames().Any(x => x == prop.Name))
                    {
                        claimsForToken.Add(new Claim(prop.Name, value.ToString()));
                    }
                }
                claimsForToken.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, tokenSetting.UserRole.ToString()));

                var jwtSecurityToken = new JwtSecurityToken(
                claims: claimsForToken,
                audience: tokenSetting.UserId,
                notBefore: DateTime.UtcNow,
                expires: tokenSetting.TokenLifeTime,
                signingCredentials: signingCredential);

                var token = new JwtSecurityTokenHandler()
                    .WriteToken(jwtSecurityToken);

                return token;

            }
            catch { throw; }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {

                    string[] parts = cipherText.Split(':');
                    byte[] iv = Convert.FromBase64String(parts[0]);
                    aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_userSetting.Key);
                    aesAlg.IV = iv;

                    byte[] cipherBytes = Convert.FromBase64String(parts[1]);
                    ICryptoTransform cryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream ms = new MemoryStream(cipherBytes, 0, cipherBytes.Length))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(csDecrypt))
                            {
                                return sr.ReadToEnd();
                            }

                        }

                    }
                }
            }
            catch { throw; }
        }

        public string Encrypt(string password)
        {
            try
            {
                using (Aes alg = Aes.Create())
                {
                    // Генерация случайного IV
                    byte[] iv = new byte[alg.BlockSize / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(iv);
                    }

                    alg.Key = System.Text.Encoding.UTF8.GetBytes(_userSetting.Key);
                    alg.IV = iv;

                    ICryptoTransform cryptoTransform = alg.CreateEncryptor(alg.Key, alg.IV);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cs))
                            {
                                streamWriter.Write(password);
                            }
                        }
                        // Возвращаем IV, зашифрованные данные и их длину в формате, который позволит нам разделить их при дешифровании
                        return Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        



        public string RefreshToken(string refreshToken)
        {
            try
            {
                var principal = GetPrincipal(refreshToken);
                Enum.TryParse<UserRole>(principal.Claims.FirstOrDefault(c => c.Type == UserClaimEnum.UserRole.ToString())?.Value, out var res);
                var token = CreateToken(
                  
                  new TokenSetiing
                  {
                      UserId = principal.Claims.FirstOrDefault(c => c.Type == UserClaimEnum.UserId.ToString())?.Value ?? string.Empty,
                      UserRole = res,
                      TokenLifeTime = DateTime.UtcNow.AddDays(1)
                  }) ;
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetClaimValue(UserClaimEnum userClaimEnum)
        {
            try
            {
                if (_contextAccessor.HttpContext is not null)
                    return _contextAccessor.HttpContext.User.FindFirstValue(userClaimEnum.ToString()) ?? string.Empty;

                return string.Empty;
            }
            catch { throw; }
        }









        private ClaimsPrincipal GetPrincipal(string token)
        {
            SecurityToken validatedToken;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_userSetting.SecretKey)),
                ValidateLifetime = false
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out validatedToken);

            return principal;
        }



    }

    public enum UserClaimEnum
    {
        UserId,
        UserRole,

    }







}
