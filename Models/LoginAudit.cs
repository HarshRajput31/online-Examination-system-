using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// One row per login attempt (collection: "login_audit").
    /// AdminDashboard chart aggregates these by day-of-week.
    /// </summary>
    public class LoginAudit
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        [BsonIgnoreIfNull]
        public string UserId { get; set; }

        [BsonElement("email")]
        [BsonIgnoreIfNull]
        public string Email { get; set; }

        [BsonElement("loginTime")]
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;

        [BsonElement("ipAddress")]
        [BsonIgnoreIfNull]
        public string IpAddress { get; set; }

        [BsonElement("status")]
        [BsonIgnoreIfNull]
        public string Status { get; set; }
    }
}
