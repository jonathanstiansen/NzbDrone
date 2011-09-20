﻿using System;
using System.Collections.Generic;
using AutoMoq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Model;
using NzbDrone.Core.Model.Notification;
using NzbDrone.Core.Providers;
using NzbDrone.Core.Providers.Indexer;
using NzbDrone.Core.Providers.Jobs;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Repository.Quality;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class SeriesSearchJobTest : TestBase
    {
        [Test]
        public void SeriesSearch_success()
        {
            var seasons = new List<int> { 1, 2, 3, 4, 5 };

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Series Search");

            mocker.GetMock<EpisodeProvider>()
                .Setup(c => c.GetSeasons(1)).Returns(seasons);

            mocker.GetMock<EpisodeProvider>()
                .Setup(c => c.IsIgnored(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            mocker.GetMock<SeasonSearchJob>()
                .Setup(c => c.Start(notification, 1, It.IsAny<int>())).Verifiable();

            //Act
            mocker.Resolve<SeriesSearchJob>().Start(notification, 1, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, 1, It.IsAny<int>()),
                                                       Times.Exactly(seasons.Count));
        }

        [Test]
        public void SeriesSearch_no_seasons()
        {
            var seasons = new List<int>();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Series Search");

            mocker.GetMock<EpisodeProvider>()
                .Setup(c => c.GetSeasons(1)).Returns(seasons);

            //Act
            mocker.Resolve<SeriesSearchJob>().Start(notification, 1, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, 1, It.IsAny<int>()),
                                                       Times.Never());
        }
    }
}