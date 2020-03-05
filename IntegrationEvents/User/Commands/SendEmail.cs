using System.Collections.Generic;

namespace IntegrationEvents.User.Commands
{
	// TODO: delete the mf
    public class SendEmail
    {
        public List<EmailRecipient> Recipients { get; set; }
        public EmailSender Sender { get; set; }

        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public string TemplateName { get; set; }

        // Data for substitution into html or template
        public object Data { get; set; }


        public SendEmail()
        {
            
        }

        public SendEmail(List<EmailRecipient> recipients, string subject, string html, string text)
        {
            Recipients = recipients;
            Subject = subject;
            Html = html;
            Text = text;
        }

        public SendEmail(List<EmailRecipient> recipients, string subject, string templateName, object data)
        {
            Recipients = recipients;
            Subject = subject;
            TemplateName = templateName;
            Data = data;
        }
    }

    public class EmailRecipient 
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }


    public class EmailSender
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
