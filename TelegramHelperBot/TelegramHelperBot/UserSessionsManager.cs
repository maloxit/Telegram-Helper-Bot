﻿using System;
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
        Dictionary<long, UserSession> activeSessions;

        public UserSessionsManager(DataBaseManager dbManager)
        {
            this.dbManager = dbManager;
            activeSessions = new Dictionary<long, UserSession>(50);
            nextMarkupEditRequests = new List<MarkupEditRequest>();
            nextReplytRequests = new List<ReplytRequest>();
        }

        //Массив запросов на редактирование предыдущих сообщений (в часности отключение inline кнопок)
        public List<MarkupEditRequest> nextMarkupEditRequests;

        //Массив запросов на отправку сообщений
        public List<ReplytRequest> nextReplytRequests;

        static long GetChatIdOfUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.Chat.Id;
                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.Chat.Id;
                default:
                    throw new Exception("Not supported update type.");
            }
        }

        //Обрабатывает ответы в чатах из activeSessions, если первое сообщение, то создает новую сессию
        //Если от одного пользователя было получено более одного сообщения, то обрабатывается только первое
        //Для этого в каждой сессии хранится оффсет - id первого апдейта последней обработанной им группы
        public void ProcessNextUpdates(Update[] nextUpdates, int offset)
        {
            nextReplytRequests.Clear();
            nextMarkupEditRequests.Clear();
            foreach (var upd in nextUpdates)
            {
                long chatId;
                try
                {
                    chatId = GetChatIdOfUpdate(upd);
                }
                catch (Exception)
                {
                    continue;
                }

                UserSession session;
                if (activeSessions.ContainsKey(chatId))
                {
                    session = activeSessions[chatId];
                }
                else
                {
                    session = new UserSession(chatId);
                    activeSessions.Add(chatId, session);
                }

                if (session.lastUpdateOffset != offset)
                {
                    session.ProcessUpdate(dbManager, chatId, upd, offset, out ReplytRequest replytRequest, out MarkupEditRequest editRequest);
                    if (replytRequest != null) nextReplytRequests.Add(replytRequest);
                    if (editRequest != null) nextMarkupEditRequests.Add(editRequest);
                }
                else
                {
                    continue;
                }
            }
        }

        //Удаляет из activeSessions сессии, которые не были активны долгое время
        public void RemoveOld()
        {
            //throw new NotImplementedException();
        }
    }

    //Хранит данные, которые нужно передать боту для отправки ответа
    public class ReplytRequest
    {
        public readonly long chatId;
        public readonly string text;
        public readonly InlineKeyboardMarkup replyMarkup;

        public ReplytRequest(long chatId, string text, params (string, string) [] options)
        {
            this.chatId = chatId;
            this.text = text;
            List<InlineKeyboardButton> keyboard = new List<InlineKeyboardButton>(options.Length);
            foreach (var opt in options)
            {
                var button = new InlineKeyboardButton
                {
                    CallbackData = opt.Item1,
                    Text = opt.Item2
                };
                keyboard.Add(button);
            }
            replyMarkup = new InlineKeyboardMarkup(keyboard);
        }
    }

    //Хранит данные, которые нужно передать боту для изменения (отключения) inline кнопок в отправленном ранее сообщении
    public class MarkupEditRequest
    {
        public readonly long chatId;
        public readonly int inlineMessageId;
        public readonly InlineKeyboardMarkup replyMarkup;

        public MarkupEditRequest(long chatId, int inlineMessageId, params (string, string)[] options)
        {
            this.chatId = chatId;
            this.inlineMessageId = inlineMessageId;
            List<InlineKeyboardButton> keyboard = new List<InlineKeyboardButton>(options.Length);
            foreach (var opt in options)
            {
                var button = new InlineKeyboardButton
                {
                    CallbackData = opt.Item1,
                    Text = opt.Item2
                };
                keyboard.Add(button);
            }
            replyMarkup = new InlineKeyboardMarkup(keyboard);
        }
    }
}
