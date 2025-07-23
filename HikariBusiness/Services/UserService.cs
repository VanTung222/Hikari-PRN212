using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class UserService
    {
        private readonly HikariContext _context;

        public UserService()
        {
            _context = new HikariContext();
            EnsureDatabaseCreated();
        }

        public UserService(HikariContext context)
        {
            _context = context;
        }

        // Ensure database is created
        private void EnsureDatabaseCreated()
        {
            try
            {
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent app crash
                System.Diagnostics.Debug.WriteLine($"Database creation error: {ex.Message}");
            }
        }

        // Hash password using SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Generate unique user ID
        private string GenerateUserId()
        {
            var lastUser = _context.UserAccounts.OrderByDescending(u => u.UserId).FirstOrDefault();
            if (lastUser == null)
            {
                return "U001";
            }
            
            int lastNumber = int.Parse(lastUser.UserId.Substring(1));
            return $"U{(lastNumber + 1):D3}";
        }

        // Generate unique student ID
        private string GenerateStudentId()
        {
            var lastStudent = _context.Students.OrderByDescending(s => s.StudentId).FirstOrDefault();
            if (lastStudent == null)
            {
                return "S001";
            }
            
            int lastNumber = int.Parse(lastStudent.StudentId.Substring(1));
            return $"S{(lastNumber + 1):D3}";
        }

        // Generate unique teacher ID
        private string GenerateTeacherId()
        {
            var lastTeacher = _context.Teachers.OrderByDescending(t => t.TeacherId).FirstOrDefault();
            if (lastTeacher == null)
            {
                return "T001";
            }
            
            int lastNumber = int.Parse(lastTeacher.TeacherId.Substring(1));
            return $"T{(lastNumber + 1):D3}";
        }

        // Register new user (Student or Teacher)
        public async Task<(bool Success, string Message, string UserId)> RegisterUserAsync(
            string username, string fullName, string email, string password, 
            string role, string phone = null, DateOnly? birthDate = null, 
            string specialization = null, int? experienceYears = null)
        {
            try
            {
                // Check if email already exists
                if (await _context.UserAccounts.AnyAsync(u => u.Email == email))
                {
                    return (false, "Email đã được sử dụng!", null);
                }

                // Check if username already exists
                if (await _context.UserAccounts.AnyAsync(u => u.Username == username))
                {
                    return (false, "Tên đăng nhập đã được sử dụng!", null);
                }

                // Create user account
                var userId = GenerateUserId();
                var userAccount = new UserAccount
                {
                    UserId = userId,
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    Password = HashPassword(password),
                    Role = role,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                    Phone = phone,
                    BirthDate = birthDate
                };

                _context.UserAccounts.Add(userAccount);

                // Create Student or Teacher record
                if (role.ToLower() == "student")
                {
                    var student = new Student
                    {
                        StudentId = GenerateStudentId(),
                        UserId = userId
                    };
                    _context.Students.Add(student);
                }
                else if (role.ToLower() == "teacher")
                {
                    var teacher = new Teacher
                    {
                        TeacherId = GenerateTeacherId(),
                        UserId = userId,
                        Specialization = specialization,
                        ExperienceYears = experienceYears ?? 0
                    };
                    _context.Teachers.Add(teacher);
                }

                await _context.SaveChangesAsync();
                return (true, "Đăng ký thành công!", userId);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        // Login user
        public async Task<(bool Success, string Message, UserAccount User)> LoginAsync(string usernameOrEmail, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Attempting login for: {usernameOrEmail}");
                
                // First, try to find user by username or email
                var user = await _context.UserAccounts
                    .Include(u => u.Student)
                    .Include(u => u.Teacher)
                    .FirstOrDefaultAsync(u => 
                        u.Username == usernameOrEmail || u.Email == usernameOrEmail);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] User not found: {usernameOrEmail}");
                    return (false, "Tên đăng nhập/Email không tồn tại!", null);
                }

                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] User found: {user.Username}, stored password: {user.Password}");
                
                // Try both plain text and hashed password for compatibility
                var hashedPassword = HashPassword(password);
                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Input password: {password}, hashed: {hashedPassword}");
                
                bool passwordMatch = user.Password == password || user.Password == hashedPassword;
                
                if (!passwordMatch)
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Password mismatch. Stored: {user.Password}, Plain: {password}, Hashed: {hashedPassword}");
                    return (false, "Mật khẩu không đúng!", null);
                }

                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Login successful for: {user.Username}");
                return (true, "Đăng nhập thành công!", user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[LOGIN DEBUG] Stack trace: {ex.StackTrace}");
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        // Get user by email (for forgot password)
        public async Task<UserAccount> GetUserByEmailAsync(string email)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Update password
        public async Task<(bool Success, string Message)> UpdatePasswordAsync(string userId, string newPassword)
        {
            try
            {
                var user = await _context.UserAccounts.FindAsync(userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng!");
                }

                user.Password = HashPassword(newPassword);
                await _context.SaveChangesAsync();
                return (true, "Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // Update user profile
        public async Task<(bool Success, string Message)> UpdateProfileAsync(
            string userId, string fullName, string email, string phone, 
            DateOnly? birthDate, string profilePicture = null)
        {
            try
            {
                var user = await _context.UserAccounts.FindAsync(userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng!");
                }

                // Check if email is being changed and if new email already exists
                if (user.Email != email && await _context.UserAccounts.AnyAsync(u => u.Email == email && u.UserId != userId))
                {
                    return (false, "Email đã được sử dụng bởi người dùng khác!");
                }

                user.FullName = fullName;
                user.Email = email;
                user.Phone = phone;
                user.BirthDate = birthDate;
                if (!string.IsNullOrEmpty(profilePicture))
                {
                    user.ProfilePicture = profilePicture;
                }

                await _context.SaveChangesAsync();
                return (true, "Cập nhật thông tin thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // Update teacher specialization
        public async Task<(bool Success, string Message)> UpdateTeacherSpecializationAsync(
            string userId, string specialization, int experienceYears)
        {
            try
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher == null)
                {
                    return (false, "Không tìm thấy thông tin giáo viên!");
                }

                teacher.Specialization = specialization;
                teacher.ExperienceYears = experienceYears;
                await _context.SaveChangesAsync();
                return (true, "Cập nhật thông tin giáo viên thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // Verify current password
        public async Task<bool> VerifyCurrentPasswordAsync(string userId, string currentPassword)
        {
            var user = await _context.UserAccounts.FindAsync(userId);
            return user != null && user.Password == HashPassword(currentPassword);
        }

        // Get user by ID
        public async Task<UserAccount> GetUserByIdAsync(string userId)
        {
            return await _context.UserAccounts
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
