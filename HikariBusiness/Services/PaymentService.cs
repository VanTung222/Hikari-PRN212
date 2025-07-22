using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class PaymentService
    {
        private readonly HikariContext _context;

        public PaymentService()
        {
            _context = new HikariContext();
        }

        public PaymentService(HikariContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentViewModel>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _context.Payments
    .Include(p => p.Student)
    .ThenInclude(s => s.User)
    .Include(p => p.Enrollment)
    .ThenInclude(e => e.Course)
    .OrderByDescending(p => p.PaymentDate)
    .ToListAsync(); // ← Truy vấn xong trước

                return payments.Select(p => new PaymentViewModel
                {
                    Id = p.Id.ToString(),
                    PaymentCode = p.TransactionId ?? $"TT{p.Id:D6}",
                    StudentName = p.Student.User != null ? p.Student.User.FullName : "N/A",
                    CourseName = p.Enrollment.Course.Title,
                    Amount = p.Amount.ToString("N0") + "đ",
                    PaymentMethod = GetPaymentMethodDisplay(p.PaymentMethod),
                    Status = GetPaymentStatusDisplay(p.PaymentStatus),
                    PaymentDate = p.PaymentDate?.ToString("dd/MM/yyyy") ?? ""
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payments: {ex.Message}");
            }
        }

        public async Task<List<PaymentViewModel>> SearchPaymentsAsync(string searchTerm, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.Payments
                    .Include(p => p.Student)
                     .ThenInclude(s => s.User)
                    .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Course)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => (p.Student.User != null && p.Student.User.FullName.Contains(searchTerm)) ||
                                           p.Enrollment.Course.Title.Contains(searchTerm) ||
                                           (p.TransactionId != null && p.TransactionId.Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.PaymentStatus == status);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate <= toDate.Value);
                }

                var rawPayments = await query
     .OrderByDescending(p => p.PaymentDate)
     .ToListAsync();

                var payments = rawPayments.Select(p => new PaymentViewModel
                {
                    Id = p.Id.ToString(),
                    PaymentCode = p.TransactionId ?? $"TT{p.Id:D6}",
                    StudentName = p.Student.User != null ? p.Student.User.FullName : "N/A",
                    CourseName = p.Enrollment.Course.Title,
                    Amount = p.Amount.ToString("N0") + "đ",
                    PaymentMethod = GetPaymentMethodDisplay(p.PaymentMethod),
                    Status = GetPaymentStatusDisplay(p.PaymentStatus),
                    PaymentDate = p.PaymentDate?.ToString("dd/MM/yyyy") ?? ""
                }).ToList();

                return payments;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching payments: {ex.Message}");
            }
        }

        public async Task<PaymentStatistics> GetPaymentStatisticsAsync()
        {
            try
            {
                var totalPayments = await _context.Payments.CountAsync();
                var completedPayments = await _context.Payments.CountAsync(p => p.PaymentStatus == "Completed");
                var pendingPayments = await _context.Payments.CountAsync(p => p.PaymentStatus == "Pending");
                var failedPayments = await _context.Payments.CountAsync(p => p.PaymentStatus == "Failed");
                var totalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == "Completed")
                    .SumAsync(p => p.Amount);

                return new PaymentStatistics
                {
                    TotalPayments = totalPayments,
                    CompletedPayments = completedPayments,
                    PendingPayments = pendingPayments,
                    FailedPayments = failedPayments,
                    TotalRevenue = totalRevenue
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payment statistics: {ex.Message}");
            }
        }

        private string GetPaymentMethodDisplay(string paymentMethod)
        {
            return paymentMethod switch
            {
                "BankTransfer" => "Chuyển khoản",
                "Cash" => "Tiền mặt",
                "CreditCard" => "Thẻ tín dụng",
                "EWallet" => "Ví điện tử",
                _ => paymentMethod ?? "Không xác định"
            };
        }

        private string GetPaymentStatusDisplay(string status)
        {
            return status switch
            {
                "Completed" => "Hoàn thành",
                "Pending" => "Đang xử lý",
                "Failed" => "Thất bại",
                "Cancelled" => "Đã hủy",
                _ => status ?? "Không xác định"
            };
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class PaymentViewModel
    {
        public string Id { get; set; }
        public string PaymentCode { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string PaymentDate { get; set; }
    }

    public class PaymentStatistics
    {
        public int TotalPayments { get; set; }
        public int CompletedPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
