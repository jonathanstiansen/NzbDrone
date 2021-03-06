﻿using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Tv;
using NzbDrone.Core.Providers;

using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.ProviderTests
{
    [TestFixture]
    public class MisnamedProviderTest : CoreTest<MisnamedProvider>
    {
        [Test]
        public void no_misnamed_files()
        {

            var series = Builder<Series>.CreateNew()
                .With(s => s.Title = "SeriesTitle")
                .Build();

            var episodeFiles = Builder<EpisodeFile>.CreateListOfSize(2)
                .TheFirst(1)
                .With(f => f.Id = 1)
                .With(f => f.Path = @"C:\Test\Title1.avi")
                .TheNext(1)
                .With(f => f.Id = 2)
                .With(f => f.Path = @"C:\Test\Title2.avi")
                .Build().ToList();

            var episodes = Builder<Episode>.CreateListOfSize(2)
                .All()
                .With(e => e.Series = series)
                .TheFirst(1)
                .With(e => e.EpisodeFile = episodeFiles[0])
                .TheNext(1)
                .With(e => e.EpisodeFile = episodeFiles[1])
                .Build().ToList();



            Mocker.GetMock<IEpisodeService>()
                .Setup(c => c.EpisodesWithFiles()).Returns(episodes);

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[0] }, It.IsAny<Series>(), episodeFiles[0]))
                .Returns("Title1");

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[1] }, It.IsAny<Series>(), episodeFiles[1]))
                .Returns("Title2");


            var totalItems = 0;
            var misnamedEpisodes = Subject.MisnamedFiles(1, 10, out totalItems);


            misnamedEpisodes.Should().HaveCount(0);
        }

        [Test]
        public void all_misnamed_files()
        {

            var series = Builder<Series>.CreateNew()
                .With(s => s.Title = "SeriesTitle")
                .Build();

            var episodeFiles = Builder<EpisodeFile>.CreateListOfSize(2)
                .TheFirst(1)
                .With(f => f.Id = 1)
                .With(f => f.Path = @"C:\Test\Title1.avi")
                .TheNext(1)
                .With(f => f.Id = 2)
                .With(f => f.Path = @"C:\Test\Title2.avi")
                .Build();

            var episodes = Builder<Episode>.CreateListOfSize(2)
                .All()
                .With(e => e.Series = series)
                .TheFirst(1)
                .With(e => e.EpisodeFile = episodeFiles[0])
                .TheNext(1)
                .With(e => e.EpisodeFile = episodeFiles[1])
                .Build().ToList();



            Mocker.GetMock<IEpisodeService>()
                .Setup(c => c.EpisodesWithFiles()).Returns(episodes);

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[0] }, It.IsAny<Series>(), episodeFiles[0]))
                .Returns("New Title 1");

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[1] }, It.IsAny<Series>(), episodeFiles[1]))
                .Returns("New Title 2");


            var totalItems = 0;
            var misnamedEpisodes = Subject.MisnamedFiles(1, 10, out totalItems);


            misnamedEpisodes.Should().HaveCount(2);
        }

        [Test]
        public void one_misnamed_file()
        {

            var series = Builder<Series>.CreateNew()
                .With(s => s.Title = "SeriesTitle")
                .Build();

            var episodeFiles = Builder<EpisodeFile>.CreateListOfSize(2)
                .TheFirst(1)
                .With(f => f.Id = 1)
                .With(f => f.Path = @"C:\Test\Title1.avi")
                .TheNext(1)
                .With(f => f.Id = 2)
                .With(f => f.Path = @"C:\Test\Title2.avi")
                .Build();

            var episodes = Builder<Episode>.CreateListOfSize(2)
                .All()
                .With(e => e.Series = series)
                .TheFirst(1)
                .With(e => e.EpisodeFile = episodeFiles[0])
                .TheNext(1)
                .With(e => e.EpisodeFile = episodeFiles[1])
                .Build().ToList();



            Mocker.GetMock<IEpisodeService>()
                .Setup(c => c.EpisodesWithFiles()).Returns(episodes);

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[0] }, It.IsAny<Series>(), episodeFiles[0]))
                .Returns("New Title 1");

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[1] }, It.IsAny<Series>(), episodeFiles[1]))
                .Returns("Title2");


            var totalItems = 0;
            var misnamedEpisodes = Subject.MisnamedFiles(1, 10, out totalItems);


            misnamedEpisodes.Should().HaveCount(1);
            misnamedEpisodes[0].CurrentName.Should().Be("Title1");
            misnamedEpisodes[0].ProperName.Should().Be("New Title 1");
        }

        [Test]
        [Ignore]
        public void misnamed_multi_episode_file()
        {

            var series = Builder<Series>.CreateNew()
                .With(s => s.Title = "SeriesTitle")
                .Build();

            var episodeFiles = Builder<EpisodeFile>.CreateListOfSize(2)
                .TheFirst(1)
                .With(f => f.Id = 1)
                .With(f => f.Path = @"C:\Test\Title1.avi")
                .TheNext(1)
                .With(f => f.Id = 2)
                .With(f => f.Path = @"C:\Test\Title2.avi")
                .Build().ToList();

            var episodes = Builder<Episode>.CreateListOfSize(3)
                .All()
                .With(e => e.Series = series)
                .TheFirst(2)
                .With(e => e.EpisodeFile = episodeFiles[0])
                .TheNext(1)
                .With(e => e.EpisodeFile = episodeFiles[1])
                .Build().ToList();



            Mocker.GetMock<IEpisodeService>()
                .Setup(c => c.EpisodesWithFiles()).Returns(episodes);

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[0], episodes[1] }, It.IsAny<Series>(), episodeFiles[0]))
                .Returns("New Title 1");

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[2] }, It.IsAny<Series>(), episodeFiles[1]))
                .Returns("Title2");


            var totalItems = 0;
            var misnamedEpisodes = Subject.MisnamedFiles(1, 10, out totalItems);


            misnamedEpisodes.Should().HaveCount(1);
            misnamedEpisodes[0].CurrentName.Should().Be("Title1");
            misnamedEpisodes[0].ProperName.Should().Be("New Title 1");
        }

        [Test]
        [Ignore]
        public void no_misnamed_multi_episode_file()
        {

            var series = Builder<Series>.CreateNew()
                .With(s => s.Title = "SeriesTitle")
                .Build();

            var episodeFiles = Builder<EpisodeFile>.CreateListOfSize(2)
                .TheFirst(1)
                .With(f => f.Id = 1)
                .With(f => f.Path = @"C:\Test\Title1.avi")
                .TheNext(1)
                .With(f => f.Id = 2)
                .With(f => f.Path = @"C:\Test\Title2.avi")
                .Build().ToList();

            var episodes = Builder<Episode>.CreateListOfSize(3)
                .All()
                .With(e => e.Series = series)
                .TheFirst(2)
                .With(e => e.EpisodeFile = episodeFiles[0])
                .TheNext(1)
                .With(e => e.EpisodeFile = episodeFiles[1])
                .Build().ToList();



            Mocker.GetMock<IEpisodeService>()
                .Setup(c => c.EpisodesWithFiles()).Returns(episodes);

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[0], episodes[1] }, It.IsAny<Series>(), episodeFiles[0]))
                .Returns("Title1");

            Mocker.GetMock<IBuildFileNames>()
                .Setup(c => c.BuildFilename(new List<Episode> { episodes[2] }, It.IsAny<Series>(), episodeFiles[1]))
                .Returns("Title2");


            var totalItems = 0;
            var misnamedEpisodes = Subject.MisnamedFiles(1, 10, out totalItems);


            misnamedEpisodes.Should().HaveCount(0);
        }
    }
}
