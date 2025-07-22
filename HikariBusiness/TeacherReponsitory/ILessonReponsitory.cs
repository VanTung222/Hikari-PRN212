using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public interface ILessonReponsitory
    {
        public void AddLesson(Lesson lesson);
        public void UpdateLesson(Lesson lesson);
        public void DeleteLesson(int lessonId);
        public Lesson GetLessonById(int lessonId);
        public List<Lesson> GetAllLessons();
        public List<Lesson> GetLessonsByCourseId(string courseId);
    }
}
