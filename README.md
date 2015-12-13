# Sitecore.Slack.IncomingWebhookForEvents
Send a message from Sitecore to a channel on Slack when users change an item, publish or install a package.

## Install with NuGet

You can install the package via [NuGet](https://www.nuget.org/packages/Sitecore.Slack.IncomingWebhookForEvents/):

Install-Package Sitecore.Slack.IncomingWebhookForEvents

After installation...
* Add an incoming webhook integration for your Slack team (https://api.slack.com/incoming-webhooks)
* Locate the file /App_Config/Include/Sitecore.Slack.IncomingWebhookForEvents.Config and change the value of the setting Sitecore.Slack.IncomingWebhookForEvents.BaseUrl (you can get the url from the integration)

To test, try and publish a single item. You should see a message in the configured channel in Slack about it.