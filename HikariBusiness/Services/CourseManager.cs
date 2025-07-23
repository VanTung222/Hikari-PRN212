using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.DAO;
using HikariDataAccess.Entities;

namespace HikariBusiness.Services
{
    public class CourseManager
    {
        private readonly CourseDAO _courseDAO;

        public CourseManager()
        {
            _courseDAO = new CourseDAO();
        }

        public List<Course> GetAllActiveCourses()
        {
            try
            {
                return _courseDAO.GetAllActiveCourses();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                throw new Exception($"Lỗi khi lấy danh sách khóa học: {ex.Message}", ex);
            }
        }

        public Course GetCourseById(string courseId)
        {
            try
            {
                return _courseDAO.GetCourseById(courseId);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                throw new Exception($"Lỗi khi lấy thông tin khóa học {courseId}: {ex.Message}", ex);
            }
        }

        public void CreateNewCourse(Course course)
        {
            if (string.IsNullOrEmpty(course.Title))
            {
                throw new ArgumentException("Tiêu đề khóa học không được để trống.");
            }
            // Các kiểm tra logic nghiệp vụ khác có thể thêm tại đây
            // Ví dụ: Đảm bảo CourseId không trùng lặp nếu bạn tự tạo ID trước khi insert
            // if (_courseDAO.GetCourseById(course.CourseId) != null)
            // {
            //     throw new ArgumentException($"Course ID '{course.CourseId}' đã tồn tại.");
            // }

            try
            {
                _courseDAO.AddCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm khóa học mới: {ex.Message}", ex);
            }
        }

        public void UpdateExistingCourse(Course course)
        {
            if (string.IsNullOrEmpty(course.CourseId)) // Sử dụng CourseId
            {
                throw new ArgumentException("Course ID không được để trống khi cập nhật.");
            }
            if (string.IsNullOrEmpty(course.Title))
            {
                throw new ArgumentException("Tiêu đề khóa học không được để trống.");
            }
            // Các kiểm tra logic nghiệp vụ khác có thể thêm tại đây

            try
            {
                _courseDAO.UpdateCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật khóa học {course.CourseId}: {ex.Message}", ex);
            }
        }

        public void RemoveCourse(string courseId)
        {
            if (string.IsNullOrEmpty(courseId)) // Sử dụng CourseId
            {
                throw new ArgumentException("Course ID không được để trống khi xóa.");
            }
            try
            {
                _courseDAO.DeactivateCourse(courseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa khóa học {courseId}: {ex.Message}", ex);
            }
        }
    }
}
