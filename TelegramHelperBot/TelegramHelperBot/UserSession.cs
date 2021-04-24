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
        public long chatId;
        public int lastUpdateOffset;
        public UserSession(long chatId)
        {
            this.chatId = chatId;
            lastUpdateOffset = -1;
        }


        public void ProcessUpdate(DataBaseManager dbManager, long chatId, Update upd, int offset, out ReplytRequest replytRequest, out MarkupEditRequest editRequest)
        {
            lastUpdateOffset = offset;
            editRequest = null;
            switch (upd.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    editRequest = new MarkupEditRequest(chatId, upd.CallbackQuery.Message.MessageId);
                    if (upd.CallbackQuery.Data == "opt1")
                    {
                        replytRequest = new ReplytRequest(chatId, "Вы кликнули тут", ("opt1", "1 Кликни тут"), ("opt2", "2 Или вот тут"), ("opt3", "3 Или даже здесь"));
                    }
                    else if (upd.CallbackQuery.Data == "opt2")
                    {
                        replytRequest = new ReplytRequest(chatId, "Вы кликнули вот тут", ("opt2", "2 Или вот тут"), ("opt3", "3 Или даже здесь"));
                    }
                    else if (upd.CallbackQuery.Data == "opt3")
                    {
                        replytRequest = new ReplytRequest(chatId, "Вы кликнули даже здесь", ("opt1", "1 Кликни тут"), ("opt2", "2 Или вот тут"));
                    }
                    else
                    {
                        replytRequest = new ReplytRequest(chatId, "Вы кликнули непойми где", ("opt1", "1 Кликни тут"));
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    if (upd.Message.Text == "/start")
                    {
                        replytRequest = new ReplytRequest(chatId, "Для начала", ("opt1", "1 Кликни тут"));
                    }
                    else
                    {
                        replytRequest = new ReplytRequest(chatId, "Зачем вы написали" + upd.Message.Text + "?");
                    }
                    break;
                default:
                    replytRequest = new ReplytRequest(chatId, "Зачем вы это сделали?");
                    break;
            }
        }
    }
}
