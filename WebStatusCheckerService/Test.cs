using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using NUnit.Framework;
using WebStatusCheckerService.Model;

namespace WebStatusCheckerService
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void PingWebSite()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            HttpClient Client = new HttpClient();
            var result = Client.GetAsync("https://divyesh.hybr.com");
            int StatusCode = (int)result.Result.StatusCode;

        }

        [Test]
        public void readParameters()
        {
            var txt = File.ReadAllText(@"C:\Repo\WebStatusCheckerService\WebStatusCheckerService\WebStatusCheckerService\Parameters.json");
            var x = JsonConvert.DeserializeObject<Parameters>(txt);
        }

        [Test]
        public void Sendmail()
        {
            String userName = "divyeshg@cloudassert.com";
            String password = "!!!";
            MailMessage msg = new MailMessage("divyeshg@cloudassert.com", "divyeshg@cloudassert.com");
            msg.Subject = "Your Subject Name";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name: ");
            msg.Body = sb.ToString();
            SmtpClient SmtpClient = new SmtpClient();
            SmtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
            SmtpClient.Host = "smtp.office365.com";
            SmtpClient.Port = 587;
            SmtpClient.EnableSsl = true;
            SmtpClient.Send(msg);
        }

        [Test]
        public void timer()
        {
            const double interval60Minutes = 10 * 1000; // milliseconds to one hour

            Timer checkForTime = new Timer(interval60Minutes);
            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            checkForTime.Enabled = true;
        }

        void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if (timeIsReady())
            //{
             Sendmail();
            //}
        }
    }
}
