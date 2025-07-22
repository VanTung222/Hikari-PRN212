using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public class LessonReponsitory : ILessonReponsitory
    {
        private LessonDAO dao = new LessonDAO();
        public void AddLesson(Lesson lesson) => dao.AddLesson(lesson);

        public void DeleteLesson(int lessonId) => dao.DeleteLesson(lessonId);

        public List<Lesson> GetAllLessons() => dao.GetAllLessons();

        public Lesson GetLessonById(int lessonId) => dao.GetLessonById(lessonId);

        public List<Lesson> GetLessonsByCourseId(string courseId) => dao.GetLessonsByCourseId(courseId);

        public void UpdateLesson(Lesson lesson) => dao.UpdateLesson(lesson);
    }
}
