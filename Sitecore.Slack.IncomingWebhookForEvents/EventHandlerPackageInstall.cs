namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diagnostics;
    using Events;
    using Install.Events;

    /// <summary>
    /// Handler for package install start and end events.
    /// </summary>
    public class EventHandlerPackageInstall : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            if (!IsEnabled)
            {
                return;
            }

            Assert.ArgumentNotNull(args, "args");

            InstallationEventArgs installArgs = Event.ExtractParameter(args, 0) as InstallationEventArgs;

            if (installArgs == null)
            {
                return;
            }

            var tagValues = new Dictionary<string, string>();

            this.AppendUserInfo(tagValues);

            tagValues.Add("package_item_count", installArgs.ItemsToInstall != null ? installArgs.ItemsToInstall.Count().ToString() : string.Empty);
            tagValues.Add("package_files_count", installArgs.FilesToInstall != null ? installArgs.FilesToInstall.Count().ToString() : string.Empty);

            this.SendMessageToSlack(tagValues);
        }
    }
}
