using HikariDataAccess.Entities;
using HikariDataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HikariBusiness.Services.payment
{
    public class CourseService
    {
        private readonly HikariContext _context;

        public CourseService()
        {
            _context = new HikariContext();
        }

        // Lấy tất cả khóa học đang hoạt động
        public async Task<List<Course>> GetAllActiveCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Where(c => c.IsActive == true)
                    .OrderBy(c => c.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khóa học: {ex.Message}");
            }
        }

        // Lấy khóa học theo ID
        public async Task<Course?> GetCourseByIdAsync(string courseId)
        {
            try
            {
                return await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseID == courseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin khóa học: {ex.Message}");
            }
        }

        // Kiểm tra khóa học có trong giỏ hàng không
        public async Task<bool> IsCourseInCartAsync(string studentId, string courseId)
        {
            try
            {
                return await _context.Carts
                    .AnyAsync(c => c.StudentID == studentId && c.CourseID == courseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra giỏ hàng: {ex.Message}");
            }
        }

        // Kiểm tra student đã đăng ký khóa học chưa
        public async Task<bool> IsCourseEnrolledAsync(string studentId, string courseId)
        {
            try
            {
                return await _context.CourseEnrollments
                    .AnyAsync(e => e.StudentID == studentId && e.CourseID == courseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra đăng ký khóa học: {ex.Message}");
            }
        }

        // Lấy danh sách khóa học đã đăng ký
        public async Task<List<Course>> GetEnrolledCoursesAsync(string studentId)
        {
            try
            {
                var enrolledCourses = await _context.CourseEnrollments
                    .Where(e => e.StudentID == studentId)
                    .Join(_context.Courses,
                          enrollment => enrollment.CourseID,
                          course => course.CourseID,
                          (enrollment, course) => course)
                    .ToListAsync();

                return enrolledCourses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy khóa học đã đăng ký: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
