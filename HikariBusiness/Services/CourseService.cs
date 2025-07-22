using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class CourseService
    {
        private readonly HikariContext _context;

        public CourseService()
        {
            _context = new HikariContext();
        }

        public CourseService(HikariContext context)
        {
            _context = context;
        }

        public async Task<List<CourseViewModel>> GetAllCoursesAsync()
        {
            try
            {
                var coursesData = await _context.Courses
                    .Include(c => c.CourseEnrollments)
                    .Include(c => c.CourseReviews)
                    .ToListAsync();

                var courses = coursesData.Select(c => new CourseViewModel
                {
                    Id = c.CourseId,
                    Title = c.Title,
                    Description = c.Description ?? "",
                    Fee = c.Fee.HasValue ? c.Fee.Value.ToString("N0") + "đ" : "0đ",
                    Duration = c.Duration.HasValue ? c.Duration.Value + " giờ" : "0 giờ",
                    StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    Status = c.IsActive == true ? "Hoạt động" : "Không hoạt động",
                    EnrollmentCount = c.CourseEnrollments?.Count ?? 0,
                    ReviewCount = c.CourseReviews?.Count ?? 0
                }).ToList();

                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving courses: {ex.Message}");
            }
        }

        public async Task<List<CourseViewModel>> SearchCoursesAsync(string courseName, decimal? minFee, decimal? maxFee, DateTime? startDate)
        {
            try
            {
                var query = _context.Courses
                    .Include(c => c.CourseEnrollments)
                    .Include(c => c.CourseReviews)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(courseName))
                {
                    query = query.Where(c => c.Title.Contains(courseName) || c.Description.Contains(courseName));
                }

                if (minFee.HasValue)
                {
                    query = query.Where(c => c.Fee.HasValue && c.Fee.Value >= minFee.Value);
                }

                if (maxFee.HasValue)
                {
                    query = query.Where(c => c.Fee.HasValue && c.Fee.Value <= maxFee.Value);
                }

                if (startDate.HasValue)
                {
                    var startDateOnly = DateOnly.FromDateTime(startDate.Value);
                    query = query.Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDateOnly);
                }

                var courseData = await query.ToListAsync();

                var courses = courseData.Select(c => new CourseViewModel
                {
                    Id = c.CourseId,
                    Title = c.Title,
                    Description = c.Description ?? "",
                    Fee = c.Fee.HasValue ? c.Fee.Value.ToString("N0") + "đ" : "0đ",
                    Duration = c.Duration.HasValue ? c.Duration.Value + " giờ" : "0 giờ",
                    StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    Status = c.IsActive == true ? "Hoạt động" : "Không hoạt động",
                    EnrollmentCount = c.CourseEnrollments?.Count ?? 0,
                    ReviewCount = c.CourseReviews?.Count ?? 0
                }).ToList();

                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching courses: {ex.Message}");
            }
        }

        public async Task<CourseStatistics> GetCourseStatisticsAsync()
        {
            try
            {
                var totalCourses = await _context.Courses.CountAsync();
                var activeCourses = await _context.Courses.CountAsync(c => c.IsActive == true);
                var totalEnrollments = await _context.CourseEnrollments.CountAsync();
                var totalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == "Completed")
                    .SumAsync(p => p.Amount);

                return new CourseStatistics
                {
                    TotalCourses = totalCourses,
                    ActiveCourses = activeCourses,
                    TotalEnrollments = totalEnrollments,
                    TotalRevenue = totalRevenue
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving course statistics: {ex.Message}");
            }
        }

        public async Task<bool> AddCourseAsync(string title, string description, decimal fee, int duration, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new Exception("Tên khóa học không được để trống.");
                if (fee < 0)
                    throw new Exception("Học phí không được âm.");
                if (duration <= 0)
                    throw new Exception("Thời lượng phải là số nguyên dương.");

                var existingCourse = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Title == title);

                if (existingCourse != null)
                {
                    throw new Exception("Tên khóa học đã tồn tại");
                }

                string courseId = await GenerateCourseIdAsync();

                var newCourse = new Course
                {
                    CourseId = courseId,
                    Title = title,
                    Description = description,
                    Fee = fee,
                    Duration = duration,
                    StartDate = DateOnly.FromDateTime(startDate),
                    EndDate = DateOnly.FromDateTime(endDate),
                    IsActive = true
                };

                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm khóa học: {ex.Message}");
            }
        }

        private async Task<string> GenerateCourseIdAsync()
        {
            var maxCourse = await _context.Courses
                .Where(c => c.CourseId.StartsWith("CO"))
                .OrderByDescending(c => c.CourseId)
                .FirstOrDefaultAsync();

            int maxNumber = maxCourse != null && maxCourse.CourseId.Length == 5 && int.TryParse(maxCourse.CourseId.Substring(2), out int number) ? number : 0;
            return $"CO{(maxNumber + 1):D3}";
        }

        public async Task<CourseViewModel> GetCourseByIdAsync(string courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.CourseEnrollments)
                    .Include(c => c.CourseReviews)
                    .FirstOrDefaultAsync(c => c.CourseId == courseId);

                if (course == null)
                    return null;

                return new CourseViewModel
                {
                    Id = course.CourseId,
                    Title = course.Title,
                    Description = course.Description ?? "",
                    Fee = course.Fee.HasValue ? course.Fee.Value.ToString("N0") + "đ" : "0đ",
                    Duration = course.Duration.HasValue ? course.Duration.Value + " giờ" : "0 giờ",
                    StartDate = course.StartDate.HasValue ? course.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = course.EndDate.HasValue ? course.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    Status = course.IsActive == true ? "Hoạt động" : "Không hoạt động",
                    EnrollmentCount = course.CourseEnrollments?.Count ?? 0,
                    ReviewCount = course.CourseReviews?.Count ?? 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving course by ID: {ex.Message}");
            }
        }

        public async Task<bool> UpdateCourseAsync(string courseId, string title, string description, decimal fee, int duration, DateTime startDate, DateTime endDate)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                    return false;

                course.Title = title;
                course.Description = description;
                course.Fee = fee;
                course.Duration = duration;
                course.StartDate = DateOnly.FromDateTime(startDate);
                course.EndDate = DateOnly.FromDateTime(endDate);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating course: {ex.Message}");
            }
        }

        public async Task<bool> DeactivateCourseAsync(string courseId)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                    return false;

                course.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deactivating course: {ex.Message}");
            }
        }

        public async Task<bool> ActivateCourseAsync(string courseId)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                    return false;

                course.IsActive = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error activating course: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class CourseViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Fee { get; set; }
        public string Duration { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public int EnrollmentCount { get; set; }
        public int ReviewCount { get; set; }
    }

    public class CourseStatistics
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}