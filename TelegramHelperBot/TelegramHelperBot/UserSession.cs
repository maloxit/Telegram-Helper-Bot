using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramHelperBot
{
    class UserSession
    {
        public long chatId;
        public int lastUpdateOffset;
        public Stack<int> questionsIds;
        const int beginOfNode = 1;
        public QuestionData currentQuestion = null;
        public UserSession(long chatId)
        {
            this.chatId = chatId;
            questionsIds = new Stack<int>();
            lastUpdateOffset = -1;
        }

        private ReplytRequest PrepareReplytRequest() 
        {
            string finalText = "";

            foreach (var link in currentQuestion.linkList)
            {
                finalText = string.Concat(finalText, "\n", link);
            }
            finalText = string.Concat(currentQuestion.nodeText, "\n", finalText);

            return new ReplytRequest(chatId, finalText, currentQuestion.optionList);
        }

       

        private void ProcessingCallbackQuery(DataBaseManager dbManager, Update upd)
        {
            Option foundOption = null;
            foreach (var option in currentQuestion.optionList)
            {
                if (upd.CallbackQuery.Data == option.shortName)
                {
                    foundOption = option;
                    break;
                }
            }
            if (foundOption == null)
            {
                throw new Exception("Invalid option");
            }
            questionsIds.Push(currentQuestion.nodeId);
            QuestionData newQuestion = dbManager.GetQuestionData(foundOption.nextNodeId);
            if (newQuestion == null)
            {
                throw new Exception("DataBase error");
            }
            currentQuestion = newQuestion;
        }

        public void ProcessUpdate(DataBaseManager dbManager, long chatId, Update upd, int offset, out ReplytRequest replytRequest, out DeleteKeyboardRequest editRequest)
        {
            lastUpdateOffset = offset;
            editRequest = null;
            replytRequest = null;
            if (currentQuestion == null)
            {
                questionsIds.Clear();
                currentQuestion = dbManager.GetQuestionData(beginOfNode);
                replytRequest = PrepareReplytRequest();
                return;
            }

            switch (upd.Type)
            {
                //upd.CallbackQuery.From.LanguageCode
                case UpdateType.CallbackQuery:
                    editRequest = new DeleteKeyboardRequest(chatId, upd.CallbackQuery.Message.MessageId);
                    ProcessingCallbackQuery(dbManager, upd);
                    replytRequest = PrepareReplytRequest();
                    break;
                case UpdateType.Message:
                    if (upd.Message.Text == "/start")
                    {
                        questionsIds.Clear();
                        currentQuestion = dbManager.GetQuestionData(beginOfNode);
                        replytRequest = PrepareReplytRequest();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
