namespace Sitecore.Slack.IncomingWebhookForEvents.Slack
{
    using Newtonsoft.Json;

    /// <summary>
    /// JSON payload object as prescribed by Slack: https://api.slack.com/incoming-webhooks
    /// </summary>
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("icon_emoji")]
        public string IconEmoji { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
    }
}
