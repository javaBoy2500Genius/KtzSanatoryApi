
using Newtonsoft.Json;

namespace KTZSanatoriumServiceApi.ModelsDto
{
    public class Authorization
    {
        [JsonProperty("login")]
        public string Login { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}

