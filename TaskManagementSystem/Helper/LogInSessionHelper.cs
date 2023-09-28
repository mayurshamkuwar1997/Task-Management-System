using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TaskManagementSystem.Helper
{
    public static class LogInSessionHelper
    {
        private static Guid LoggedInUserId { get; set; }
        private static string? LoggedInEmail { get; set; }
        public static string? GetLoggedInUserEmail()
        {
            return LoggedInEmail;
        }

        public static void SetLoggedInUser(Guid userId, string? email)
        {
            LoggedInUserId = userId;
            LoggedInEmail = email;
        }

        public static void Logout()
        {
            LoggedInUserId = Guid.Empty;
            LoggedInEmail = string.Empty;
        }
    }
}
