using System.Net;
using System.Net.Mail;

namespace DernekSitesi.Services
{
    public class EmailService
    {
        public async Task TopluMailGonderAsync(List<string> aliciMailleri, string konu, string icerik)
        {
            // BURAYA KENDİ TEST GMAIL ADRESİNİ YAZACAKSIN
            string gondericiMail = "polatbilir55@gmail.com";

            // DİKKAT: Buraya normal Gmail şifren DEĞİL, Google'dan alacağımız 16 haneli "Uygulama Şifresi" gelecek
            string uygulamaSifresi = "bldh jckd ticd odfw";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(gondericiMail, uygulamaSifresi),
                EnableSsl = true
            };

            foreach (var alici in aliciMailleri)
            {
                // Herkese tek tek, şık bir HTML mail fırlatıyoruz
                MailMessage mesaj = new MailMessage(gondericiMail, alici, konu, icerik);
                mesaj.IsBodyHtml = true;

                await smtpClient.SendMailAsync(mesaj);
            }
        }
    }
}