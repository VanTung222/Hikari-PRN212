using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HikariDataAccess.DAO
{
    public class ProgressDAO
    {
        private readonly HikariContext _context;

        public ProgressDAO()
        {
            _context = new HikariContext();
        }

        public Progress GetProgress(string studentId, int lessonId)
        {
            return _context.Progresses
                           .FirstOrDefault(p => p.StudentId == studentId && p.LessonId == lessonId);
        }

        public void AddOrUpdateProgress(Progress progress)
        {
            var existingProgress = GetProgress(progress.StudentId, progress.LessonId);

            if (existingProgress != null)
            {
                // Update existing record
                existingProgress.CompletionStatus = progress.CompletionStatus;
                existingProgress.EndDate = progress.EndDate;
                if (progress.Score.HasValue)
                {
                    existingProgress.Score = progress.Score;
                }
                if (!string.IsNullOrEmpty(progress.Feedback))
                {
                    existingProgress.Feedback = progress.Feedback;
                }
                _context.Progresses.Update(existingProgress);
            }
            else
            {
                // Add new record
                _context.Progresses.Add(progress);
            }

            _context.SaveChanges();
        }

        public List<Progress> GetProgressForCourse(string studentId, string courseId)
        {
            return _context.Progresses
                           .Where(p => p.StudentId == studentId && p.Lesson.CourseId == courseId)
                           .ToList();
        }
    }
}
