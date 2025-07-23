using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HikariBusiness.Services.payment
{
    public class DiscountService : IDisposable
    {
        private readonly HikariContext _context;
        private bool _disposed = false;

        public DiscountService(HikariContext context)
        {
            _context = context;
        }
        
        public DiscountService()
        {
            _context = new HikariContext();
        }

        /// <summary>
        /// Validate discount code and return discount info
        /// </summary>
        public async Task<DiscountValidationResult> ValidateDiscountCodeAsync(string code, string courseId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new DiscountValidationResult
                    {
                        IsValid = false,
                        Message = "Vui lòng nhập mã giảm giá"
                    };
                }

                var discount = await _context.Discounts
                    .Include(d => d.Course)
                    .FirstOrDefaultAsync(d => d.Code.ToLower() == code.ToLower() && d.IsActive == true);

                if (discount == null)
                {
                    return new DiscountValidationResult
                    {
                        IsValid = false,
                        Message = "Mã giảm giá không tồn tại hoặc đã hết hạn"
                    };
                }

                // Check date validity
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (discount.StartDate.HasValue && today < discount.StartDate.Value)
                {
                    return new DiscountValidationResult
                    {
                        IsValid = false,
                        Message = $"Mã giảm giá chưa có hiệu lực. Có hiệu lực từ {discount.StartDate.Value:dd/MM/yyyy}"
                    };
                }

                if (discount.EndDate.HasValue && today > discount.EndDate.Value)
                {
                    return new DiscountValidationResult
                    {
                        IsValid = false,
                        Message = $"Mã giảm giá đã hết hạn vào {discount.EndDate.Value:dd/MM/yyyy}"
                    };
                }

                // Check if discount applies to specific course
                if (!string.IsNullOrEmpty(courseId) && discount.CourseId != courseId)
                {
                    return new DiscountValidationResult
                    {
                        IsValid = false,
                        Message = $"Mã giảm giá chỉ áp dụng cho khóa học: {discount.Course.Title}"
                    };
                }

                return new DiscountValidationResult
                {
                    IsValid = true,
                    Discount = discount,
                    Message = $"Áp dụng thành công! Giảm {discount.DiscountPercent}% cho khóa học {discount.Course.Title}"
                };
            }
            catch (Exception ex)
            {
                return new DiscountValidationResult
                {
                    IsValid = false,
                    Message = $"Lỗi khi kiểm tra mã giảm giá: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get all active discounts
        /// </summary>
        public async Task<List<Discount>> GetActiveDiscountsAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                
                return await _context.Discounts
                    .Include(d => d.Course)
                    .Where(d => d.IsActive == true && 
                               (!d.StartDate.HasValue || d.StartDate.Value <= today) &&
                               (!d.EndDate.HasValue || d.EndDate.Value >= today))
                    .OrderBy(d => d.Code)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Discount>();
            }
        }

        /// <summary>
        /// Get discounts for specific course
        /// </summary>
        public async Task<List<Discount>> GetDiscountsForCourseAsync(string courseId)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                
                return await _context.Discounts
                    .Include(d => d.Course)
                    .Where(d => d.CourseId == courseId && 
                               d.IsActive == true && 
                               (!d.StartDate.HasValue || d.StartDate.Value <= today) &&
                               (!d.EndDate.HasValue || d.EndDate.Value >= today))
                    .OrderByDescending(d => d.DiscountPercent)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Discount>();
            }
        }

        /// <summary>
        /// Calculate discount amount
        /// </summary>
        public decimal CalculateDiscountAmount(decimal originalPrice, int discountPercent)
        {
            if (discountPercent <= 0 || discountPercent > 100)
                return 0;

            return originalPrice * discountPercent / 100;
        }

        /// <summary>
        /// Calculate final price after discount
        /// </summary>
        public decimal CalculateFinalPrice(decimal originalPrice, int discountPercent)
        {
            var discountAmount = CalculateDiscountAmount(originalPrice, discountPercent);
            return originalPrice - discountAmount;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }

    public class DiscountValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public Discount? Discount { get; set; }
    }
}
