using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public class CourseService : ICourseService
    {
        private readonly ICourseReponsitory _courseRepository;

        public CourseService()
        {
            _courseRepository = new CourseReponsitory();
        }
        public void AddCourse(Course course) => _courseRepository.AddCourse(course);

        public void DeleteCourse(String courseId) => _courseRepository.DeleteCourse(courseId);

        public List<Course> GetAllCourses() => _courseRepository.GetAllCourses();
        public Course GetCourseById(String courseId) => _courseRepository.GetCourseById(courseId);


        public void UpdateCourse(Course course) => _courseRepository.UpdateCourse(course);
    }
}
