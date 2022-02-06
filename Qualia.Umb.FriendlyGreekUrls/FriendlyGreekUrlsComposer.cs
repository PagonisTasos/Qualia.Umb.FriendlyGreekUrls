using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Qualia.Umb.FriendlyGreekUrls
{
    public class FriendlyGreekUrlsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentSavingNotification, FriendlyGreekUrlHandler>();
        }
    }
}
