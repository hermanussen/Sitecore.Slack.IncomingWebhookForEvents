namespace Sitecore.Slack.IncomingWebhookForEvents
{
    using System;
    using System.Collections.Generic;
    using Diagnostics;
    using Events;
    using Publishing;

    /// <summary>
    /// Eventhandler for dealing with publish start and end events.
    /// </summary>
    public class EventHandlerPublish : EventHandlerBase
    {
        public override void OnEventFired(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            Publisher publisher = Event.ExtractParameter(args, 0) as Publisher;

            if (publisher == null || publisher.Options == null || publisher.Options.RootItem == null)
            {
                return;
            }

            if (!MatchesPathIncludes(publisher.Options.RootItem))
            {
                return;
            }

            var tagValues = new Dictionary<string, string>();

            this.AppendUserInfo(tagValues);

            this.AppendItemInfo(tagValues, publisher.Options.RootItem, 1);

            tagValues.Add("publish_mode", Enum.GetName(typeof(PublishMode), publisher.Options.Mode));
            tagValues.Add("publish_descendants", publisher.Options.Deep ? "including descendants" : "excluding descendants");
            tagValues.Add("publish_related_items", publisher.Options.PublishRelatedItems ? "including related items" : "excluding related items");
            tagValues.Add("publish_language", StringUtil.GetString(publisher.Options.Language.GetDisplayName(), publisher.Options.Language.Name));
            tagValues.Add("publish_source", publisher.Options.SourceDatabase.Name);
            tagValues.Add("publish_target", publisher.Options.TargetDatabase.Name);
            
            this.SendMessageToSlack(tagValues);
        }
    }
}
