namespace APIFrame.Utils.Email.DataObjects
{
    public abstract class MailOptions
    {
        public string SenderName { get; set; }

        public string SenderEmail { get; set; }

        public string SenderPassword { get; set; }

        public string Subject { get; set; }

        public string BodyWithHtml { get; set; }
    }
}
