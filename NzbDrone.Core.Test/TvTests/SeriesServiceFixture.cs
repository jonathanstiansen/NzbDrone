﻿using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.RootFolders;
using NzbDrone.Core.Tv;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Tv.Events;

namespace NzbDrone.Core.Test.TvTests
{
    [TestFixture]
    public class SeriesServiceFixture : CoreTest<SeriesService>
    {
        private Mock<ISeriesRepository> Repo;

        private Series fakeSeries;

        [SetUp]
        public void Setup()
        {
            Repo = Mocker.GetMock<ISeriesRepository>();
            fakeSeries = Builder<Series>.CreateNew().Build();
        }

        [Test]
        public void series_added_event_should_have_proper_path()
        {
            Mocker.GetMock<IRootFolderService>().Setup(c => c.Get(It.IsAny<int>()))
                .Returns(new RootFolder { Path = "c:\\root" });

            var series = Subject.AddSeries(fakeSeries);

            series.RootFolder.Should().NotBeNull();
            series.RootFolder.Value.Should().NotBeNull();

            VerifyEventPublished<SeriesAddedEvent>();
        }

        [Test]
        public void is_monitored()
        {
            Repo.Setup(c => c.Get(12))
                .Returns(fakeSeries);

            fakeSeries.Monitored = true;

            Subject.IsMonitored(12).Should().Be(true);
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             