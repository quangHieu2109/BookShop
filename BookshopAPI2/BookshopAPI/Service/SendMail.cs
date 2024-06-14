using System.Net.Mail;

namespace BookshopAPI.Service
{
    public class SendMail
    {

        public void SendEmail(string to, string otp)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("fitstore567@gmail.com");
            message.To.Add(new MailAddress(to));
            message.Subject = "OTP đặt lại mật khẩu cho BookShop";
            message.Body = "OTP đặt lại mật khẩu cho tài khoản của bạn là: "+otp+"\n"+"OTP sẽ hết hạn sau 5 phút";

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("fitstore567@gmail.com", "cgkf kchb iaxe egwh");
            client.EnableSsl = true;
            client.Send(message);

            Console.WriteLine("Email sent successfully!");
        }

    }
}
