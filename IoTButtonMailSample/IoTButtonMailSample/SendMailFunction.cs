using Amazon.Lambda.Core;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using IoTButtonMailSample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace IoTButtonMailSample
{
    public class SendMailFunction
    {
        private readonly AmazonSimpleEmailServiceClient _amazonSimpleEmailServiceClient = new AmazonSimpleEmailServiceClient();

        private const string RecipientEnvironmentKey = "Recipient";
        private const string VerificationEmailWarning = "A verification email sent to the provided mail address. You first need to verify it.";

        public async Task FunctionHandler(IoTButtonEvent buttonEvent, ILambdaContext context)
        {
            var email = Environment.GetEnvironmentVariable(RecipientEnvironmentKey);
            var payload = JsonConvert.SerializeObject(buttonEvent);

            var emailStatus = await CheckEmailStatusAsync(email);

            if (!emailStatus.IsVerified)
            {
                Console.WriteLine($"Failed to check email: {email}. {emailStatus.ErrorMessage}");
                return;
            }

            var subject = $"Hello from your IoT Button {buttonEvent.SerialNumber}";
            var body = $"Hello from your IoT Button {buttonEvent.SerialNumber}. Here is the full event: {payload}.";

            var request = new SendEmailRequest
            {
                Source = email,
                Destination = new Destination(new List<string> { email }),
                Message = new Message(new Content(subject), new Body(new Content(body)))

            };

            await _amazonSimpleEmailServiceClient.SendEmailAsync(request);
        }

        private async Task<EmailStatus> SendVerificationEmailAsync(string email)
        {
            var request = new VerifyEmailIdentityRequest { EmailAddress = email };
            await _amazonSimpleEmailServiceClient.VerifyEmailIdentityAsync(request);

            return new EmailStatus(false, VerificationEmailWarning);
        }

        private async Task<EmailStatus> CheckEmailStatusAsync(string email)
        {
            var request = new GetIdentityVerificationAttributesRequest { Identities = new List<string> { email } };
            var response = await _amazonSimpleEmailServiceClient.GetIdentityVerificationAttributesAsync(request);
            var attributes = response.VerificationAttributes;
            if (attributes.TryGetValue(email, out IdentityVerificationAttributes verificationAttributes))
            {
                if (verificationAttributes.VerificationStatus == VerificationStatus.Success)
                    return new EmailStatus(true, string.Empty);
            }

            return await SendVerificationEmailAsync(email);
        }
    }
}
