namespace Messages.Data.Models
{
    using System;
    using System.Security.AccessControl;

    public class ChannelMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Channel Channel { get; set; }
    }
}
