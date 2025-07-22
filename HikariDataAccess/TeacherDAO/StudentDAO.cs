using DataAccessLayer;
using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HikariDataAccess.TeacherDAO
{
    public class StudentDAO
    {
        public List<Student> GetAllStudents()
        {
            using (var context = new HikariContext())
            {
                return context.Students.Include(s => s.User).ToList();
            }
        }

        public List<CourseEnrollment> GetEnrollmentsWithCourse(string studentId)
        {
            using (var context = new HikariContext())
            {
                return context.CourseEnrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId)
                    .ToList();
            }
        }
    }
} 