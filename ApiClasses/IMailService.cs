using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
//using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Permissions;

namespace Rent_a_Car.ApiClasses
{
    public interface IMailService
    {
        Task SendMail(string toEmail, string subject, string content);
        Task AddContact();
    }

    public class SendGridMailService : IMailService
    {
        private IConfiguration _configuration;

        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMail(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendGridSenderEmail"], _configuration["SendGridSenderName"]);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task AddContact()
        {
            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);

            var data = @"{
            'contacts': [
                {
                    'email': 'ryan39@lee-young.com'
                }
            ]
        }";

            var response = await client.RequestAsync(
                method: SendGridClient.Method.PUT,
                urlPath: "v3/marketing/contacts",
                requestBody: data
            );
            //Assert.True(HttpStatusCode.OK == response.StatusCode);
        }
    }
}
