using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace Qualia.Umb.FriendlyGreekUrls
{
    internal class FriendlyGreekUrlHandler : INotificationHandler<ContentSavingNotification>
    {
        private readonly ILocalizationService localizationService;

        public FriendlyGreekUrlHandler(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        /// <summary>
        ///     If the content node being saved 
        ///     has the "url name" property, 
        ///     and is left empty by the backoffice editor, 
        ///     it is populated using the node's name converted to english chars
        /// </summary>
        /// <param name="notification"></param>
        public void Handle(ContentSavingNotification notification)
        {
            foreach (Umbraco.Cms.Core.Models.IContent content in notification.SavedEntities)
            {
                if (content.IsCultureInvariant() && content.SupportsCustomUrl() && !content.HasCustomUrl(null))
                {
                    var nodeName = content.Name;
                    var friendlyUrl = nodeName?.GreekToEngChars()?.ToFriendlyUrl();
                    content.SetCustomUrl(friendlyUrl, null);
                }
                else
                {
                    foreach (Umbraco.Cms.Core.Models.ILanguage lang in localizationService.GetAllLanguages())
                    {
                        if (notification.IsSavingCulture(content, lang.IsoCode) && content.SupportsCustomUrl() && !content.HasCustomUrl(lang.IsoCode))
                        {
                            var nodeName = content.CultureInfos[lang.IsoCode].Name;
                            var friendlyUrl = nodeName?.GreekToEngChars()?.ToFriendlyUrl();
                            content.SetCustomUrl(friendlyUrl, lang.IsoCode);
                        }
                    }
                }

            }
        }
    }

}
