using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem
{
    /// <summary>
    /// Faculty profile document (collection: "faculty").
    /// Lives in OnlineExaminationSystem (not .Models) because the
    /// existing AddFaculty/EditFaculty pages reference it without a
    /// using directive.
    /// </summary>
    public class Faculty
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("facultyId")]
        public string FacultyId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        // Login email (auto-generated on invite)
        [BsonElement("email")]
        public string Email { get; set; }

        // Where the invite is sent
        [BsonElement("personalEmail")]
        [BsonIgnoreIfNull]
        public string PersonalEmail { get; set; }

        [BsonElement("loginEmail")]
        [BsonIgnoreIfNull]
        public string LoginEmail { get; set; }

        [BsonElement("department")] [BsonIgnoreIfNull] public string Department { get; set; }
        [BsonElement("mobile")]     [BsonIgnoreIfNull] public string Mobile { get; set; }
        [BsonElement("course")]     [BsonIgnoreIfNull] public string Course { get; set; }

        [BsonElement("isActive")]   [BsonDefaultValue(true)] public bool IsActive { get; set; } = true;
        [BsonElement("createdAt")]  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
