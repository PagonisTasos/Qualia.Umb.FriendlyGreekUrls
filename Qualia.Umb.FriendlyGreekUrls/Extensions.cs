using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Qualia.Umb.FriendlyGreekUrls
{
    internal static class Extensions
    {
        private const string UMB_URL_NAME = "umbracoUrlName";

        public static bool IsCultureInvariant(this Umbraco.Cms.Core.Models.IContent content)
        {
            return content.CultureInfos.Count == 0;
        }

        /// <summary>
        ///     Checks if the IContent has an umbracoUrlName property.
        /// </summary>
        /// <param name="content">The icontent to check.</param>
        /// <returns></returns>
        public static bool SupportsCustomUrl(this Umbraco.Cms.Core.Models.IContent content)
        {
            return content.Properties.Any(prop => prop.Alias.Equals(UMB_URL_NAME));
        }

        /// <summary>
        ///     Checks if the icontent has a non-empty umbracoUrlName value.
        /// </summary>
        /// <param name="content">The icontent to check.</param>
        /// <param name="cultureIso">The culture to check. (Set cultureIso to null if the IContent is culture invariant)</param>
        /// <returns></returns>
        public static bool HasCustomUrl(this Umbraco.Cms.Core.Models.IContent content, string? cultureIso)
        {
            var customUrl = content
                            .Properties
                            .FirstOrDefault(prop => prop.Alias.Equals(UMB_URL_NAME))?
                            .Values
                            .FirstOrDefault(v => v.Culture == cultureIso?.ToLower())?
                            .EditedValue?
                            .ToString()
                            ;

            return !String.IsNullOrEmpty(customUrl);
        }

        /// <summary>
        ///     Sets the umbracoUrlName property to the specified value.
        /// </summary>
        /// <param name="this">the @this to act on.</param>
        /// <param name="url">the desired url value</param>
        /// <param name="cultureIso">The culture to act upon. (Set cultureIso to null if the IContent is culture invariant)</param>
        public static void SetCustomUrl(this Umbraco.Cms.Core.Models.IContent @this, string? url, string? cultureIso)
        {
            @this.SetValue(UMB_URL_NAME, url, cultureIso?.ToLower());
        }

        /// <summary>
        ///     A string extension method that removes the diacritics characters from strings.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The string without diacritics.</returns>
        public static string RemoveDiacritics(this string @this)
        {
            string normalizedString = @this.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char t in normalizedString)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        ///     Concatenates all the elements of a IEnumerable, using the specified separator between each element.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">An IEnumerable that contains the elements to concatenate.</param>
        /// <param name="separator">
        ///     The string to use as a separator. separator is included in the returned string only if
        ///     value has more than one element.
        /// </param>
        /// <returns>
        ///     A string that consists of the elements in value delimited by the separator string. If value is an empty array,
        ///     the method returns String.Empty.
        /// </returns>
        public static string StringJoin<T>(this IEnumerable<T> @this, string separator)
        {
            return string.Join(separator, @this);
        }

        /// <summary>
        ///     Concatenates all the elements of a IEnumerable, using the specified separator between
        ///     each element.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="separator">
        ///     The string to use as a separator. separator is included in the
        ///     returned string only if value has more than one element.
        /// </param>
        /// <returns>
        ///     A string that consists of the elements in value delimited by the separator string. If
        ///     value is an empty array, the method returns String.Empty.
        /// </returns>
        public static string StringJoin<T>(this IEnumerable<T> @this, char separator)
        {
            return string.Join(separator.ToString(), @this);
        }

        /// <summary>
        ///     Removes all diacritics, then replaces greek letters in string with greeklish equivalent eng chars.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A string without diacretics and with its greek chars replaced by eng chars.</returns>
        public static string GreekToEngChars(this string @this)
        {
            var gr = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψω";
            var toEng = "ABGDEZI8IKLMNJOPRSTUFXYWabgdezi8iklmnjoprstufxyw";

            return
                @this
                .RemoveDiacritics()
                .Select(@char => gr.Contains(@char) ? toEng[gr.IndexOf(@char)] : @char)
                .StringJoin("");
            ;
        }

        /// <summary>
        ///     Replaces spaces with dashes and filters out all letters but eng, digits and underscore.
        /// </summary>
        /// <param name="this"></param>
        /// <returns>A string composed of eng chars, digits, - and _</returns>
        public static string ToFriendlyUrl(this string @this)
        {
            var allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_";

            return
                @this
                .Replace(" ", "-")
                .Where(@char => allowed.IndexOf(@char) > -1)
                .Where((@char, index) =>
                {
                //filter out multiple consecutive dashes or underscores
                var isDash = @char != '-';
                    var isUnderscore = @char != '_';
                    var nextCharExists = @this.Length > index + 1;
                    return !isDash && !isUnderscore || !nextCharExists || @this[index] != @this[index + 1];
                })
                .StringJoin("")
                ;
        }
    }

}
