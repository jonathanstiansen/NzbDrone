﻿// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using AutoMoq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Model.Notification;
using NzbDrone.Core.Providers;
using NzbDrone.Core.Providers.Jobs;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.JobTests
{ 
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class ImportNewSeriesJobTest : TestBase
    {
        [Test]
        public void import_new_series_succesfull()
        {
            var series = Builder<Series>.CreateListOfSize(2)
                     .All().With(s => s.LastInfoSync = null)
                     .TheFirst(1).With(s => s.SeriesId = 12)
                     .TheNext(1).With(s => s.SeriesId = 15)
                        .Build();

            var notification = new ProgressNotification("Test");

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<SeriesProvider>()
                .Setup(p => p.GetAllSeries())
                .Returns(series);


            mocker.GetMock<DiskScanJob>()
                .Setup(j => j.Start(notification, series[0].SeriesId, 0))
                .Callback(() => series[0].LastDiskSync = DateTime.Now);


            mocker.GetMock<DiskScanJob>()
                .Setup(j => j.Start(notification, series[1].SeriesId, 0))
                .Callback(() => series[1].LastDiskSync = DateTime.Now);

            mocker.GetMock<BannerDownloadJob>()
                .Setup(j => j.Start(notification, It.IsAny<int>(), 0));

            mocker.GetMock<UpdateInfoJob>()
                .Setup(j => j.Start(notification, series[0].SeriesId, 0))
                .Callback(() => series[0].LastInfoSync = DateTime.Now);

            mocker.GetMock<UpdateInfoJob>()
                .Setup(j => j.Start(notification, series[1].SeriesId, 0))
                .Callback(() => series[1].LastInfoSync = DateTime.Now);

            mocker.GetMock<SeriesProvider>()
                .Setup(s => s.GetSeries(series[0].SeriesId)).Returns(series[0]);

            mocker.GetMock<SeriesProvider>()
                .Setup(s => s.GetSeries(series[1].SeriesId)).Returns(series[1]);

            mocker.GetMock<MediaFileProvider>()
                .Setup(s => s.GetSeriesFiles(It.IsAny<int>())).Returns(new List<EpisodeFile>());

            //Act
            mocker.Resolve<ImportNewSeriesJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();

            mocker.GetMock<DiskScanJob>().Verify(j => j.Start(notification, series[0].SeriesId, 0), Times.Once());
            mocker.GetMock<DiskScanJob>().Verify(j => j.Start(notification, series[1].SeriesId, 0), Times.Once());

            mocker.GetMock<UpdateInfoJob>().Verify(j => j.Start(notification, series[0].SeriesId, 0), Times.Once());
            mocker.GetMock<UpdateInfoJob>().Verify(j => j.Start(notification, series[1].SeriesId, 0), Times.Once());

        }




        [Test]
        [Timeout(3000)]
        public void failed_import_should_not_be_stuck_in_loop()
        {
            var series = Builder<Series>.CreateListOfSize(2)
                     .All().With(s => s.LastInfoSync = null)
                     .TheFirst(1).With(s => s.SeriesId = 12)
                     .TheNext(1).With(s => s.SeriesId = 15)
                        .Build();

            var notification = new ProgressNotification("Test");

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<SeriesProvider>()
                .Setup(p => p.GetAllSeries())
                .Returns(series);

            mocker.GetMock<UpdateInfoJob>()
                .Setup(j => j.Start(notification, series[0].SeriesId, 0))
                .Callback(() => series[0].LastInfoSync = DateTime.Now);

            mocker.GetMock<UpdateInfoJob>()
                .Setup(j => j.Start(notification, series[1].SeriesId, 0))
                .Throws(new InvalidOperationException());

            mocker.GetMock<DiskScanJob>()
                .Setup(j => j.Start(notification, series[0].SeriesId, 0))
                .Callback(() => series[0].LastDiskSync = DateTime.Now);

            mocker.GetMock<BannerDownloadJob>()
                .Setup(j => j.Start(notification, series[0].SeriesId, 0));

            mocker.GetMock<SeriesProvider>()
                .Setup(s => s.GetSeries(series[0].SeriesId)).Returns(series[0]);

            mocker.GetMock<MediaFileProvider>()
                .Setup(s => s.GetSeriesFiles(It.IsAny<int>())).Returns(new List<EpisodeFile>());

            //Act
            mocker.Resolve<ImportNewSeriesJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();

            mocker.GetMock<UpdateInfoJob>().Verify(j => j.Start(notification, series[0].SeriesId, 0), Times.Once());
            mocker.GetMock<UpdateInfoJob>().Verify(j => j.Start(notification, series[1].SeriesId, 0), Times.Once());

            mocker.GetMock<DiskScanJob>().Verify(j => j.Start(notification, series[0].SeriesId, 0), Times.Once());

            ExceptionVerification.ExcpectedErrors(1);

        }



        [Test]
        public void AutoIgnoreSeason_new_series_should_not_ignore_any()
        {
            int seriesId = 12;

            var mocker = new AutoMoqer(MockBehavior.Strict);
            mocker.GetMock<MediaFileProvider>()
                .Setup(p => p.GetSeriesFiles(seriesId))
                .Returns(new List<EpisodeFile>());

            mocker.GetMock<EpisodeProvider>()
                .Setup(p => p.GetSeasons(seriesId))
                .Returns(new List<int> { 0, 1, 2, 3, 4 });

            mocker.Resolve<ImportNewSeriesJob>().AutoIgnoreSeasons(seriesId);


            mocker.GetMock<EpisodeProvider>().Verify(p => p.SetSeasonIgnore(seriesId, It.IsAny<int>(), It.IsAny<Boolean>()), Times.Never());
        }

        [Test]
        public void AutoIgnoreSeason_existing_should_not_ignore_currentseason()
        {
            int seriesId = 12;

            var episodesFiles = Builder<EpisodeFile>.CreateListOfSize(2)
            .All().With(e => e.SeriesId = seriesId)
            .Build();

            episodesFiles[0].SeasonNumber = 0;
            episodesFiles[1].SeasonNumber = 1;

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<MediaFileProvider>()
                .Setup(p => p.GetSeriesFiles(seriesId))
                .Returns(episodesFiles);

            mocker.GetMock<EpisodeProvider>()
                .Setup(p => p.GetSeasons(seriesId))
                .Returns(new List<int> { 0, 1, 2 });

            mocker.Resolve<ImportNewSeriesJob>().AutoIgnoreSeasons(seriesId);

            mocker.GetMock<EpisodeProvider>().Verify(p => p.SetSeasonIgnore(seriesId, 2, It.IsAny<Boolean>()), Times.Never());
        }

        [Test]
        public void AutoIgnoreSeason_existing_should_ignore_seasons_with_no_file()
        {
            int seriesId = 12;

            var episodesFiles = Builder<EpisodeFile>.CreateListOfSize(2)
            .All().With(e => e.SeriesId = seriesId)
            .Build();

            episodesFiles[0].SeasonNumber = 1;

            var mocker = new AutoMoqer();

            mocker.GetMock<MediaFileProvider>()
                .Setup(p => p.GetSeriesFiles(seriesId))
                .Returns(episodesFiles);

            mocker.GetMock<EpisodeProvider>()
                .Setup(p => p.GetSeasons(seriesId))
                .Returns(new List<int> { 0, 1, 2 });

            mocker.Resolve<ImportNewSeriesJob>().AutoIgnoreSeasons(seriesId);

            mocker.GetMock<EpisodeProvider>().Verify(p => p.SetSeasonIgnore(seriesId, 0, true), Times.Once());
            mocker.GetMock<EpisodeProvider>().Verify(p => p.SetSeasonIgnore(seriesId, 1, true), Times.Never());
            mocker.GetMock<EpisodeProvider>().Verify(p => p.SetSeasonIgnore(seriesId, 2, It.IsAny<Boolean>()), Times.Never());
        }
    }


}