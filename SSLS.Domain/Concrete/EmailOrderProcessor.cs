using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSLS.Domain.Abstract;
using System.Configuration;
using System.Net;
using System.Net.Mail;


namespace SSLS.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = ConfigurationManager.AppSettings["MailToAddress"];
        public string MailFromAddress = ConfigurationManager.AppSettings["MailFromAddress"];
        public bool UseSsl = bool.Parse(ConfigurationManager.AppSettings["UseSsl" ?? "true"]);
        public string Username = ConfigurationManager.AppSettings["Username"];
        public string Password = ConfigurationManager.AppSettings["Password"];
        public string ServerName = ConfigurationManager.AppSettings["ServerName"];
        public int ServerPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"] ?? "587");

    }
    public class EmailOrderProcessor : MailProcessor
    {
        private EmailSettings emailSettings;
        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(RegisterDetail registerDetail)
        {
            SmtpClient smtp = new SmtpClient();
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                StringBuilder body = new StringBuilder();

                body.AppendLine("---")
                    .AppendLine("用户注册信息:")
                    .AppendLine(string.Format("账户名：{0}  密码:{1}", registerDetail.Name, registerDetail.Password));

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress,
                    emailSettings.MailToAddress,
                    string.Format("注册日期：{0}", System.DateTime.Now.ToString()),
                    body.ToString());
                mailMessage.BodyEncoding = Encoding.UTF8;
                smtpClient.Send(mailMessage);
            }
        }
    }


}
