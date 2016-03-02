namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Data.Items;
    using Diagnostics;
    using Events;

    /// <summary>
    /// Handler that sends a message to Slack when an item is deleted.
    /// </summary>
    public class EventHandlerItemDeleted : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            if (!IsEnabled)
            {
                return;
            }

            Assert.ArgumentNotNull(args, "args");

            Item target = Event.ExtractParameter(args, 0) as Item;
            ID parentId = Event.ExtractParameter(args, 1) as ID;

            if (target == null || parentId == null as ID || parentId.IsNull)
            {
                return; 
            }

            Item parent = target.Database.GetItem(parentId);
            if (parent == null)
            {
                return;
            }

            if (!MatchesPathIncludes(parent))
            {
                return;
            }

            var tagValues = new Dictionary<string, string>();

            this.AppendUserInfo(tagValues);

            tagValues.Add("item_path", string.Concat(parent.Paths.FullPath, "/", target.Name));
            tagValues.Add("item_id", target.ID.ToString());

            this.SendMessageToSlack(tagValues);
        }
    }
}
