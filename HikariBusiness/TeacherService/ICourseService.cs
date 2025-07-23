using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public interface ICourseService
    {
       public void AddCourse(Course course);
        public void UpdateCourse(Course course);
        public void DeleteCourse(String courseId);
        public Course GetCourseById(String courseId);
        public List<Course> GetAllCourses();
    }
}
