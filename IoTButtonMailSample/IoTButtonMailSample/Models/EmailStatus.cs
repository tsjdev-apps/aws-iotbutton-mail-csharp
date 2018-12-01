namespace IoTButtonMailSample.Models
{
    public class EmailStatus
    {
        public bool IsVerified { get; }
        public string ErrorMessage { get; }


        public EmailStatus(bool isVerified, string errorMessage)
        {
            IsVerified = isVerified;
            ErrorMessage = errorMessage;
        }
    }
}