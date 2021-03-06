﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marr.Data;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Messaging;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Test.Framework
{
    public abstract class DbTest<TSubject, TModel> : DbTest
        where TSubject : class
        where TModel : ModelBase, new()
    {
        private TSubject _subject;

        protected BasicRepository<TModel> Storage { get; private set; }

        protected IList<TModel> AllStoredModels
        {
            get
            {
                return Storage.All().ToList();
            }
        }

        protected TModel StoredModel
        {
            get
            {
                return Storage.All().Single();
            }
        }

        [SetUp]
        public void CoreTestSetup()
        {
            _subject = null;
            Storage = Mocker.Resolve<BasicRepository<TModel>>();
        }

        protected TSubject Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = Mocker.Resolve<TSubject>();
                }

                return _subject;
            }

        }
    }




    public abstract class DbTest : CoreTest
    {

        private string _dbName;

        private ITestDatabase _db;
        private IDatabase _database;


        protected virtual MigrationType MigrationType
        {
            get
            {
                return MigrationType.Main;

            }
        }

        protected ITestDatabase Db
        {
            get
            {
                if (_db == null)
                    throw new InvalidOperationException("Test object database doesn't exists. Make sure you call WithRealDb() if you intend to use an actual database.");

                return _db;
            }
        }

        private void WithObjectDb(bool memory = true)
        {

            _dbName = DateTime.Now.Ticks.ToString() + ".db";

            MapRepository.Instance.EnableTraceLogging = true;

            var factory = new DbFactory(new MigrationController(new MigrationLogger(TestLogger)));
            _database = factory.Create(_dbName, MigrationType);
            _db = new TestTestDatabase(_database);
            Mocker.SetConstant(_database);
        }

        [SetUp]
        public void SetupReadDb()
        {
            WithObjectDb();
        }

        [TearDown]
        public void TearDown()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.db");

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
        }

    }


    public interface ITestDatabase
    {
        void InsertMany<T>(IEnumerable<T> items) where T : ModelBase, new();
        T Insert<T>(T item) where T : ModelBase, new();
        List<T> All<T>() where T : ModelBase, new();
        T Single<T>() where T : ModelBase, new();
        void Update<T>(T childModel) where T : ModelBase, new();
        void Delete<T>(T childModel) where T : ModelBase, new();
    }

    public class TestTestDatabase : ITestDatabase
    {
        private readonly IDatabase _dbConnection;
        private IMessageAggregator _messageAggregator;

        public TestTestDatabase(IDatabase dbConnection)
        {
            _messageAggregator = new Mock<IMessageAggregator>().Object;
            _dbConnection = dbConnection;
        }

        public void InsertMany<T>(IEnumerable<T> items) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _messageAggregator).InsertMany(items.ToList());
        }

        public T Insert<T>(T item) where T : ModelBase, new()
        {
            return new BasicRepository<T>(_dbConnection, _messageAggregator).Insert(item);
        }

        public List<T> All<T>() where T : ModelBase, new()
        {
            return new BasicRepository<T>(_dbConnection, _messageAggregator).All().ToList();
        }

        public T Single<T>() where T : ModelBase, new()
        {
            return All<T>().SingleOrDefault();
        }

        public void Update<T>(T childModel) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _messageAggregator).Update(childModel);
        }

        public void Delete<T>(T childModel) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _messageAggregator).Delete(childModel);
        }
    }
}