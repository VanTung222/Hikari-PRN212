namespace HikariApp
{
    public static class AppSession
    {
        // These would be set after a successful login
        public static string CurrentStudentId { get; set; } = "ST001"; // Placeholder
        public static string CurrentEnrollmentId { get; set; } = "EN001"; // Placeholder

        public static void Clear()
        {
            CurrentStudentId = null;
            CurrentEnrollmentId = null;
        }
    }
}
