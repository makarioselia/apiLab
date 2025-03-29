using EF_Core.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace NewWithApi.Helpers.filter
{
    public class GemeralExceptionFilter : ExceptionFilterAttribute
    {
        private string path = Directory.GetCurrentDirectory() +"/logging/" + DateTime.Today.ToString("dd-MM-yy");

        public override void OnException(ExceptionContext context)
        {
            string User = context.HttpContext.User.Claims.Any() ?
                    context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier).Value
                    : "Guest";
            StringBuilder Error = new StringBuilder();
            Error.Append($"On {DateTime.Now}");
            Error.Append($"User {User}");
            Error.Append($"Error {context.Exception.Message}");
            Error.Append($"Details {context.Exception.StackTrace}");

            File.AppendAllText(path, Error.ToString());


            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("6ecff26c8406b7", "83304a61e8f702"),
                EnableSsl = true
            };
            //client.Send("from@example.com", "to@example.com", "Genaral Exception Filter", stringBuilder.ToString());

            client.Send("from@example.com", "to@example.com", "Genaral Exception Filter", Error.ToString());
            System.Console.WriteLine("Sent");

            context.ExceptionHandled = true;
            context.Result = new JsonResult(new APIResault <string>
            {
                Massage = "Sorry Someting wrong happand!!!",
                Success = false,
                Status = 500,
                Data = ""
            });

        }
    }
}
