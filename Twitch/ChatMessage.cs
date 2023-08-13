using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public abstract class ChatMessage
    {
        protected ChatMessage(string sender, string userName, string text, bool isModerator)
        {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            SenderUserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            IsFromModerator = isModerator;
        }

        public virtual string Sender { get; protected set; }

        public virtual string SenderUserName { get; protected set; }

        public virtual string Text { get; protected set; }

        public virtual bool IsFromModerator { get; protected set; }
    }
}
