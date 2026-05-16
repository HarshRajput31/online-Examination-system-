using System;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Services
{
    /// <summary>
    /// Helper for inserting and reading notifications. Pages call
    /// NotificationService.Push(...) so the wiring is centralized.
    /// </summary>
    public static class NotificationService
    {
        private static IMongoCollection<Notification> Collection =>
            MongoDbContext.GetCollection<Notification>("notifications");

        public static string Push(string userId, string type, string title, string message, string link = null)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var n = new Notification
            {
                NotificationId = "N" + DateTime.UtcNow.Ticks.ToString().Substring(8),
                UserId = userId,
                Type = type ?? "general",
                Title = title ?? "",
                Message = message ?? "",
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            Collection.InsertOne(n);
            return n.NotificationId;
        }

        public static long UnreadCount(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return 0;
            var f = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.IsRead, false));
            return Collection.CountDocuments(f);
        }

        public static void MarkRead(string notificationId, string userId)
        {
            if (string.IsNullOrWhiteSpace(notificationId)) return;
            var f = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.NotificationId, notificationId),
                Builders<Notification>.Filter.Eq(n => n.UserId, userId));
            Collection.UpdateOne(f,
                Builders<Notification>.Update
                    .Set(n => n.IsRead, true)
                    .Set(n => n.ReadAt, DateTime.UtcNow));
        }

        public static void MarkAllRead(string userId)
        {
            var f = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
            Collection.UpdateMany(f,
                Builders<Notification>.Update
                    .Set(n => n.IsRead, true)
                    .Set(n => n.ReadAt, DateTime.UtcNow));
        }
    }
}
