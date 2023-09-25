using Newtonsoft.Json;

namespace SimpleReddit.AuthTokenLib.EventArgs
{
    [Serializable]
    public class AuthSuccessEventArgs
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
