using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// In-app notification (collection: "notifications").
    /// One row per recipient. Type values:
    ///   "exam_approved" | "exam_rejected" | "result_published" |
    ///   "recheck_requested" | "recheck_assigned" | "recheck_completed" |
    ///   "exam_published" | "general"
    /// </summary>
    public class Notification
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("notificationId")] public string NotificationId { get; set; }
        [BsonElement("userId")]         public string UserId { get; set; }
        [BsonElement("title")]          public string Title { get; set; }
        [BsonElement("message")]        public string Message { get; set; }
        [BsonElement("type")]           public string Type { get; set; } = "general";
        [BsonElement("link")]           [BsonIgnoreIfNull] public string Link { get; set; }
        [BsonElement("isRead")]         public bool IsRead { get; set; }
        [BsonElement("createdAt")]      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("readAt")]         [BsonIgnoreIfNull] public DateTime? ReadAt { get; set; }
    }
}
