using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Represents a row in the "users" collection. Used by all auth flows.
    /// Roles: 1 = Admin, 2 = Student, 3 = Faculty.
    /// </summary>
    public class User
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("name")]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("personalEmail")]
        [BsonIgnoreIfNull]
        public string PersonalEmail { get; set; }

        [BsonElement("passwordHash")]
        [BsonIgnoreIfNull]
        public string PasswordHash { get; set; }

        [BsonElement("roleId")]
        public int RoleId { get; set; }

        [BsonElement("role")]
        [BsonIgnoreIfNull]
        public string Role { get; set; }

        [BsonElement("isActive")]
        [BsonDefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [BsonElement("isBlocked")]
        [BsonDefaultValue(false)]
        public bool IsBlocked { get; set; }

        [BsonElement("mustSetPassword")]
        [BsonDefaultValue(false)]
        public bool MustSetPassword { get; set; }

        // Profile fields (optional)
        [BsonElement("mobile")]      [BsonIgnoreIfNull] public string Mobile { get; set; }
        [BsonElement("course")]      [BsonIgnoreIfNull] public string Course { get; set; }
        [BsonElement("department")]  [BsonIgnoreIfNull] public string Department { get; set; }
        [BsonElement("profilePhoto")][BsonIgnoreIfNull] public string ProfilePhoto { get; set; }

        // Security questions (used by ForgotPassword)
        [BsonElement("SecurityQuestion")] [BsonIgnoreIfNull] public string SecurityQuestion { get; set; }
        [BsonElement("SecurityAnswer")]   [BsonIgnoreIfNull] public string SecurityAnswer { get; set; }

        // Password setup (invite flow)
        [BsonElement("passwordSetupToken")]          [BsonIgnoreIfNull] public string PasswordSetupToken { get; set; }
        [BsonElement("passwordSetupTokenHash")]      [BsonIgnoreIfNull] public string PasswordSetupTokenHash { get; set; }
        [BsonElement("passwordSetupTokenExpiresAt")] [BsonIgnoreIfNull] public DateTime? PasswordSetupTokenExpiresAt { get; set; }

        // Audit
        [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("lastLogin")] [BsonIgnoreIfNull] public DateTime? LastLogin { get; set; }
    }
}
