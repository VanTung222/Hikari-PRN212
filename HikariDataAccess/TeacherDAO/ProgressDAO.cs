using DataAccessLayer;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HikariDataAccess.TeacherDAO
{
    public class ProgressDAO
    {
        public List<Progress> GetProgressByStudentId(string studentId)
        {
            using (var context = new HikariContext())
            {
                return context.Progresses
                    .Include(p => p.Lesson)
                    .Where(p => p.StudentId == studentId)
                    .ToList();
            }
        }

        public List<Progress> GetAllProgress()
        {
            using (var context = new HikariContext())
            {
                return context.Progresses.ToList();
            }
        }

        public void AddProgress(Progress progress)
        {
            using (var context = new HikariContext())
            {
                context.Progresses.Add(progress);
                context.SaveChanges();
            }
        }

        public void UpdateProgress(Progress progress)
        {
            using (var context = new HikariContext())
            {
                var existing = context.Progresses.FirstOrDefault(p => p.Id == progress.Id);
                if (existing != null)
                {
                    existing.CompletionStatus = progress.CompletionStatus;
                    existing.Score = progress.Score;
                    existing.Feedback = progress.Feedback;
                    existing.StartDate = progress.StartDate;
                    existing.EndDate = progress.EndDate;

                    context.SaveChanges();
                }
            }
        }

        public void DeleteProgress(int id)
        {
            using (var context = new HikariContext())
            {
                var progress = context.Progresses.FirstOrDefault(p => p.Id == id);
                if (progress != null)
                {
                    context.Progresses.Remove(progress);
                    context.SaveChanges();
                }
            }
        }

        public Progress GetProgressById(int id)
        {
            using (var context = new HikariContext())
            {
                return context.Progresses.FirstOrDefault(p => p.Id == id);
            }
        }
    }
}
