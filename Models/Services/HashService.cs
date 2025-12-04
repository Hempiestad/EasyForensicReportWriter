using System.Security.Cryptography;
using System.Text;

namespace EasyForensicReportWriter.Services
{
    public static class HashService
    {
        public static (string Md5, string Sha256) ComputeHashes(byte[] data)
        {
            using var md5 = MD5.Create();
            using var sha256 = SHA256.Create();
            var md5Hash = BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
            var sha256Hash = BitConverter.ToString(sha256.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
            return (md5Hash, sha256Hash);
        }
    }
}