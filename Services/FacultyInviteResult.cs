namespace OnlineExaminationSystem.Services
{
    /// <summary>
    /// Returned by FacultyAccountService.EnsureFacultyLogin so the
    /// caller can show the admin a confirmation panel with the login
    /// email, setup link, and email-status message.
    /// </summary>
    public class FacultyInviteResult
    {
        public string FacultyId { get; set; }
        public string LoginEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string SetupLink { get; set; }
        public bool EmailSent { get; set; }
        public string EmailMessage { get; set; }
    }
}
