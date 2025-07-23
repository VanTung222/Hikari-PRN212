using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.Services
{
    public class PasswordResetService
    {
        private static Dictionary<string, (string Code, DateTime Expiry)> _resetCodes = new Dictionary<string, (string, DateTime)>();

        // Generate a 6-digit reset code
        public string GenerateResetCode(string email)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(15); // Code expires in 15 minutes

            _resetCodes[email] = (code, expiry);
            return code;
        }

        // Verify reset code
        public bool VerifyResetCode(string email, string code)
        {
            if (!_resetCodes.ContainsKey(email))
                return false;

            var (storedCode, expiry) = _resetCodes[email];
            
            if (DateTime.Now > expiry)
            {
                _resetCodes.Remove(email);
                return false;
            }

            return storedCode == code;
        }

        // Remove used reset code
        public void RemoveResetCode(string email)
        {
            _resetCodes.Remove(email);
        }

        // Clean expired codes (should be called periodically)
        public void CleanExpiredCodes()
        {
            var expiredKeys = _resetCodes
                .Where(kvp => DateTime.Now > kvp.Value.Expiry)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _resetCodes.Remove(key);
            }
        }

        // Simulate sending email (in real app, you would integrate with email service)
        public async Task<(bool Success, string Code)> SendResetCodeAsync(string email, string code)
        {
            // In a real application, you would send email here
            // For demo purposes, we'll just return true with the code
            await Task.Delay(1000); // Simulate network delay
            
            // Return success and the code for display in UI layer
            return (true, code);
        }
    }
}
