using HikariDataAccess.Entities;
using HikariDataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HikariBusiness.Services
{
    public class CartService
    {
        private readonly HikariContext _context;

        public CartService()
        {
            _context = new HikariContext();
        }

        // Thêm khóa học vào giỏ hàng
        public async Task<bool> AddToCartAsync(string studentId, string courseId)
        {
            try
            {
                // Kiểm tra xem khóa học đã có trong giỏ hàng chưa
                var existingItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.StudentID == studentId && c.CourseID == courseId);

                if (existingItem != null)
                {
                    return false; // Đã có trong giỏ hàng
                }

                // Thêm mới vào giỏ hàng
                var cartItem = new Cart
                {
                    StudentID = studentId,
                    CourseID = courseId,
                    AddedDate = DateTime.Now
                };

                _context.Carts.Add(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm vào giỏ hàng: {ex.Message}");
            }
        }

        // Lấy tất cả items trong giỏ hàng của student
        public async Task<List<CartItemViewModel>> GetCartItemsAsync(string studentId)
        {
            try
            {
                var cartItems = await _context.Carts
                    .Where(c => c.StudentID == studentId)
                    .Join(_context.Courses,
                          cart => cart.CourseID,
                          course => course.CourseID,
                          (cart, course) => new CartItemViewModel
                          {
                              CartId = cart.Id,
                              CourseId = course.CourseID,
                              Title = course.Title,
                              Description = course.Description ?? "",
                              Fee = course.Fee ?? 0,
                              FeeFormatted = (course.Fee ?? 0).ToString("N0") + " VNĐ",
                              AddedDate = cart.AddedDate,
                              AddedDateFormatted = cart.AddedDate.ToString("dd/MM/yyyy")
                          })
                    .ToListAsync();

                return cartItems;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách giỏ hàng: {ex.Message}");
            }
        }

        // Đếm số lượng items trong giỏ hàng
        public async Task<int> GetCartCountAsync(string studentId)
        {
            try
            {
                return await _context.Carts
                    .Where(c => c.StudentID == studentId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm items trong giỏ hàng: {ex.Message}");
            }
        }

        // Tính tổng tiền trong giỏ hàng
        public async Task<decimal> GetCartTotalAsync(string studentId)
        {
            try
            {
                var total = await _context.Carts
                    .Where(c => c.StudentID == studentId)
                    .Join(_context.Courses,
                          cart => cart.CourseID,
                          course => course.CourseID,
                          (cart, course) => course.Fee ?? 0)
                    .SumAsync();

                return total;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính tổng tiền giỏ hàng: {ex.Message}");
            }
        }

        // Xóa item khỏi giỏ hàng
        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            try
            {
                var cartItem = await _context.Carts.FindAsync(cartId);
                if (cartItem != null)
                {
                    _context.Carts.Remove(cartItem);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa khỏi giỏ hàng: {ex.Message}");
            }
        }

        // Xóa tất cả items trong giỏ hàng
        public async Task<bool> ClearCartAsync(string studentId)
        {
            try
            {
                var cartItems = await _context.Carts
                    .Where(c => c.StudentID == studentId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.Carts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa giỏ hàng: {ex.Message}");
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
                throw new Exception($"Lỗi khi kiểm tra khóa học trong giỏ hàng: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    // ViewModel cho Cart Item
    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public string CourseId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public string FeeFormatted { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
        public string AddedDateFormatted { get; set; } = string.Empty;
    }
}
