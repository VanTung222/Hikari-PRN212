using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System.Collections.Generic;

namespace HikariBusiness.TeacherService
{
    public class StudentService
    {
        private StudentReponsitory _studentRepository = new StudentReponsitory();
        public List<Student> GetAllStudents() => _studentRepository.GetAllStudents();
        public List<CourseEnrollment> GetEnrollmentsWithCourse(string studentId) => _studentRepository.GetEnrollmentsWithCourse(studentId);
    }
} 