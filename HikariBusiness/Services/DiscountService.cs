using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class DiscountService
    {
        private readonly HikariContext _context;

        public DiscountService()
        {
            _context = new HikariContext();
        }

        public DiscountService(HikariContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountViewModel>> GetAllDiscountsAsync()
        {
            try
            {
                var discountData = await _context.Discounts
                    .Include(d => d.Course)
                    .ToListAsync();

                var discounts = discountData
                    .Select(d => new DiscountViewModel
                    {
                        Id = d.Id.ToString(),
                        Code = d.Code,
                        CourseName = d.Course?.Title ?? "",
                        DiscountPercent = d.DiscountPercent.HasValue ? d.DiscountPercent.Value + "%" : "0%",
                        StartDate = d.StartDate.HasValue ? d.StartDate.Value.ToString("dd/MM/yyyy") : "",
                        EndDate = d.EndDate.HasValue ? d.EndDate.Value.ToString("dd/MM/yyyy") : "",
                        Status = d.IsActive == true ? "Hoạt động" : "Không hoạt động",
                        Type = GetDiscountType(d.DiscountPercent ?? 0)
                    })
                    .OrderByDescending(d => d.StartDate)
                    .ToList();

                return discounts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving discounts: {ex.Message}");
            }
        }

        public async Task<List<DiscountViewModel>> SearchDiscountsAsync(string searchTerm, string type = null, bool? isActive = null)
{
    try
    {
        var query = _context.Discounts
            .Include(d => d.Course)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(d => d.Code.Contains(searchTerm) ||
                                     d.Course.Title.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(type))
        {
            switch (type)
            {
                case "Thấp":
                    query = query.Where(d => d.DiscountPercent < 20);
                    break;
                case "Trung bình":
                    query = query.Where(d => d.DiscountPercent >= 20 && d.DiscountPercent < 50);
                    break;
                case "Cao":
                    query = query.Where(d => d.DiscountPercent >= 50);
                    break;
            }
        }

        if (isActive.HasValue)
        {
            query = query.Where(d => d.IsActive == isActive.Value);
        }

        var discountData = await query.ToListAsync();

        var discounts = discountData
            .Select(d => new DiscountViewModel
            {
                Id = d.Id.ToString(),
                Code = d.Code,
                CourseName = d.Course?.Title ?? "",
                DiscountPercent = d.DiscountPercent.HasValue ? d.DiscountPercent.Value + "%" : "0%",
                StartDate = d.StartDate.HasValue ? d.StartDate.Value.ToString("dd/MM/yyyy") : "",
                EndDate = d.EndDate.HasValue ? d.EndDate.Value.ToString("dd/MM/yyyy") : "",
                Status = d.IsActive == true ? "Hoạt động" : "Không hoạt động",
                Type = GetDiscountType(d.DiscountPercent ?? 0)
            })
            .OrderByDescending(d => d.StartDate)
            .ToList();

        return discounts;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error searching discounts: {ex.Message}");
    }
}


        public async Task<DiscountStatistics> GetDiscountStatisticsAsync()
        {
            try
            {
                var totalDiscounts = await _context.Discounts.CountAsync();
                var activeDiscounts = await _context.Discounts.CountAsync(d => d.IsActive == true);
                var expiredDiscounts = await _context.Discounts.CountAsync(d => d.EndDate < DateOnly.FromDateTime(DateTime.Now));
                var upcomingDiscounts = await _context.Discounts.CountAsync(d => d.StartDate > DateOnly.FromDateTime(DateTime.Now));

                return new DiscountStatistics
                {
                    TotalDiscounts = totalDiscounts,
                    ActiveDiscounts = activeDiscounts,
                    ExpiredDiscounts = expiredDiscounts,
                    UpcomingDiscounts = upcomingDiscounts
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving discount statistics: {ex.Message}");
            }
        }

        private string GetDiscountType(int discountPercent)
        {
            return discountPercent switch
            {
                < 20 => "Thấp",
                >= 20 and < 50 => "Trung bình",
                _ => "Cao" // không cần case >= 50 riêng
            };
        }


        public async Task<bool> AddDiscountAsync(string code, string courseId, int discountPercent, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Check if discount code already exists
                var existingDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == code);
                
                if (existingDiscount != null)
                {
                    throw new Exception("Mã giảm giá đã tồn tại");
                }

                // Validate course exists
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                {
                    throw new Exception("Khóa học không tồn tại");
                }

                var newDiscount = new Discount
                {
                    Code = code,
                    CourseId = courseId,
                    DiscountPercent = discountPercent,
                    StartDate = DateOnly.FromDateTime(startDate),
                    EndDate = DateOnly.FromDateTime(endDate),
                    IsActive = true
                };

                _context.Discounts.Add(newDiscount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm mã giảm giá: {ex.Message}");
            }
        }

        public async Task<List<CourseViewModel>> GetCoursesForDiscountAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .Where(c => c.IsActive == true)
                    .Select(c => new CourseViewModel
                    {
                        Id = c.CourseId,
                        Title = c.Title
                    })
                    .ToListAsync();

                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khóa học: {ex.Message}");
            }
        }

        public async Task<DiscountViewModel> GetDiscountByIdAsync(string discountId)
        {
            try
            {
                var discount = await _context.Discounts
                    .Include(d => d.Course)
                    .FirstOrDefaultAsync(d => d.Id.ToString() == discountId);

                if (discount == null)
                    return null;

                return new DiscountViewModel
                {
                    Id = discount.Id.ToString(),
                    Code = discount.Code,
                    CourseName = discount.Course?.Title ?? "",
                    DiscountPercent = discount.DiscountPercent.HasValue ? discount.DiscountPercent.Value + "%" : "0%",
                    StartDate = discount.StartDate.HasValue ? discount.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = discount.EndDate.HasValue ? discount.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    Status = discount.IsActive == true ? "Hoạt động" : "Không hoạt động",
                    Type = GetDiscountType(discount.DiscountPercent ?? 0)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving discount by ID: {ex.Message}");
            }
        }

        public async Task<bool> UpdateDiscountAsync(string discountId, string code, string courseId, int discountPercent, DateTime startDate, DateTime endDate)
        {
            try
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id.ToString() == discountId);
                if (discount == null)
                    return false;

                discount.Code = code;
                discount.CourseId = courseId;
                discount.DiscountPercent = discountPercent;
                discount.StartDate = DateOnly.FromDateTime(startDate);
                discount.EndDate = DateOnly.FromDateTime(endDate);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating discount: {ex.Message}");
            }
        }

        public async Task<bool> DeactivateDiscountAsync(string discountId)
        {
            try
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id.ToString() == discountId);
                if (discount == null)
                    return false;

                discount.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deactivating discount: {ex.Message}");
            }
        }

        public async Task<bool> ActivateDiscountAsync(string discountId)
        {
            try
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id.ToString() == discountId);
                if (discount == null)
                    return false;

                discount.IsActive = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error activating discount: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class DiscountViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string CourseName { get; set; }
        public string DiscountPercent { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }

    public class DiscountStatistics
    {
        public int TotalDiscounts { get; set; }
        public int ActiveDiscounts { get; set; }
        public int ExpiredDiscounts { get; set; }
        public int UpcomingDiscounts { get; set; }
    }
}
