using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Represents an exam document (collection: "exams").
    /// QuestionIds is the list of question IDs assigned to this paper -
    /// referenced by ExamPreview, AssignQuestions, and StartExam.
    /// </summary>
    public class Exam
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("examId")]
        public string ExamId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("subject")]
        [BsonIgnoreIfNull]
        public string Subject { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; }

        [BsonElement("totalMarks")]
        public int TotalMarks { get; set; }

        [BsonElement("setNumber")]
        [BsonIgnoreIfNull]
        public string SetNumber { get; set; }

        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; }

        [BsonElement("isPublished")]
        public bool IsPublished { get; set; }

        [BsonElement("instructions")]
        [BsonIgnoreIfNull]
        public string Instructions { get; set; }

        [BsonElement("startDate")]
        [BsonIgnoreIfNull]
        public DateTime? StartDate { get; set; }

        [BsonElement("dueDate")]
        [BsonIgnoreIfNull]
        public DateTime? DueDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("approvedAt")]
        [BsonIgnoreIfNull]
        public DateTime? ApprovedAt { get; set; }

        [BsonElement("publishedDate")]
        [BsonIgnoreIfNull]
        public DateTime? PublishedDate { get; set; }

        // Question linkage
        [BsonElement("questions")]
        [BsonIgnoreIfNull]
        public List<string> QuestionIds { get; set; } = new List<string>();
    }
}
