using OrderManagementService.Domain.Services;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagementService.Infrastructure.Helper
{
    public class Md5PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
