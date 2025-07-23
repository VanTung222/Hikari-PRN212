using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class AccountService
    {
        private readonly HikariContext _context;

        public AccountService()
        {
            _context = new HikariContext();
        }

        public AccountService(HikariContext context)
        {
            _context = context;
        }

        public async Task<List<AccountViewModel>> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await _context.UserAccounts
                    .Include(u => u.Student)
                        .ThenInclude(s => s.CourseEnrollments)
                    .Include(u => u.Teacher)
                    .ToListAsync();

                return accounts.Select(u => new AccountViewModel
                {
                    Id = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    RegistrationDate = u.RegistrationDate != null ? u.RegistrationDate.Value.ToString("dd/MM/yyyy") : "",
                    Phone = u.Phone ?? "",
                    Status = u.IsActive == true ? "Hoạt động" : "Bị khóa",
                    CourseCount = u.Student?.CourseEnrollments?.Count ?? 0,
                    CreatedDate = u.RegistrationDate != null ? u.RegistrationDate.Value.ToString("dd/MM/yyyy") : ""
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving accounts: {ex.Message}");
            }
        }

        public async Task<List<AccountViewModel>> SearchAccountsAsync(string username, string fullName, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = _context.UserAccounts.AsQueryable();

                // Filter by username
                if (!string.IsNullOrEmpty(username))
                {
                    query = query.Where(u => u.Username.Contains(username));
                }

                // Filter by full name
                if (!string.IsNullOrEmpty(fullName))
                {
                    query = query.Where(u => u.FullName.Contains(fullName));
                }

                // Filter by date range
                if (fromDate.HasValue)
                {
                    var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                    query = query.Where(u => u.RegistrationDate >= fromDateOnly);
                }

                if (toDate.HasValue)
                {
                    var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                    query = query.Where(u => u.RegistrationDate <= toDateOnly);
                }

                var accounts = await query
                    .Include(u => u.Student)
                        .ThenInclude(s => s.CourseEnrollments)
                    .Include(u => u.Teacher)
                    .ToListAsync();

                return accounts.Select(u => new AccountViewModel
                {
                    Id = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    RegistrationDate = u.RegistrationDate != null ? u.RegistrationDate.Value.ToString("dd/MM/yyyy") : "",
                    Phone = u.Phone ?? "",
                    Status = u.IsActive == true ? "Hoạt động" : "Bị khóa",
                    CourseCount = u.Student?.CourseEnrollments?.Count ?? 0,
                    CreatedDate = u.RegistrationDate != null ? u.RegistrationDate.Value.ToString("dd/MM/yyyy") : ""
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching accounts: {ex.Message}");
            }
        }

        public async Task<AccountStatistics> GetAccountStatisticsAsync()
        {
            try
            {
                var totalUsers = await _context.UserAccounts.CountAsync();
                var totalStudents = await _context.Students.CountAsync();
                var totalTeachers = await _context.Teachers.CountAsync();
                var totalAdmins = await _context.UserAccounts.CountAsync(u => u.Role == "Admin");

                return new AccountStatistics
                {
                    TotalUsers = totalUsers,
                    TotalStudents = totalStudents,
                    TotalTeachers = totalTeachers,
                    TotalAdmins = totalAdmins
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving account statistics: {ex.Message}");
            }
        }

        public async Task<bool> AddAccountAsync(string username, string fullName, string email, string password, string role, string phone = null, DateOnly? birthDate = null)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

                if (existingUser != null)
                {
                    throw new Exception("Tên đăng nhập hoặc email đã tồn tại");
                }

                // Generate proper user ID based on role and database constraints
                string userId = await GenerateUserIdAsync(role);

                var newAccount = new UserAccount
                {
                    UserId = userId,
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    Password = password, // In production, this should be hashed
                    Role = role,
                    Phone = phone,
                    BirthDate = birthDate,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.UserAccounts.Add(newAccount);

                // Create corresponding Student or Teacher record based on role
                if (role == "Student")
                {
                    string studentId = await GenerateStudentIdAsync();
                    var student = new Student
                    {
                        StudentId = studentId,
                        UserId = userId
                    };
                    _context.Students.Add(student);
                }
                else if (role == "Teacher")
                {
                    string teacherId = await GenerateTeacherIdAsync();
                    var teacher = new Teacher
                    {
                        TeacherId = teacherId,
                        UserId = userId
                    };
                    _context.Teachers.Add(teacher);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm tài khoản: {ex.Message}");
            }
        }

        private async Task<string> GenerateUserIdAsync(string role)
        {
            string prefix = role switch
            {
                "Student" => "U",
                "Teacher" => "U",
                "Admin" => "U",
                "Coordinator" => "U",
                _ => "U"
            };

            // Get the highest existing user ID with this prefix
            var existingIds = await _context.UserAccounts
                .Where(u => u.UserId.StartsWith(prefix))
                .Select(u => u.UserId)
                .ToListAsync();

            int maxNumber = 0;
            foreach (var id in existingIds)
            {
                if (id.Length == 4 && int.TryParse(id.Substring(1), out int number))
                {
                    maxNumber = Math.Max(maxNumber, number);
                }
            }

            return $"{prefix}{(maxNumber + 1):D3}";
        }

        private async Task<string> GenerateStudentIdAsync()
        {
            var existingIds = await _context.Students
                .Where(s => s.StudentId.StartsWith("S"))
                .Select(s => s.StudentId)
                .ToListAsync();

            int maxNumber = 0;
            foreach (var id in existingIds)
            {
                if (id.Length == 4 && int.TryParse(id.Substring(1), out int number))
                {
                    maxNumber = Math.Max(maxNumber, number);
                }
            }

            return $"S{(maxNumber + 1):D3}";
        }

        private async Task<string> GenerateTeacherIdAsync()
        {
            var existingIds = await _context.Teachers
                .Where(t => t.TeacherId.StartsWith("T"))
                .Select(t => t.TeacherId)
                .ToListAsync();

            int maxNumber = 0;
            foreach (var id in existingIds)
            {
                if (id.Length == 4 && int.TryParse(id.Substring(1), out int number))
                {
                    maxNumber = Math.Max(maxNumber, number);
                }
            }

            return $"T{(maxNumber + 1):D3}";
        }

        public async Task<AccountViewModel> GetAccountByIdAsync(string accountId)
        {
            try
            {
                var account = await _context.UserAccounts
                    .Include(u => u.Student)
                        .ThenInclude(s => s.CourseEnrollments)
                    .Include(u => u.Teacher)
                    .FirstOrDefaultAsync(u => u.UserId == accountId);

                if (account == null)
                    return null;

                return new AccountViewModel
                {
                    Id = account.UserId,
                    Username = account.Username,
                    FullName = account.FullName,
                    Email = account.Email,
                    Role = account.Role,
                    RegistrationDate = account.RegistrationDate?.ToString("dd/MM/yyyy") ?? "",
                    Phone = account.Phone ?? "",
                    Status = account.IsActive == true ? "Hoạt động" : "Bị khóa",
                    CourseCount = account.Student?.CourseEnrollments?.Count ?? 0,
                    CreatedDate = account.RegistrationDate?.ToString("dd/MM/yyyy") ?? ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving account by ID: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAccountAsync(string accountId, string fullName, string email, string phone)
        {
            try
            {
                var account = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == accountId);
                if (account == null)
                    return false;

                account.FullName = fullName;
                account.Email = email;
                account.Phone = phone;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating account: {ex.Message}");
            }
        }

        public async Task<bool> BlockAccountAsync(string accountId)
        {
            try
            {
                var account = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == accountId);
                if (account == null)
                    return false;

                account.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error blocking account: {ex.Message}");
            }
        }

        public async Task<bool> UnblockAccountAsync(string accountId)
        {
            try
            {
                var account = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == accountId);
                if (account == null)
                    return false;

                account.IsActive = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unblocking account: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class AccountViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RegistrationDate { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public int CourseCount { get; set; }
        public string CreatedDate { get; set; }
    }

    public class AccountStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalAdmins { get; set; }
    }
}