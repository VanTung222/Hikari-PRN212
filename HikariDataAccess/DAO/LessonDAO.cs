using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariDataAccess.DAO
{
    public class LessonDAO
    {
        public List<Lesson> GetLessonsByCourseId(string courseId)
        {
            using (var context = new HikariContext())
            {
                return context.Lessons
                              .Include(l => l.Exercise) // Tải kèm dữ liệu bài tập
                              .Where(l => l.CourseId == courseId && l.IsActive == true)
                              .OrderBy(l => l.Id) // Sắp xếp theo thứ tự để bài học có trình tự
                              .ToList();
            }
        }

        public Lesson GetLessonById(int lessonId)
        {
            using (var context = new HikariContext())
            {
                return context.Lessons.FirstOrDefault(l => l.Id == lessonId);
            }
        }

        // Tùy chọn: Phương thức để cập nhật trạng thái đã hoàn thành của bài học
        public void UpdateLessonCompletion(int lessonId, bool isCompleted)
        {
            using (var context = new HikariContext())
            {
                var lesson = context.Lessons.FirstOrDefault(l => l.Id == lessonId);
                if (lesson != null)
                {
                    lesson.IsCompleted = isCompleted;
                    context.SaveChanges();
                }
            }
        }
    }
}
