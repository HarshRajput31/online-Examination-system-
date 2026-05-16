using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Recheck workflow record (collection: "recheck_requests").
    /// Status values:
    ///   "Pending"   -> student submitted, admin not yet acted
    ///   "Assigned"  -> admin assigned to a faculty member
    ///   "Completed" -> faculty rechecked and updated result
    ///   "Rejected"  -> admin rejected the request
    /// </summary>
    public class RecheckRequest
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("recheckId")] public string RecheckId { get; set; }
        [BsonElement("studentId")] public string StudentId { get; set; }
        [BsonElement("examId")]    public string ExamId { get; set; }
        [BsonElement("examTitle")] [BsonIgnoreIfNull] public string ExamTitle { get; set; }
        [BsonElement("subject")]   [BsonIgnoreIfNull] public string Subject { get; set; }
        [BsonElement("resultId")]  [BsonIgnoreIfNull] public string ResultId { get; set; }
        [BsonElement("reason")]    public string Reason { get; set; }

        [BsonElement("status")]    public string Status { get; set; } = "Pending";

        [BsonElement("assignedFacultyId")] [BsonIgnoreIfNull] public string AssignedFacultyId { get; set; }
        [BsonElement("assignedAt")]        [BsonIgnoreIfNull] public DateTime? AssignedAt { get; set; }

        [BsonElement("oldScore")] [BsonIgnoreIfNull] public double? OldScore { get; set; }
        [BsonElement("newScore")] [BsonIgnoreIfNull] public double? NewScore { get; set; }
        [BsonElement("facultyComments")] [BsonIgnoreIfNull] public string FacultyComments { get; set; }

        [BsonElement("requestedAt")] public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("completedAt")] [BsonIgnoreIfNull] public DateTime? CompletedAt { get; set; }
    }
}
