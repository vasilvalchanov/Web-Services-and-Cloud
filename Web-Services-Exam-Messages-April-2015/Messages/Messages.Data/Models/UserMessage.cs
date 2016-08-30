namespace Messages.Data.Models
{
    using System;

    public class UserMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string RecipientId { get; set; }

        public virtual User RecipientUser { get; set; }

        public string OwnerId { get; set; }

        public virtual User OwnerUser { get; set; }
    }
}
