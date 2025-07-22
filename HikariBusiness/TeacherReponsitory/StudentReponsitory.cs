using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System.Collections.Generic;

namespace HikariBusiness.TeacherReponsitory
{
    public class StudentReponsitory
    {
        private StudentDAO dao = new StudentDAO();
        public List<Student> GetAllStudents() => dao.GetAllStudents();
        public List<CourseEnrollment> GetEnrollmentsWithCourse(string studentId) => dao.GetEnrollmentsWithCourse(studentId);
    }
} 