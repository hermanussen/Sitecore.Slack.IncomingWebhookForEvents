namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Data.Items;
    using Diagnostics;
    using Events;

    /// <summary>
    /// Handler that sends a message saying which item has been moved.
    /// </summary>
    public class EventHandlerItemMoved : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            Item target = Event.ExtractParameter(args, 0) as Item;
            ID previousParentId = Event.ExtractParameter(args, 1) as ID;

            if (target == null || previousParentId == null as ID || previousParentId.IsNull)
            {
                return;
            }

            Item previousParent = target.Database.GetItem(previousParentId);
            if (previousParent == null)
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
            this.AppendItemInfo(tagValues, previousParent, 2);

            this.SendMessageToSlack(tagValues);
        }
    }
}
