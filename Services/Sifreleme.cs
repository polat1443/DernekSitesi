using System.Security.Cryptography;
using System.Text;

namespace DernekSitesi.Services
{
    public static class Sifreleme
    {
        public static string HashSifre(string sifre)
        {
            if (string.IsNullOrEmpty(sifre)) return "";

            using (SHA256 sha256 = SHA256.Create())
            {
                // Şifreyi baytlara ayır ve SHA256 ile karıştır
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sifre));

                // Karışık baytları tekrar metne (string) çevir
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString(); // Kriptolanmış o uzun metni geri ver!
            }
        }
    }
}