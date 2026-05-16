using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Represents a question (collection: "questions").
    /// Supports both MCQ (with options + correctAnswer) and Descriptive
    /// (modelAnswer + maxWords). QuestionType is "mcq" or "desc".
    /// </summary>
    public class Question
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement("questionId")]
        public string QuestionId { get; set; }

        [BsonElement("examId")]
        public string ExamId { get; set; }

        [BsonElement("subject")]
        [BsonIgnoreIfNull]
        public string Subject { get; set; }

        [BsonElement("setNumber")]
        [BsonIgnoreIfNull]
        public string SetNumber { get; set; }

        [BsonElement("questionText")]
        public string QuestionText { get; set; }

        // "mcq" or "desc"
        [BsonElement("questionType")]
        public string QuestionType { get; set; } = "mcq";

        // ---- MCQ ----
        [BsonElement("optionA")] [BsonIgnoreIfNull] public string OptionA { get; set; }
        [BsonElement("optionB")] [BsonIgnoreIfNull] public string OptionB { get; set; }
        [BsonElement("optionC")] [BsonIgnoreIfNull] public string OptionC { get; set; }
        [BsonElement("optionD")] [BsonIgnoreIfNull] public string OptionD { get; set; }
        [BsonElement("correctAnswer")] [BsonIgnoreIfNull] public string CorrectAnswer { get; set; }

        // ---- Descriptive ----
        [BsonElement("modelAnswer")] [BsonIgnoreIfNull] public string ModelAnswer { get; set; }
        [BsonElement("maxWords")]    [BsonIgnoreIfNull] public int? MaxWords { get; set; }

        // ---- Marks ----
        [BsonElement("marks")]         public double Marks { get; set; } = 1;
        [BsonElement("negativeMarks")] public double NegativeMarks { get; set; } = 0;

        [BsonElement("difficulty")] [BsonIgnoreIfNull] public string Difficulty { get; set; } = "Medium";

        // Sub-questions support (legacy from AddQuestion.aspx)
        [BsonElement("hasSubQuestions")] public bool HasSubQuestions { get; set; }
        [BsonElement("subQuestions")]
        [BsonIgnoreIfNull]
        public List<BsonDocument> SubQuestions { get; set; }

        [BsonElement("createdBy")] [BsonIgnoreIfNull] public string CreatedBy { get; set; }
        [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
