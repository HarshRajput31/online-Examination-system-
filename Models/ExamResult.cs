using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Represents a single submission (collection: "results").
    /// MyResults / ExamHistory / FacultyAnalytics all read this shape.
    /// Note: Score and TotalQuestions are doubles so percentage maths
    /// stays accurate regardless of question marks.
    /// </summary>
    public class ExamResult
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("resultId")]
        [BsonIgnoreIfNull]
        public string ResultId { get; set; }

        [BsonElement("studentId")]
        public string StudentId { get; set; }

        [BsonElement("studentName")]
        [BsonIgnoreIfNull]
        public string StudentName { get; set; }

        [BsonElement("examId")]
        public string ExamId { get; set; }

        [BsonElement("examName")]
        [BsonIgnoreIfNull]
        public string ExamName { get; set; }

        [BsonElement("subject")]
        [BsonIgnoreIfNull]
        public string Subject { get; set; }

        // Marks awarded
        [BsonElement("score")]          public double Score { get; set; }
        [BsonElement("totalQuestions")] public double TotalQuestions { get; set; }
        [BsonElement("totalMarks")]     public double TotalMarks { get; set; }

        // Breakdown
        [BsonElement("correctAnswers")] public int CorrectAnswers { get; set; }
        [BsonElement("wrongAnswers")]   public int WrongAnswers { get; set; }
        [BsonElement("notAttempted")]   public int NotAttempted { get; set; }

        [BsonElement("percentage")] public double Percentage { get; set; }
        [BsonElement("passed")]     public bool Passed { get; set; }

        // Status: "Auto-Graded", "Pending Review", "Reviewed", "Recheck Pending"
        [BsonElement("status")]
        [BsonIgnoreIfNull]
        public string Status { get; set; }

        // Map of questionId -> selected answer (e.g. "A"/"B"/...) or text
        [BsonElement("answers")]
        [BsonIgnoreIfNull]
        public Dictionary<string, string> Answers { get; set; }

        // Manual marks given by faculty (questionId -> marks awarded)
        [BsonElement("descriptiveMarks")]
        [BsonIgnoreIfNull]
        public Dictionary<string, double> DescriptiveMarks { get; set; }

        [BsonElement("submittedAt")] public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("reviewedAt")]  [BsonIgnoreIfNull] public DateTime? ReviewedAt { get; set; }
        [BsonElement("reviewedBy")]  [BsonIgnoreIfNull] public string ReviewedBy { get; set; }
    }
}
