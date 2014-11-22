﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.Domain.Contracts.Email;
using CH.Domain.Models;

namespace CH.Domain.Handlers.Emails
{
    public class SendUsernameChangedEmail 
    {
        private IEmailClient _emailClient;

        public SendUsernameChangedEmail(IEmailClient emailClient)
            : base()
        {
            _emailClient = emailClient;
        }

        public async Task Execute(string email, string oldUsername, string newUsername, string organizationName)
        {
            EmailMessage message = new EmailMessage()
            {
                Subject = organizationName + " Admin Notification"
            };

            message.To.Add(email);

            EmailBuilder
                .Begin(message)
                .AddParagraph(string.Format("This is a notification that your username has been changed from {0} to {1}.", oldUsername, newUsername))
                .End();

            await _emailClient.SendAsync(message);
        }
    }
}
