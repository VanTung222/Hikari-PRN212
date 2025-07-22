using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public class LessonService : ILessonService
    {
        private readonly ILessonReponsitory _lessonRepository;
        public LessonService()
        {
            _lessonRepository = new LessonReponsitory();
        }
        public void AddLesson(Lesson lesson) => _lessonRepository.AddLesson(lesson);

        public void DeleteLesson(int lessonId) => _lessonRepository.DeleteLesson(lessonId);

        public List<Lesson> GetAllLessons() => _lessonRepository.GetAllLessons();

        public Lesson GetLessonById(int lessonId) => _lessonRepository.GetLessonById(lessonId);

        public List<Lesson> GetLessonsByCourseId(string courseId) => _lessonRepository.GetLessonsByCourseId(courseId);

        public void UpdateLesson(Lesson lesson) => _lessonRepository.UpdateLesson(lesson);
    }
}
