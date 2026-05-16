using MongoDB.Driver;
using System;

namespace OnlineExaminationSystem.App_Start
{
    public static class MongoDbContext
    {
        private static readonly IMongoDatabase _database;

        static MongoDbContext()
        {
            // 🔥 FINAL FIX (NO DNS, NO CRASH)
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress("localhost", 27017),

                // 🚫 Disable DNS completely
                DirectConnection = true,

                ServerSelectionTimeout = TimeSpan.FromSeconds(5),
                ConnectTimeout = TimeSpan.FromSeconds(5)
            };

            var client = new MongoClient(settings);
            _database = client.GetDatabase("OnlineExamDB");
        }

        public static IMongoDatabase Database => _database;
    }
}