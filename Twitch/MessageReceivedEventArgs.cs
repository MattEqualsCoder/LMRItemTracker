﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(ChatMessage message)
        {
            Message = message;
        }

        public ChatMessage Message { get; }
    }
}
