using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace HikariBusiness.Services
{
    public class DatabaseTestService
    {
        private readonly HikariContext _context;

        public DatabaseTestService()
        {
            _context = new HikariContext();
        }

        public async Task<string> TestDatabaseConnection()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DB TEST] Testing database connection...");
                
                // Test connection
                await _context.Database.OpenConnectionAsync();
                System.Diagnostics.Debug.WriteLine("[DB TEST] Database connection successful!");
                
                // Check if UserAccount table exists and has data
                var userCount = await _context.UserAccounts.CountAsync();
                System.Diagnostics.Debug.WriteLine($"[DB TEST] UserAccount table has {userCount} records");
                
                if (userCount > 0)
                {
                    // Get first few users for debugging
                    var users = await _context.UserAccounts
                        .Take(5)
                        .Select(u => new { u.UserId, u.Username, u.Email, u.FullName })
                        .ToListAsync();
                    
                    System.Diagnostics.Debug.WriteLine("[DB TEST] Sample users:");
                    foreach (var user in users)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DB TEST] - ID: {user.UserId}, Username: {user.Username}, Email: {user.Email}, Name: {user.FullName}");
                    }
                    
                    return $"Database connected successfully! Found {userCount} users.";
                }
                else
                {
                    return "Database connected but UserAccount table is empty!";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB TEST] Error: {ex.Message}");
                return $"Database connection failed: {ex.Message}";
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<string> SearchUser(string usernameOrEmail)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DB TEST] Searching for user: {usernameOrEmail}");
                
                var user = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => 
                        u.Username == usernameOrEmail || u.Email == usernameOrEmail);
                
                if (user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DB TEST] User found: {user.Username} ({user.Email})");
                    return $"User found: {user.Username} ({user.Email})";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DB TEST] User not found: {usernameOrEmail}");
                    
                    // Try case-insensitive search
                    var userCaseInsensitive = await _context.UserAccounts
                        .FirstOrDefaultAsync(u => 
                            u.Username.ToLower() == usernameOrEmail.ToLower() || 
                            u.Email.ToLower() == usernameOrEmail.ToLower());
                    
                    if (userCaseInsensitive != null)
                    {
                        return $"User found with case-insensitive search: {userCaseInsensitive.Username} ({userCaseInsensitive.Email})";
                    }
                    
                    return $"User not found: {usernameOrEmail}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB TEST] Search error: {ex.Message}");
                return $"Search failed: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
