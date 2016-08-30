namespace Messages.Data.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        private ICollection<UserMessage> receivedMessages;
        private ICollection<ChannelMessage> channelMessages;

        public User()
        {
            this.receivedMessages = new HashSet<UserMessage>();
            this.channelMessages = new HashSet<ChannelMessage>();
        }

        public virtual ICollection<UserMessage> UserMessages
        {
            get
            {
                return this.receivedMessages;
            }

            set
            {
                this.receivedMessages = value;
            }
        }

        public virtual ICollection<ChannelMessage> ChannelMessages
        {
            get
            {
                return this.channelMessages;
            }

            set
            {
                this.channelMessages = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
