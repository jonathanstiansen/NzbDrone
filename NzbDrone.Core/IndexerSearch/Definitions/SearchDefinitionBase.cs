using System;
using System.Text.RegularExpressions;

namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public abstract class SearchDefinitionBase
    {
        private static readonly Regex NoneWord = new Regex(@"[\W]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BeginningThe = new Regex(@"^the\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public int SeriesId { get; set; }
        public string SceneTitle { get; set; }

        public string QueryTitle
        {
            get
            {
                return GetQueryTitle(SceneTitle);
            }
        }

        private static string GetQueryTitle(string title)
        {
            var cleanTitle = BeginningThe.Replace(title, String.Empty);

            cleanTitle = cleanTitle
                .Replace("&", "and")
                .Replace("`", "")
                .Replace("'", "");

            cleanTitle = NoneWord.Replace(cleanTitle, "+");

            //remove any repeating +s
            cleanTitle = Regex.Replace(cleanTitle, @"\+{2,}", "+");
            return cleanTitle.Trim('+', ' ');
        }
    }
}