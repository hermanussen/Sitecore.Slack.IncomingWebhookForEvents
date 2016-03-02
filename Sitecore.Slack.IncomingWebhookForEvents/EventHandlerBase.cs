namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Data.Items;
    using Diagnostics;
    using Newtonsoft.Json;
    using Slack;
    using Web;

    /// <summary>
    /// Base class for handling events that will be sent to Slack.
    /// </summary>
    public abstract class EventHandlerBase
    {
        /// <summary>
        /// Formattable message that will be sent to slack. Text is injected from config file.
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// Incoming webhook url. Check the Slack integration for the correct url and enter it in the config file.
        /// </summary>
        private string SlackBaseUrl { get; set; }

        protected EventHandlerBase()
        {
            if (!IsEnabled)
            {
                return;
            }

            const string settingName = "Sitecore.Slack.IncomingWebhookForEvents.BaseUrl";
            this.SlackBaseUrl = Configuration.Settings.GetSetting(settingName);
            Assert.IsNotNullOrEmpty(
                this.SlackBaseUrl,
                "Setting {0} was not configured in the .config files",
                new object[] { settingName });
        }

        /// <summary>
        /// Override this in specific implementations to handle fired events in a consistent way.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public abstract void OnEventFired(object sender, EventArgs args);

        /// <summary>
        /// Actually sends the message with a JSON payload to the Slack incoming webhook url.
        /// </summary>
        /// <param name="tagValues"></param>
        protected void SendMessageToSlack(Dictionary<string,string> tagValues)
        {
            string message = tagValues.Aggregate(
                this.MessageTemplate,
                (current, tag) => current.Replace(string.Concat("{", tag.Key, "}"), tag.Value));
            try
            {
                using (var wc = new WebClient())
                {
                    // Create the JSON
                    string body = JsonConvert.SerializeObject(new Payload()
                        {
                            Text = message,
                            IconEmoji = Configuration.Settings.GetSetting("Sitecore.Slack.IncomingWebhookForEvents.IconEmoji", string.Empty),
                            Username = Configuration.Settings.GetSetting("Sitecore.Slack.IncomingWebhookForEvents.UserName", string.Empty),
                            Attachments = new []
                                {
                                    new Attachment()
                                        {
                                            AuthorName = string.Format("{0} ({1})", GetUserDisplayName(), Context.GetUserName()),
                                            AuthorLink = string.Concat(
                                                WebUtil.GetFullUrl("/sitecore/shell/~/xaml/Sitecore.Shell.Applications.Security.EditUser.aspx"),
                                                "?us=",
                                                WebUtil.UrlEncode(Context.GetUserName()))
                                        }
                                }
                        });
                    
                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = body;

                    // Send the JSON to the incoming webhook url from Slack
                    var response = wc.UploadValues(this.SlackBaseUrl, "POST", data);

                    //The response text is usually "ok"
                    string responseText = Encoding.UTF8.GetString(response);
                }
            }
            catch (WebException ex)
            {
                Log.Error(string.Format("Unable to post to {0}: {1}", this.SlackBaseUrl, ex.Message), ex, this);
            }
        }

        /// <summary>
        /// Adds tag values for user data.
        /// </summary>
        /// <param name="tagValues"></param>
        protected void AppendUserInfo(Dictionary<string, string> tagValues)
        {
            tagValues.Add("user", Context.GetUserName());
            tagValues.Add("user_display_name", GetUserDisplayName());
        }

        /// <summary>
        /// Returns the current user's display name, or user name (if no display name is available)
        /// </summary>
        /// <returns></returns>
        private static string GetUserDisplayName()
        {
            return StringUtil.GetString(
                Context.User.Profile != null ? Context.User.Profile.FullName : null,
                Context.GetUserName(),
                "unknown");
        }

        /// <summary>
        /// Adds tag values for item data.
        /// </summary>
        /// <param name="tagValues"></param>
        /// <param name="target"></param>
        /// <param name="number">1 or 2, the latter of which will add a postfix to the tag keys</param>
        protected void AppendItemInfo(Dictionary<string, string> tagValues, Item target, int number)
        {
            string numberPostfix = number > 1 ? string.Format("_{0}", number) : string.Empty;

            tagValues.Add(string.Concat("item_name", numberPostfix), target.Name);
            tagValues.Add(string.Concat("item_path", numberPostfix), target.Paths.FullPath);
            tagValues.Add(string.Concat("item_id", numberPostfix), target.ID.ToString());
            tagValues.Add(string.Concat("item_edit_url", numberPostfix), this.GetCmsOpenUrl(target));
        }

        /// <summary>
        /// Returns a url that links directly to the item in the content editor of the CMS.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected string GetCmsOpenUrl(Item target)
        {
            return string.Concat(
                WebUtil.GetFullUrl("/sitecore/shell/sitecore/content/Applications/Content%20Editor.aspx"),
                "?id=",
                target.ID.ToString(),
                "&la=",
                target.Language.Name,
                "&fo=",
                target.ID.ToString());
        }

        /// <summary>
        /// Returns true if the item that is passed matches the setting defined in the config file.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool MatchesPathIncludes(Item target)
        {
            string[] pathIncludes =
                (Configuration.Settings.GetSetting("Sitecore.Slack.IncomingWebhookForEvents.ItemBasedPathIncludes") ?? string.Empty)
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return pathIncludes.Any(p => target.Paths.FullPath
                .StartsWith(p, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets a value indicating whether the module is enabled.
        /// </summary>
        protected bool IsEnabled
        {
            get
            {
                const string enabledSettingName = "Sitecore.Slack.IncomingWebhookForEvents.Enabled";
                return Configuration.Settings.GetBoolSetting(enabledSettingName, false);
            }
        }
    }
}