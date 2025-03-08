using BCrypt.Net;

namespace BackendMultiChat.Helpers
{
    public static class PasswordHelper
    {
        // Hash password using BCript
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Check if password is valid
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
