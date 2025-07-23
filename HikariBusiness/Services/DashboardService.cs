using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class DashboardService
    {
        private readonly HikariContext _context;

        public DashboardService()
        {
            _context = new HikariContext();
        }

        public DashboardService(HikariContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
        {
            try
            {
                var totalUsers = await _context.UserAccounts.CountAsync();
                var totalCourses = await _context.Courses.CountAsync();
                var totalPayments = await _context.Payments.CountAsync();
                var totalReviews = await _context.CourseReviews.CountAsync();
                
                var totalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == "Completed")
                    .SumAsync(p => p.Amount);

                var activeStudents = await _context.Students.CountAsync();
                var activeCourses = await _context.Courses.CountAsync(c => c.IsActive == true);

                return new DashboardStatistics
                {
                    TotalUsers = totalUsers,
                    TotalCourses = totalCourses,
                    TotalPayments = totalPayments,
                    TotalReviews = totalReviews,
                    TotalRevenue = totalRevenue,
                    ActiveStudents = activeStudents,
                    ActiveCourses = activeCourses
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving dashboard statistics: {ex.Message}");
            }
        }

        public async Task<List<RecentCourseViewModel>> GetRecentCoursesAsync(int count = 5)
        {
            try
            {
                var recentCoursesData = await _context.Courses
                    .Include(c => c.CourseEnrollments)
                    .OrderByDescending(c => c.StartDate)
                    .Take(count)
                    .ToListAsync();

                var recentCourses = recentCoursesData
                    .Select(c => new RecentCourseViewModel
                    {
                        Title = c.Title,
                        EnrollmentCount = c.CourseEnrollments?.Count ?? 0,
                        StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("dd/MM/yyyy") : "",
                        Status = c.IsActive == true ? "Hoạt động" : "Không hoạt động"
                    })
                    .ToList();

                return recentCourses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent courses: {ex.Message}");
            }
        }

        public async Task<List<NewUserViewModel>> GetNewUsersAsync(int count = 5)
        {
            try
            {
                var newUsersData = await _context.UserAccounts
                    .OrderByDescending(u => u.RegistrationDate)
                    .Take(count)
                    .ToListAsync();

                var newUsers = newUsersData
                    .Select(u => new NewUserViewModel
                    {
                        FullName = u.FullName,
                        Email = u.Email,
                        Role = u.Role,
                        RegistrationDate = u.RegistrationDate.HasValue ? u.RegistrationDate.Value.ToString("dd/MM/yyyy") : ""
                    })
                    .ToList();

                return newUsers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving new users: {ex.Message}");
            }
        }

        public async Task<List<MonthlyRevenueData>> GetMonthlyRevenueDataAsync(int months = 12)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddMonths(-months);

                var revenueData = await _context.Payments
                    .Where(p => p.PaymentStatus == "Completed" && p.PaymentDate.HasValue && p.PaymentDate >= startDate)
                    .ToListAsync();

                var monthlyRevenue = revenueData
                    .GroupBy(p => new { p.PaymentDate.Value.Year, p.PaymentDate.Value.Month })
                    .Select(g => new MonthlyRevenueData
                    {
                        Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Revenue = g.Sum(p => p.Amount)
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                return monthlyRevenue;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving monthly revenue data: {ex.Message}");
            }
        }

        public async Task<List<UserGrowthData>> GetUserGrowthDataAsync(int months = 12)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddMonths(-months);

                var userData = await _context.UserAccounts
                    .Where(u => u.RegistrationDate.HasValue && u.RegistrationDate >= DateOnly.FromDateTime(startDate))
                    .ToListAsync();

                var userGrowth = userData
                    .GroupBy(u => new { u.RegistrationDate.Value.Year, u.RegistrationDate.Value.Month })
                    .Select(g => new UserGrowthData
                    {
                        Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                        UserCount = g.Count()
                    })
                    .OrderBy(u => u.Month)
                    .ToList();

                return userGrowth;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user growth data: {ex.Message}");
            }
        }


        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class DashboardStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalPayments { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveStudents { get; set; }
        public int ActiveCourses { get; set; }
    }

    public class RecentCourseViewModel
    {
        public string Title { get; set; }
        public int EnrollmentCount { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
    }

    public class NewUserViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class UserGrowthData
    {
        public string Month { get; set; }
        public int UserCount { get; set; }
    }
}
