using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebStatusCheckerService.Model;

namespace WebStatusCheckerService
{
    public partial class Service1 : ServiceBase
    {
        private Timer Schedular;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ReadCheckWebSite();
            this.ScheduleService();
        }

        protected override void OnStop()
        {
            this.Schedular.Dispose();
        }

        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));

                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;

                //Get the Interval in Minutes from AppSettings.
                scheduledTime = CalculateInterval();

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                this.WriteToFile("hi, .Simple Service scheduled to run after: " + schedule + " {0}");

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToFile("Simple Service Error on: {0} " + ex.Message + ex.StackTrace);

                //Stop the Windows Service.
                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                {
                    serviceController.Stop();
                }
            }
        }

        private static DateTime CalculateInterval()
        {
            DateTime scheduledTime;
            int intervalMinutes = Convert.ToInt32(5);

            //Set the Scheduled Time by adding the Interval to Current Time.
            scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
            if (DateTime.Now > scheduledTime)
            {
                //If Scheduled Time is passed set Schedule for the next Interval.
                scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
            }

            return scheduledTime;
        }

        private void SchedularCallback(object e)
        {
            this.WriteToFile("Simple Service Log: {0}");
            this.ReadCheckWebSite();
            this.ScheduleService();
        }

        private void ReadCheckWebSite()
        {
            var parameters = this.ReadSiteList();
            this.WriteToFile("Parameters: Count:"+ parameters.Sites.Count + " parameters.Sites.Url: " + parameters.Sites[0].Url);
            if (parameters.Sites.Any())
            {
                var senderMailId = parameters.SenderMailId;
                var senderMailPassword = parameters.SenderMailPassword;
                foreach (var site in parameters.Sites)
                {
                    var response = PingWebSite(site);
                    this.WriteToFile("response: " + response.Result.IsSuccessStatusCode);
                    if (!response.Result.IsSuccessStatusCode)
                    {
                        SendFailureMail(site, senderMailId, senderMailPassword);
                    }
                }
            }
        }

        public void SendFailureMail(Site site, string senderId, string userpassword)
        {
            this.WriteToFile("senderId: " + senderId);
            this.WriteToFile("userpassword: " + userpassword);

            String userName = senderId;
            String password = userpassword;
            var receiverList = site.ReceiverList.Split(',');
            MailMessage msg = new MailMessage(senderId, receiverList[0]);
            this.WriteToFile(" receiverList[0]: " + receiverList[0]);
            msg.Subject = "Your Site is down - " + site.Name;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Site Name: " + site.Name);
            sb.AppendLine("Site URL: " + site.Url);
            msg.Body = sb.ToString();
            SmtpClient SmtpClient = new SmtpClient();
            SmtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
            SmtpClient.Host = "smtp.office365.com";
            SmtpClient.Port = 587;
            SmtpClient.EnableSsl = true;
            SmtpClient.Send(msg);
        }

        public async Task<HttpResponseMessage> PingWebSite(Site site)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            try
            {
                HttpClient Client = new HttpClient();
                var result = await Client.GetAsync(site.Url);
                return result;
            }
            catch (Exception ex)
            {
                this.WriteToFile("Exception in Ping: " + ex);
            }
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
            };
        }

        private Parameters ReadSiteList()
        {
            try
            {
                var paramJson = ConfigurationManager.AppSettings["Parameters"];
                var parameters = JsonConvert.DeserializeObject<Parameters>(paramJson);
                return parameters;
            }
            catch (Exception ex)
            {
                WriteToFile("Exception:  " + ex);
                return new Parameters();
            }
        }

        private void WriteToFile(string text)
        {
            string path = "C:\\Repo\\ServiceLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }
    }
}
