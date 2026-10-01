using Microsoft.AspNetCore.Identity;

namespace AuraFitWebService.Utilities
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<object> _hasher = new();

        //here user is a dummy object
        public static string Hash(string password)
        {
            return _hasher.HashPassword("user", password);
        }

        public static bool Verify(string password, string hashed)
        {
            return _hasher.VerifyHashedPassword("user", hashed, password) == PasswordVerificationResult.Success;
        }
    }
}
