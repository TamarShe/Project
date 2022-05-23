using BL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public static class SendEmail
    {
        public static int SendTemporaryPassword(string emailAddress)
        {
            Random random = new Random();
            int temporaryPassword = random.Next(10000, 99999);
            if (emailAddress == null)
                return 0;
            try
            {
                string email = "4321vz@gmail.com";
                string password = "vz**4321";
                var loginInfo = new NetworkCredential(email, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress(emailAddress));
                string sInsert = "סיסמה זמנית ממערך ההתנדבות";
                msg.Subject = sInsert;
                #region buildHtmlMessageBody
                string htmlBodyString = string.Format(
                      @"
                      <div style='  direction: rtl;
                                    background-color: 'red';
                                    font - family: Amerald;
                                    font - size:medium; '>
                              <h2>היי!</h2>
                         
                          <div style='  position: relative;
                                        padding: 0.95rem 1.25rem;
                                        margin-bottom: 1rem;
                                        margin-left: 10%;
                                        margin-right:10%;
                                        color:'yellow'ss;
                                        width: 75%;
                                        '>
                              <label>סיסמת האימות שלך היא "+temporaryPassword+@"</label>
                          </div>"
                       );
                #endregion
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBodyString, null, MediaTypeNames.Text.Html);
                msg.AlternateViews.Add(alternateView);
                msg.IsBodyHtml = true;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
            }
            catch
            {
            }
            return temporaryPassword;
        }

        public static bool SendToManager(int volunteeringDetailsCode, string emailContent, string subject)
        {
            ManagerBL managerBL = new ManagerBL();
            VolunteeringDetailsBL volunteeringDetailsBL = new VolunteeringDetailsBL();
            int orgCode = volunteeringDetailsBL.GetAllVolunteeringDetails().Find(a=>a.volunteering_details_code==volunteeringDetailsCode).org_code;
            string managerEmail = managerBL.GetAllManagers().Find(a => a.manager_org_code == orgCode).manager_email;
            if (managerEmail == null || managerEmail=="")
                return false;
            try
            {
                string email = "4321vz@gmail.com";
                string password = "vz**4321";
                var loginInfo = new NetworkCredential(email, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress(managerEmail));
                msg.Subject = subject;
                #region buildHtmlMessageBody
                string htmlBodyString = string.Format(@"<h2>"+emailContent + @"</h2>");
                #endregion
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBodyString, null, MediaTypeNames.Text.Html);
                msg.AlternateViews.Add(alternateView);
                msg.IsBodyHtml = true;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
            }
            catch
            {
            }
            return true;
        }
    }
}