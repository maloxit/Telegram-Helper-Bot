using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramHelperBot
{
    class UserSessionsManager
    {
        DataBaseManager dbManager;
        Dictionary<ChatId, UserSession> activeSessions;

        public UserSessionsManager(DataBaseManager dbManager)
        {
            this.dbManager = dbManager;
            activeSessions = new Dictionary<ChatId, UserSession>(50);
        }

        //Доступ к конкретным сессиям по id чата из вне
        public UserSession this[ChatId chatId]
        {
            get
            {
                return activeSessions[chatId];
            }
        }

        //Возвращает массив запросов на редактирование предыдущих сообщений (в часности отключение inline кнопок), просматривая activeSessions
        public MarkupEditRequest[] nextMarkupEditRequests { get; }

        //Возвращает массив запросов на отправку сообщений, просматривая activeSessions
        public ReplytRequest[] nextReplytRequests { get; }

        //Обрабатывает ответы в чатах из activeSessions, если первое сообщение, то создает новую сессию
        //Если от одного пользователя было получено более одного сообщения, то обрабатывается только первое
        public void ProcessNextUpdates(Update[] nextUpdates)
        {
            
            throw new NotImplementedException();
        }

        //Удаляет из activeSessions сессии, которые не были активны долгое время
        internal void RemoveOld()
        {
            throw new NotImplementedException();
        }
    }

    //Хранит данные, которые нужно передать боту для отправки ответа
    public class ReplytRequest
    {
        public readonly ChatId chatId;
        public readonly string text;
        public readonly ParseMode replyMarkup;
    }

    //Хранит данные, которые нужно передать боту для изменения (отключения) inline кнопок в отправленном ранее сообщении
    public class MarkupEditRequest
    {
        public readonly ChatId chatId;
        public readonly int inlineMessageId;
        public readonly InlineKeyboardMarkup replyMarkup;
    }
}
