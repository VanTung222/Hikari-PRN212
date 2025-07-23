using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HikariDataAccess.TeacherDAO
{
    public class CourseDAO
    {
        public List<Course> GetAllCourses()
        {
            using (var context = new HikariContext())
            {
                return context.Courses.ToList();
            }
        }

        public void AddCourse(Course course)
        {
            using (var context = new HikariContext())
            {
                context.Courses.Add(course);
                context.SaveChanges();
            }
        }

        public void UpdateCourse(Course course)
        {
            using (var context = new HikariContext())
            {
                context.Courses.Update(course);
                context.SaveChanges();
            }
        }

        public void DeleteCourse(string courseId)
        {
            using (var context = new HikariContext())
            {
                var course = context.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course != null)
                {
                    context.Courses.Remove(course);
                    context.SaveChanges();
                }
            }
        }

        public Course GetCourseById(string courseId)
        {
            using (var context = new HikariContext())
            {
                return context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            }
        }
    }
}
