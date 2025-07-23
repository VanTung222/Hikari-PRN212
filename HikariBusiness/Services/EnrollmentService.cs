using HikariDataAccess.Entities;
using HikariDataAccess;
using HikariBusiness.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HikariBusiness.Services
{
    public class EnrollmentService
    {
        private readonly HikariContext _context;

        public EnrollmentService()
        {
            _context = new HikariContext();
        }

        // Lấy danh sách khóa học đã đăng ký với thông tin progress
        public async Task<List<EnrolledCourseViewModel>> GetEnrolledCoursesWithProgressAsync(string studentId)
        {
            try
            {
                var enrolledCourses = await _context.CourseEnrollments
                    .Where(e => e.StudentID == studentId)
                    .Join(_context.Courses,
                          enrollment => enrollment.CourseID,
                          course => course.CourseID,
                          (enrollment, course) => new { enrollment, course })
                    .ToListAsync();

                var result = new List<EnrolledCourseViewModel>();

                foreach (var item in enrolledCourses)
                {
                    var viewModel = new EnrolledCourseViewModel
                    {
                        CourseID = item.course.CourseID,
                        Title = item.course.Title,
                        Description = item.course.Description ?? "",
                        Fee = item.course.Fee ?? 0,
                        EnrollmentDate = item.enrollment.EnrollmentDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now,
                        TotalLessons = await GetTotalLessonsAsync(item.course.CourseID),
                        CompletedLessons = await GetCompletedLessonsAsync(studentId, item.course.CourseID),
                        CourseIcon = GetCourseIcon(item.course.Title),
                        BackgroundColor = GetCourseColor(item.course.Title)
                    };

                    result.Add(viewModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy khóa học đã đăng ký: {ex.Message}");
            }
        }

        // Lấy tổng số bài học của khóa học
        private async Task<int> GetTotalLessonsAsync(string courseId)
        {
            try
            {
                var count = await _context.Lessons
                    .Where(l => l.CourseId == courseId)
                    .CountAsync();
                
                // Nếu không có lessons trong DB, trả về số mặc định
                return count > 0 ? count : 20;
            }
            catch
            {
                return 20; // Default value
            }
        }

        // Lấy số bài học đã hoàn thành (giả lập vì chưa có bảng progress)
        private async Task<int> GetCompletedLessonsAsync(string studentId, string courseId)
        {
            try
            {
                // Tạm thời trả về giá trị ngẫu nhiên để demo
                var random = new Random(studentId.GetHashCode() + courseId.GetHashCode());
                var totalLessons = await GetTotalLessonsAsync(courseId);
                return random.Next(0, totalLessons + 1);
            }
            catch
            {
                return 0;
            }
        }

        // Lấy icon cho khóa học dựa trên title
        private string GetCourseIcon(string title)
        {
            if (title.Contains("C#") || title.Contains("Programming")) return "💻";
            if (title.Contains("Web") || title.Contains("ASP.NET")) return "🌐";
            if (title.Contains("Data") || title.Contains("Python")) return "📊";
            if (title.Contains("Mobile") || title.Contains("Android") || title.Contains("iOS")) return "📱";
            if (title.Contains("Security") || title.Contains("An ninh")) return "🔒";
            if (title.Contains("AI") || title.Contains("Machine Learning")) return "🤖";
            if (title.Contains("Java")) return "☕";
            if (title.Contains("Cloud") || title.Contains("AWS") || title.Contains("Azure")) return "☁️";
            return "📚";
        }

        // Lấy màu nền cho khóa học
        private string GetCourseColor(string title)
        {
            if (title.Contains("C#") || title.Contains("Programming")) return "#6C5CE7";
            if (title.Contains("Web") || title.Contains("ASP.NET")) return "#00BCD4";
            if (title.Contains("Data") || title.Contains("Python")) return "#E91E63";
            if (title.Contains("Mobile")) return "#FF9800";
            if (title.Contains("Security") || title.Contains("An ninh")) return "#F44336";
            if (title.Contains("AI") || title.Contains("Machine Learning")) return "#9C27B0";
            if (title.Contains("Java")) return "#795548";
            if (title.Contains("Cloud")) return "#607D8B";
            return "#4CAF50";
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
