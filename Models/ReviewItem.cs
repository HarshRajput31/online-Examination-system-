namespace OnlineExaminationSystem.Models
{
    /// <summary>
    /// Mirror of OnlineExaminationSystem.ReviewItem in Models namespace
    /// (keeps the csproj declaration valid). Not used directly - the
    /// real type lives next to ExamReview.aspx.cs.
    /// </summary>
    public class ReviewItemDto
    {
        public string QuestionText { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string Result { get; set; }

        public double Marks { get; set; }
        public double NegativeMarks { get; set; }
        public double MarksAwarded { get; set; }
    }
}
