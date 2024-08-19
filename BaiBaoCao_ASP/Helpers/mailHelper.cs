using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

public class MailHelper
{
    public bool SendMail(string toEmailAddress, string subject, string content)
    {
        // Replace these with your actual email credentials and SMTP server details
        string fromEmailAddress = "duynene2004@gmail.com";
        string emailUsername = "duynene2004@gmail.com";
        string emailPassword = "iyku kcgh uswj zkkr"; // Use App Password if 2-Step Verification is enabled
        try
        {
            // Create the mail message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmailAddress),
                Subject = subject,
                Body = content,
                IsBodyHtml = false // Set to true if you're sending HTML content
            };

            // Add recipient
            mailMessage.To.Add(toEmailAddress);

            // Create an SmtpClient to send the email
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailUsername, emailPassword),
                EnableSsl = true // Enable SSL for secure connection
            };

            // Send the email
            smtpClient.Send(mailMessage);

            // Log success
            System.Diagnostics.Debug.WriteLine("Email sent successfully to " + toEmailAddress);
            return true;
        }
        catch (SmtpException smtpEx)
        {
            // Log detailed SMTP error
            System.Diagnostics.Debug.WriteLine($"SMTP Exception: {smtpEx.StatusCode} - {smtpEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log general exceptions
            System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
            return false;
        }
    }
}