using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.Entities;

namespace HikariDataAccess.DAO
{
    public class CourseDAO
    {
        // Lấy tất cả các khóa học đang hoạt động
        public List<Course> GetAllActiveCourses()
        {
            using (var context = new HikariContext()) // HikariContext là tên DbContext của bạn
            {
                // Sử dụng IsActive? == true để xử lý kiểu bool?
                return context.Courses
                              .Where(c => c.IsActive == true)
                              .ToList();
            }
        }

        // Lấy một khóa học theo CourseId
        public Course GetCourseById(string courseId)
        {
            using (var context = new HikariContext())
            {
                return context.Courses.FirstOrDefault(c => c.CourseId == courseId); // Sử dụng CourseId
            }
        }

        // Thêm một khóa học mới
        public void AddCourse(Course course)
        {
            using (var context = new HikariContext())
            {
                context.Courses.Add(course);
                context.SaveChanges();
            }
        }

        // Cập nhật thông tin khóa học
        public void UpdateCourse(Course course)
        {
            using (var context = new HikariContext())
            {
                var existingCourse = context.Courses.FirstOrDefault(c => c.CourseId == course.CourseId); // Sử dụng CourseId
                if (existingCourse != null)
                {
                    existingCourse.Title = course.Title;
                    existingCourse.Description = course.Description;
                    existingCourse.ImageUrl = course.ImageUrl; // Đã đổi thành ImageUrl
                    existingCourse.Fee = course.Fee;             // Đã đổi thành Fee
                    existingCourse.Duration = course.Duration;
                    existingCourse.StartDate = course.StartDate; // Thuộc tính mới
                    existingCourse.EndDate = course.EndDate;     // Thuộc tính mới
                    existingCourse.IsActive = course.IsActive;

                    // Các thuộc tính CreatedAt và Difficulty không còn trong model Course của bạn

                    context.Courses.Update(existingCourse);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy khóa học để cập nhật.");
                }
            }
        }

        // Xóa (đánh dấu không hoạt động) một khóa học
        public void DeactivateCourse(string courseId)
        {
            using (var context = new HikariContext())
            {
                var course = context.Courses.FirstOrDefault(c => c.CourseId == courseId); // Sử dụng CourseId
                if (course != null)
                {
                    course.IsActive = false; // Đánh dấu không hoạt động thay vì xóa vật lý
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy khóa học để xóa.");
                }
            }
        }
    }
}
