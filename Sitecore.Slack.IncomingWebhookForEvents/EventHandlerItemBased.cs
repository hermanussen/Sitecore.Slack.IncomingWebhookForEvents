namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using Data.Items;
    using Diagnostics;
    using Events;

    /// <summary>
    /// Handler that can deal with various item operations.
    /// Any other item related events can be handled by more specific handlers.
    /// </summary>
    public class EventHandlerItemBased : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            object[] parameters = Event.ExtractParameters(args);

            Item target = parameters.Length > 0 ? parameters[0] as Item : null;
            if (target == null)
            {
                return;
            }

            Item target2 = parameters.Length > 1 ? parameters[1] as Item : null;
            
            if (! MatchesPathIncludes(target))
            {
                return;
            }

            var tagValues = new Dictionary<string, string>();
            
            this.AppendUserInfo(tagValues);
            
            this.AppendItemInfo(tagValues, target, 1);
            
            if (target2 != null)
            {
                AppendItemInfo(tagValues, target2, 2);
            }

            this.SendMessageToSlack(tagValues);
        }
    }
}