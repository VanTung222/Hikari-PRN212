using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class ReviewService
    {
        private readonly HikariContext _context;

        public ReviewService()
        {
            _context = new HikariContext();
        }

        public ReviewService(HikariContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewViewModel>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _context.CourseReviews
                    .Include(r => r.User)
                    .Include(r => r.Course)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToListAsync(); // fetch data first

                return reviews.Select(r => new ReviewViewModel
                {
                    Id = r.Id.ToString(),
                    StudentName = r.User.FullName,
                    CourseName = r.Course.Title,
                    Rating = r.Rating?.ToString() ?? "0",
                    Comment = r.ReviewText ?? "",
                    ReviewDate = r.ReviewDate.HasValue
                        ? r.ReviewDate.Value.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy")
                        : "",
                    Status = "Đã duyệt"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving reviews: {ex.Message}");
            }
        }

        public async Task<List<ReviewViewModel>> SearchReviewsAsync(string searchTerm, int? minRating = null, int? maxRating = null, DateTime? fromDate = null, DateTime? toDate = null, string status = null)
        {
            try
            {
                var query = _context.CourseReviews
                    .Include(r => r.User)
                    .Include(r => r.Course)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(r => r.User.FullName.Contains(searchTerm) ||
                                             r.Course.Title.Contains(searchTerm) ||
                                             r.ReviewText.Contains(searchTerm));
                }

                if (minRating.HasValue)
                {
                    query = query.Where(r => r.Rating >= minRating.Value);
                }

                if (maxRating.HasValue)
                {
                    query = query.Where(r => r.Rating <= maxRating.Value);
                }

                if (fromDate.HasValue)
                {
                    var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                    query = query.Where(r => r.ReviewDate >= fromDateOnly);
                }

                if (toDate.HasValue)
                {
                    var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                    query = query.Where(r => r.ReviewDate <= toDateOnly);
                }

                var reviews = await query.OrderByDescending(r => r.ReviewDate).ToListAsync();

                return reviews.Select(r => new ReviewViewModel
                {
                    Id = r.Id.ToString(),
                    StudentName = r.User.FullName,
                    CourseName = r.Course.Title,
                    Rating = r.Rating?.ToString() ?? "0",
                    Comment = r.ReviewText ?? "",
                    ReviewDate = r.ReviewDate.HasValue
                        ? r.ReviewDate.Value.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy")
                        : "",
                    Status = "Đã duyệt"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching reviews: {ex.Message}");
            }
        }

        public async Task<List<string>> GetDistinctStudentNamesAsync()
        {
            try
            {
                return await _context.CourseReviews
                    .Include(r => r.User)
                    .Select(r => r.User.FullName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student names: {ex.Message}");
            }
        }

        public async Task<List<string>> GetDistinctCourseNamesAsync()
        {
            try
            {
                return await _context.CourseReviews
                    .Include(r => r.Course)
                    .Select(r => r.Course.Title)
                    .Distinct()
                    .OrderBy(title => title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving course names: {ex.Message}");
            }
        }


        public async Task<ReviewStatistics> GetReviewStatisticsAsync()
        {
            try
            {
                var totalReviews = await _context.CourseReviews.CountAsync();
                var averageRating = await _context.CourseReviews.AverageAsync(r => (double?)r.Rating) ?? 0;
                var fiveStarReviews = await _context.CourseReviews.CountAsync(r => r.Rating == 5);
                var oneStarReviews = await _context.CourseReviews.CountAsync(r => r.Rating == 1);

                return new ReviewStatistics
                {
                    TotalReviews = totalReviews,
                    AverageRating = Math.Round(averageRating, 1),
                    FiveStarReviews = fiveStarReviews,
                    OneStarReviews = oneStarReviews
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving review statistics: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class ReviewViewModel
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
        public string ReviewDate { get; set; }
        public string Status { get; set; }
    }

    public class ReviewStatistics
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int FiveStarReviews { get; set; }
        public int OneStarReviews { get; set; }
    }
}
