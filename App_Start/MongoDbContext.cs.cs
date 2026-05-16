using System;
using System.Configuration;
using MongoDB.Driver;

namespace OnlineExaminationSystem.App_Start
{
    /// <summary>
    /// Centralized MongoDB connection. Reads connection string and database
    /// name from Web.config (appSettings) so we don't hardcode them everywhere.
    /// Falls back to localhost defaults so existing pages keep working.
    /// </summary>
    public static class MongoDbContext
    {
        private static readonly Lazy<IMongoDatabase> _database =
            new Lazy<IMongoDatabase>(BuildDatabase);

        public static IMongoDatabase Database => _database.Value;

        public static IMongoClient Client { get; private set; }

        private static IMongoDatabase BuildDatabase()
        {
            string connStr = ConfigurationManager.AppSettings["MongoConnectionString"];
            if (string.IsNullOrWhiteSpace(connStr))
            {
                connStr = "mongodb://localhost:27017";
            }

            string dbName = ConfigurationManager.AppSettings["MongoDatabaseName"];
            if (string.IsNullOrWhiteSpace(dbName))
            {
                dbName = "OnlineExamDB";
            }

            Client = new MongoClient(connStr);
            return Client.GetDatabase(dbName);
        }

        /// <summary>Convenience accessor for a typed collection.</summary>
        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return Database.GetCollection<T>(name);
        }
    }
}
