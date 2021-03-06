﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.Test.MetadataSourceTests
{
    [TestFixture]
    public class TraktProxyFixture : CoreTest<TraktProxy>
    {
        [TestCase("The Simpsons")]
        [TestCase("South Park")]
        [TestCase("Franklin & Bash")]
        public void successful_search(string title)
        {
            var result = Subject.SearchForNewSeries(title);

            result.Should().NotBeEmpty();

            result[0].Title.Should().Be(title);
        }


        [Test]
        public void no_search_result()
        {
            var result = Subject.SearchForNewSeries(Guid.NewGuid().ToString());
            result.Should().BeEmpty();
        }

        [Test]
        public void should_be_able_to_get_series_detail()
        {
            var details = Subject.GetSeriesInfo(75978);

            ValidateSeries(details);
        }

        [Test]
        public void should_be_able_to_get_list_of_episodes()
        {
            var details = Subject.GetEpisodeInfo(75978);

            details.Should().NotBeEmpty();

            details.GroupBy(e => e.SeasonNumber.ToString("000") + e.EpisodeNumber.ToString("000"))
                .Max(e => e.Count()).Should().Be(1);

            details.Select(c => c.TvDbEpisodeId).Should().OnlyHaveUniqueItems();

            details.Should().Contain(c => c.SeasonNumber > 0);
            details.Should().Contain(c => !string.IsNullOrWhiteSpace(c.Overview));

            foreach (var episode in details)
            {
                episode.AirDate.Should().HaveValue();
                episode.AirDate.Value.Kind.Should().Be(DateTimeKind.Utc);
                episode.EpisodeNumber.Should().NotBe(0);
                episode.Title.Should().NotBeBlank();
                episode.TvDbEpisodeId.Should().NotBe(0);
            }
        }



        private void ValidateSeries(Series series)
        {
            series.Should().NotBeNull();
            series.Title.Should().NotBeBlank();
            series.Overview.Should().NotBeBlank();
            series.AirTime.Should().NotBeBlank();
            series.FirstAired.Should().HaveValue();
            series.FirstAired.Value.Kind.Should().Be(DateTimeKind.Utc);
            series.Images.Should().NotBeEmpty();
            series.ImdbId.Should().NotBeBlank();
            series.Network.Should().NotBeBlank();
            series.Runtime.Should().BeGreaterThan(0);
            series.TitleSlug.Should().NotBeBlank();
            series.TvRageId.Should().BeGreaterThan(0);
            series.TvdbId.Should().BeGreaterThan(0);
        }
    }
}
