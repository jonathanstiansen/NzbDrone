using System;
using System.Collections.Generic;
using NzbDrone.Core.Download;
using NzbDrone.Core.Download.Clients.Nzbget;
using NzbDrone.Core.Download.Clients.Sabnzbd;

namespace NzbDrone.Core.Configuration
{
    public interface IConfigService
    {
        IEnumerable<Config> All();
        Dictionary<String, Object> AllWithDefaults();
        string UpdateUrl { get; set; }
        String SabHost { get; set; }
        int SabPort { get; set; }
        String SabApiKey { get; set; }
        String SabUsername { get; set; }
        String SabPassword { get; set; }
        String SabTvCategory { get; set; }
        SabPriorityType SabBacklogTvPriority { get; set; }
        SabPriorityType SabRecentTvPriority { get; set; }
        String DownloadedEpisodesFolder { get; set; }
        bool UseSeasonFolder { get; set; }
        string SortingSeasonFolderFormat { get; set; }
        int DefaultQualityProfile { get; set; }
        string TwitterAccessToken { get; set; }
        string TwitterAccessTokenSecret { get; set; }
        bool EnableBacklogSearching { get; set; }
        bool AutoIgnorePreviouslyDownloadedEpisodes { get; set; }
        int Retention { get; set; }
        Guid UGuid { get; }
        DownloadClientType DownloadClient { get; set; }
        string BlackholeDirectory { get; set; }
        string ServiceRootUrl { get; }
        Boolean MetadataUseBanners { get; set; }
        string PneumaticDirectory { get; set; }
        string RecycleBin { get; set; }
        int RssSyncInterval { get; set; }
        Boolean IgnoreArticlesWhenSortingSeries { get; set; }
        String NzbgetUsername { get; set; }
        String NzbgetPassword { get; set; }
        String NzbgetHost { get; set; }
        Int32 NzbgetPort { get; set; }
        String NzbgetTvCategory { get; set; }
        Int32 NzbgetPriority { get; set; }
        PriorityType NzbgetBacklogTvPriority { get; set; }
        PriorityType NzbgetRecentTvPriority { get; set; }
        string NzbRestrictions { get; set; }
        string AllowedReleaseGroups { get; set; }
        string GetValue(string key, object defaultValue, bool persist = false);
        void SetValue(string key, string value);
        void SaveValues(Dictionary<string, object> configValues);
    }
}