using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramHelperBot
{
    class UserSession
    {
        public Task<Message> lastSendMessageTask { get; internal set; }
    }
}
