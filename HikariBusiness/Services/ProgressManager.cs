using HikariDataAccess.DAO;
using HikariDataAccess.Entities;
using System;

namespace HikariBusiness.Services
{
    public class ProgressManager
    {
        private readonly ProgressDAO _progressDAO;

        public ProgressManager()
        {
            _progressDAO = new ProgressDAO();
        }

        public Progress GetProgress(string studentId, int lessonId)
        {
            try
            {
                return _progressDAO.GetProgress(studentId, lessonId);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error getting progress: {ex.Message}");
                return null;
            }
        }

        public void SaveExerciseScore(string studentId, int lessonId, decimal score, string enrollmentId)
        {
            try
            {
                var progress = new Progress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    EnrollmentId = enrollmentId, // Important for context
                    CompletionStatus = "Completed", // Mark as completed when they do the exercise
                    EndDate = DateOnly.FromDateTime(DateTime.Now),
                    Score = score
                };

                _progressDAO.AddOrUpdateProgress(progress);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error saving exercise score: {ex.Message}");
            }
        }

        public void MarkLessonAsCompleted(string studentId, int lessonId, string enrollmentId)
        {
            try
            {
                var progress = new Progress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    EnrollmentId = enrollmentId,
                    CompletionStatus = "Completed",
                    EndDate = DateOnly.FromDateTime(DateTime.Now),
                    Score = null // No score for just watching a video
                };

                _progressDAO.AddOrUpdateProgress(progress);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error marking lesson as completed: {ex.Message}");
            }
        }
    }
}
