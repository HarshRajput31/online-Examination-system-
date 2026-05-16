using System;
using System.Configuration;
using MongoDB.Driver;

namespace OnlineExaminationSystem.App_Start
{
    public static class MongoDbContext
    {
        private static readonly Lazy<MongoClient> LazyClient = new Lazy<MongoClient>(CreateClient, true);

        public static MongoClient Client => LazyClient.Value;

        public static IMongoDatabase Database => Client.GetDatabase(DatabaseName);

        public static string ConnectionString =>
            ConfigurationManager.AppSettings["MongoConnectionString"] ?? "mongodb://127.0.0.1:27017";

        public static string DatabaseName =>
            ConfigurationManager.AppSettings["MongoDatabaseName"] ?? "OnlineExamDB";

        private static MongoClient CreateClient()
        {
            var settings = MongoClientSettings.FromConnectionString(ConnectionString);

            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
            settings.ApplicationName = "OnlineExaminationSystem";

            return new MongoClient(settings);
        }
    }
}
