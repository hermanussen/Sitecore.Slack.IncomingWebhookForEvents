namespace Sitecore.Slack.IncomingWebhookForEvents.Slack
{
    using Newtonsoft.Json;

    /// <summary>
    /// JSON attachment object as prescribed by Slack: https://api.slack.com/docs/attachments
    /// </summary>
    public class Attachment
    {
        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author_link")]
        public string AuthorLink { get; set; }
    }
}
