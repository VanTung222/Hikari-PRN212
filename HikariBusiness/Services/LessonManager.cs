using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.DAO;
using HikariDataAccess.Entities;

namespace HikariBusiness.Services
{
    public class LessonManager
    {
        private readonly LessonDAO _lessonDAO;
        private readonly ProgressDAO _progressDAO; // Add ProgressDAO

        public LessonManager()
        {
            _lessonDAO = new LessonDAO();
            _progressDAO = new ProgressDAO(); // Initialize it
        }

        // Updated method to include student progress
        public List<Lesson> GetLessonsForCourse(string courseId, string studentId)
        {
            var lessons = _lessonDAO.GetLessonsByCourseId(courseId);
            var progressRecords = _progressDAO.GetProgressForCourse(studentId, courseId);

            foreach (var lesson in lessons)
            {
                // Check if there is any progress record for this lesson for the current user
                lesson.IsCompletedByUser = progressRecords.Any(p => p.LessonId == lesson.Id && p.CompletionStatus == "Completed");
            }

            return lessons;
        }

        public Lesson GetLessonDetails(int lessonId)
        {
            return _lessonDAO.GetLessonById(lessonId);
        }
    }
}
