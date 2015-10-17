using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Net.Mail;
using SendGrid;


namespace SendMail
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {

            var host = new JobHost();
            var method = typeof(Program).GetMethod("SendWelcome");
            host.Call(method, new { dummy = "dummy" });


            //var host = new JobHost();
            //// The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();
        }

        [NoAutomaticTrigger]
        public static async void SendWelcome(string dummy)
        {
            PetUniteDataContext db = new PetUniteDataContext();

            Console.Write("sendwelcome");

            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("noreply@petsociety.co.za");

            // Add multiple addresses to the To field.
            List<String> recipients = new List<String>
            {
                @"Danie Rossouw <rossouw.daniel@gmail.com>",
                 @"Annette Jacobs <qa@healthinsite.net>"
            };

            myMessage.AddTo(recipients);

            myMessage.Subject = "Test mail from PetSociety";

            //Add the HTML and Text bodies
            string template = "";
            var linqMyMessages = db.sp_GetTemplate(1);
            foreach(var linqMyMessage in linqMyMessages)
            {
                template = linqMyMessage.TemplateText;
            }

            myMessage.Html = template;
            myMessage.Text = "Hello World plain text!";

            // Create network credentials to access your SendGrid account.
            var username = "azure_00db54dbccd19ec61af8bb92193f66ca@azure.com";
            var pswd = "Lappies12";

            var credentials = new NetworkCredential(username, pswd);

            // Create an Web transport for sending email, using credentials...
            var transportWeb = new Web(credentials);

            // Send the email. 
            try
            {
                await transportWeb.DeliverAsync(myMessage);
                Console.WriteLine("Sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR = " + ex.Message);
            }
        }
    }
}
