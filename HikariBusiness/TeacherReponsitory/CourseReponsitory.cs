using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public class CourseReponsitory : ICourseReponsitory
    {
        private CourseDAO dao = new CourseDAO();
        public void AddCourse(Course course) => dao.AddCourse(course);

        public void DeleteCourse(string courseId) => dao.DeleteCourse(courseId);

        public List<Course> GetAllCourses() => dao.GetAllCourses();

        public Course GetCourseById(string courseId) => dao.GetCourseById(courseId);

        public void UpdateCourse(Course course) => dao.UpdateCourse(course);

    }
}
