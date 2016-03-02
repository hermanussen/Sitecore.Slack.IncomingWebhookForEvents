namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using Data.Items;
    using Diagnostics;
    using Events;
    
    /// <summary>
    /// Handler that is triggered when an item is renamed.
    /// </summary>
    public class EventHandlerItemRenamed : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            if (!IsEnabled)
            {
                return;
            }

            Assert.ArgumentNotNull(args, "args");

            Item target = Event.ExtractParameter(args, 0) as Item;
            string previousName = Event.ExtractParameter(args, 1) as string;

            if (target == null || string.IsNullOrWhiteSpace(previousName))
            {
                return;
            }

            if (!MatchesPathIncludes(target))
            {
                return;
            }

            var tagValues = new Dictionary<string, string>();

            this.AppendUserInfo(tagValues);
            this.AppendItemInfo(tagValues, target, 1);
            tagValues.Add("item_name_2", previousName);

            this.SendMessageToSlack(tagValues);
        }
    }
}
