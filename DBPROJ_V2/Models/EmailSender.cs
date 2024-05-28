using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DBPROJ_V2.Models;

namespace DBPROJ.Services
{

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(IConfiguration configuration,ApplicationDbContext context, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _context = context;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message) 
        {
            var apiKey = _configuration["Mailgun:ApiKey"];
            var domain = _configuration["Mailgun:Domain"];
            var requestUri = $"https://api.mailgun.net/v3/{domain}/messages";

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));

            var user = _context.UserProfiles.FirstOrDefault(u => u.User.Email == email);
            if(user == null)
            {
                return;
            }
            var token = user.UserId;
            var confirmationLink = $"https://localhost:7027/Account/ConfirmEmail?email={email}&token={token}";

            // Customizing email content
            var emailContent = $@"<html>
                                <head>
                                    <style>
                                        /* Body styles */
                                        body {{
                                            font-family: Arial, sans-serif;
                                            line-height: 1.6;
                                            color: #333;
                                        }}

                                        /* Container styles */
                                        .container {{
                                            max-width: 600px;
                                            margin: 0 auto;
                                            padding: 20px;
                                            border: 1px solid #ccc;
                                            border-radius: 5px;
                                            background-color: #f9f9f9;
                                        }}

                                        /* Header styles */
                                        .header {{
                                            text-align: center;
                                            margin-bottom: 20px;
                                        }}

                                        /* Link styles */
                                        a {{
                                            color: #007bff;
                                            text-decoration: none;
                                        }}

                                        a:hover {{
                                            text-decoration: underline;
                                        }}

                                        /* Footer styles */
                                        .footer {{
                                            text-align: center;
                                            margin-top: 20px;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class=""container"">
                                        <div class=""header"">
                                            <h2>Verify Your Email Address</h2>
                                        </div>
                                        <p>Dear User,</p>
                                        <p>Please verify your email address by clicking the following link:</p>
                                        <p><a href=""{confirmationLink}"" target=""_blank"">Verify Email Address</a></p>
                                        <p>If you did not request this email, you can safely ignore it.</p>
                                        <div class=""footer"">
                                            <p>Best regards,</p>
                                            <p>taskAssist</p>
                                        </div>
                                    </div>
                                </body>
                                </html>
                                ";

            var content = new MultipartFormDataContent
            {
                { new StringContent($"TaskAssist <noreply@" + domain +">"), "from" },
                { new StringContent(email), "to" },
                { new StringContent(subject), "subject" },
                { new StringContent(emailContent), "html" } // Use "html" instead of "text" for HTML content
            };

            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log the error response content
                _logger.LogError($"Mailgun API request failed with status code {response.StatusCode}. Response content: {responseContent}");
                // Throw an exception or handle the error accordingly
                throw new Exception($"Mailgun API request failed with status code {response.StatusCode}");
            }

            response.EnsureSuccessStatusCode();

        }

    }

}
