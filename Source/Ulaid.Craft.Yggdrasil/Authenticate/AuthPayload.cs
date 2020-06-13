using Newtonsoft.Json;

namespace Ulaid.Craft.Yggdrasil.Authenticate
{

    public partial class AuthPayload
    {
        [JsonProperty("agent")]
        public Agent Agent { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }

        [JsonProperty("requestUser")]
        public bool RequestUser { get; set; }
    }

    public partial class Agent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }
    }
}
