using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>Student profile document (collection: "students").</summary>
    public class Student
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("studentId")]
        public string StudentId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("personalEmail")]
        [BsonIgnoreIfNull]
        public string PersonalEmail { get; set; }

        [BsonElement("loginEmail")]
        [BsonIgnoreIfNull]
        public string LoginEmail { get; set; }

        [BsonElement("rollNumber")] [BsonIgnoreIfNull] public string RollNumber { get; set; }
        [BsonElement("course")]     [BsonIgnoreIfNull] public string Course { get; set; }
        [BsonElement("department")] [BsonIgnoreIfNull] public string Department { get; set; }
        [BsonElement("semester")]   [BsonIgnoreIfNull] public string Semester { get; set; }
        [BsonElement("mobile")]     [BsonIgnoreIfNull] public string Mobile { get; set; }

        [BsonElement("isActive")]   [BsonDefaultValue(true)] public bool IsActive { get; set; } = true;
        [BsonElement("createdAt")]  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
