using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Saiive.SuperNode.Export
{
    public class MailHandler
    {
        private readonly ISendGridClient _mailHandler;
        private readonly ILogger<MailHandler> _log;

        private const string ToProperty = "to";

        public MailHandler(ISendGridClient mailHandler, ILogger<MailHandler> log)
        {
            _mailHandler = mailHandler;
            _log = log;
        }

        public async Task Send(string to, params Attachment[] attachments)
        {
            try
            {
                var msg = MailHelper.CreateSingleEmail(
                    new EmailAddress("export@tritonwallet.com", "Triton DeFi Wallet Export"),
                    new EmailAddress(to), "Triton Export", "Your exports are attached to this email!", "");

                var i = 0;
                foreach (var attachment in attachments)
                {
                    msg.AddAttachment(attachment);
                    i++;
                }

                var response = await _mailHandler.SendEmailAsync(msg);
                var body = await response.Body.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(body);
                }

            }
            catch (Exception e)
            {
                _log.LogError(e, $"Error sending mail message...");
                //ignore now
            }
        }

    }
}
