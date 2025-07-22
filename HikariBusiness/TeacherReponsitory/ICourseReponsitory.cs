using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public interface ICourseReponsitory
    {
        public void AddCourse(Course course);
        public void UpdateCourse(Course course);
        public void DeleteCourse(string courseId);
       
        public Course GetCourseById(string courseId);

        public List<Course> GetAllCourses();
    }
}
