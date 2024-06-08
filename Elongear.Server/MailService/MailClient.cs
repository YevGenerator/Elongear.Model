using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Elongear.Server.Encryption;

namespace Elongear.Server.MailService;

public class MailClient
{
    public string HostEmail { get; set; } = "percev.yevgen@gmail.com";
    public string Password { get; set; } = "hackerdown123";
    public string Server { get; set; } = "smtp-mail.outlook.com";
    public string HtmlPath { get; set; } = "C:\\Users\\yesman\\Documents\\Visual Studio 2022\\Projects\\Elongear.Model\\Elongear.Server\\MailService\\template.html";
    public string CssPath { get; set; } = "C:\\Users\\yesman\\Documents\\Visual Studio 2022\\Projects\\Elongear.Model\\Elongear.Server\\MailService\\style.css";
    public string RootPath { get; set; } = "";
    public string ConfirmPart { get; set; } = "confirm_reg/";
    public string Url(string token) => RootPath + ConfirmPart + token;
    public async Task SendActivationLetter(byte[] digits, string token, string email)
    {
        SmtpClient client = new()
        {
            Host = Server,
            Port = 587,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(HostEmail, Password),
            EnableSsl = true
        };
        var html = await GetHtml(digits, token);
        var message = GetMessage(html, email);
        await client.SendMailAsync(message);
    }

    public async Task<string> GetHtml(byte[] digits, string token)
    {
        var parser = new HtmlParser();
        await parser.ReadHtml(HtmlPath);
        parser.SetDigits(digits);
        parser.SetConfirmationLink(Url(token));
        await parser.SetStylesToHtml(CssPath);
        return parser.Html;
    }
    public MailMessage GetMessage(string text, string email)
    {
        var message = new MailMessage(HostEmail, email)
        {
            Subject = "Підтвердження реєстрації на Elongear",
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Body = text
        };
        return message;
    }
}
