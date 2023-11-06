using System.ComponentModel;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using TelegramBot.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot
{
    public class BotMethods
    {
        string group = "@newGroup";
        string temporaryFilePath = "C:\\Users\\tefgo\\source\\repos\\TelegramBot\\TelegramBot\\images\\";
        public async Task DeleteMessageTG(ITelegramBotClient botClient, Message message)
        {
            try
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine($"Nie można usunąć wiadomości: {ex.Message}");
            }
        }
        public async Task<long> GetUserIdAsync(ITelegramBotClient botClient, string username)
        {
            try
            {
                var chat = await botClient.GetChatAsync(username);
                if (chat != null)
                {
                    return chat.Id;
                }
                else
                {
                    return -1; 
                }
            }
            catch (Exception ex)
            {
                return -1; 
            }
        }
        public async Task SetWarn(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                if (message.Text.Contains("@"))
                {
                    int position = message.Text.IndexOf("@");
                    string nick = message.Text.Substring(position);
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        var user = context.Users.FirstOrDefault(u => u.Name == nick);
                        if (user != null)
                        {
                            user.Warn += 1;
                            if (user.Warn > 2)
                            {
                                long userId = await GetUserIdAsync(botClient, user.Name);
                                await botClient.RestrictChatMemberAsync(
                                    group,
                                    userId,
                                    new ChatPermissions
                                    {
                                        CanSendMessages = false,
                                        CanSendOtherMessages = false,
                                        CanSendPolls = false,
                                        CanSendPhotos = false,
                                        CanManageTopics = false,
                                        CanChangeInfo = false
                                    });
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            await DeleteMessageTG(botClient, message);
        }

        public async Task DeleteOneWarn(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                if (message.Text.Contains("@"))
                {
                    int position = message.Text.IndexOf("@");
                    string nick = message.Text.Substring(position);
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        var user = context.Users.FirstOrDefault(u => u.Name == nick);
                        if (user != null)
                        {
                            if (user.Warn >= 0)
                            {
                                user.Warn -= 1;
                                if (user.Warn < 3)
                                {
                                    long userId = await GetUserIdAsync(botClient, user.Name);
                                    await botClient.RestrictChatMemberAsync(
                                        group,
                                        userId,
                                        new ChatPermissions
                                        {
                                            CanSendMessages = true,
                                            CanSendOtherMessages = true,
                                            CanSendPolls = true,
                                            CanSendPhotos = true,
                                        });
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            await DeleteMessageTG(botClient, message);
        }
        public async Task GetUsersWithMedals(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Medals:");
                    var us = context.Users.Include(u => u.UserMedals).ThenInclude(um => um.Medal).AsNoTracking();
                    foreach (var user in us)
                    {
                        stringBuilder.AppendLine(user.Name);
                        string bronze = user.UserMedals.Where(x => x.MedalId == 1).Count().ToString();
                        if (bronze != "0")
                            stringBuilder.AppendLine("bronze: " + bronze);
                        string silver = user.UserMedals.Where(x => x.MedalId == 2).Count().ToString();
                        if (silver != "0")
                            stringBuilder.AppendLine("silver: " + silver);
                        string gold = user.UserMedals.Where(x => x.MedalId == 3).Count().ToString();
                        if (gold != "0")
                            stringBuilder.AppendLine("gold:   " + gold);
                        string penalty = user.UserMedals.Where(x => x.MedalId == 4).Count().ToString();
                        if (penalty != "0")
                            stringBuilder.AppendLine("karny:  " + penalty);
                    }

                    string text = stringBuilder.ToString();
                    sentMessage = await botClient.SendTextMessageAsync(
                    chatId: group,
                    text: text,
                    cancellationToken: cancellationToken);
                }
            }
            catch (Exception)
            {
                throw;
            }
            await DeleteMessageTG(botClient, message);
        }
        public async Task Advertisement(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                string filePath = temporaryFilePath + "reklama.jpg";
                byte[] photoBytes = System.IO.File.ReadAllBytes(filePath);

                string text = "<b>ADD</b>\nBelow are the available groups: \n1. Description of group1:" +
                    "\n@Group1" +
                    "\n2. Description of group2:" +
                    "\n@Group2" +
                    "\n3. Description of group3:" +
                    "\n@Group3" +
                    "\n4. Description of group4:" +
                    "\n@Group4" +
                    "\n5. Description of group5:" +
                    "\n@Group5" +
                    "\n6. Description of group6:" +
                    "\n@Group6";

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Send Photo to group
                    await botClient.SendPhotoAsync(
                        chatId: group,
                        photo: InputFileId.FromStream(fileStream),
                        parseMode: ParseMode.Html,
                        caption: text);
                }


            }
            catch (Exception)
            {

                throw;
            }
            await DeleteMessageTG(botClient, message);
        }

        public async Task RandomText(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                List<string> listaStringow = new List<string>();
                listaStringow.Add("Hey");
                listaStringow.Add("Hello");
                listaStringow.Add("Hi there");
                Random rand = new Random();

                int wylosowanaLiczba = rand.Next(0, listaStringow.Count - 1);

                sentMessage = await botClient.SendTextMessageAsync(
                chatId: group,
                text: listaStringow[rand.Next(3)],
                cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

                throw;
            }
            await DeleteMessageTG(botClient, message);
        }

        public async Task CatMethod(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                await DeleteMessageTG(botClient, message);
                string filePath = temporaryFilePath + "cat.jpg";
                byte[] photoBytes = System.IO.File.ReadAllBytes(filePath);

                //bot.send_photo(chat_id, photo = open('path', 'rb'))
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await botClient.SendPhotoAsync(
                        chatId: group,
                        photo: InputFileId.FromStream(fileStream),       //new InputOnlineFile(fileStream, "nazwa_pliku.jpg"),
                        caption: "This is a cat");
                }
                Thread.Sleep(1000);
                sentMessage = await botClient.SendTextMessageAsync(
               //chatId: chatId,
               chatId: group,
               text: "I am a powerful bot",
               cancellationToken: cancellationToken);

                Thread.Sleep(3000);
                sentMessage = await botClient.SendTextMessageAsync(
               chatId: group,
               text: "I reply with a delay of 3000ms",
               cancellationToken: cancellationToken);

                Thread.Sleep(3000);
                sentMessage = await botClient.SendTextMessageAsync(
               chatId: group,
               text: "This is another message sent late",
               cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

                throw;
            }

            await DeleteMessageTG(botClient, message);
        }

        
        

        public async Task HelloMethod(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, string messageText, Message message)
        {
            try
            {
                await DeleteMessageTG(botClient, message);
                Random rand = new Random();
                string nick = "";

                for (int i = 0; i < messageText.Length - 1; i++)
                {
                    if (messageText[i] == '@')
                    {
                        nick = messageText.Substring(i);
                        break;
                    }
                }

                List<string> listaStringow = new List<string>();
                listaStringow.Add($"{nick} Hey");
                listaStringow.Add($"{nick} Hello");
                listaStringow.Add($"{nick} Hi");

                sentMessage = await botClient.SendTextMessageAsync(
                chatId: group,
                text: listaStringow[rand.Next(3)],
                cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

                throw;
            }
            await DeleteMessageTG(botClient, message);
        }

        public async Task RegulaminMethod(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, Message message)
        {
            try
            {
                string filePath = temporaryFilePath + "advertisement.jpg";
                byte[] photoBytes = System.IO.File.ReadAllBytes(filePath);

                string text = "<b>REGULATIONS</b> \n1. No spam \n2. Prohibition of sales offers\n3. No scam";

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {

                    await botClient.SendPhotoAsync(
                        chatId: group,
                        photo: InputFileId.FromStream(fileStream),    
                        parseMode: ParseMode.Html,
                        caption: text);
                }
            }
            catch (Exception)
            {

                throw;
            }

            await DeleteMessageTG(botClient, message);
        }

        public async Task MedalMethod(ITelegramBotClient botClient, Message sentMessage, CancellationToken cancellationToken, string messageText, Message message)
        {
            try
            {
                if (messageText.Contains('@') && messageText.Contains('$'))
                {
                    byte[] photoBytes;
                    int charNickPosition = messageText.IndexOf("@");
                    string nick = messageText.Substring(charNickPosition);
                    int charMedalPosition = messageText.IndexOf("$");
                    int medal = 0;
                    int.TryParse(messageText[charMedalPosition + 1].ToString(), out medal);

                    if (((medal > 0) && (medal < 5)) && (charNickPosition != 0))
                    {
                        using (ApplicationDbContext context = new ApplicationDbContext())
                        {
                            context.Database.EnsureCreated();
                            int userId = 0;
                            var user = context.Users.FirstOrDefault(u => u.Name == nick);
                            if (user == null)
                            {
                                TelegramBot.Models.User newUser = new TelegramBot.Models.User() { Name = nick };
                                context.Users.Add(newUser);
                                context.SaveChanges();
                                userId = context.Users.First(i => i.Name == nick).Id;
                            }
                            else
                            {
                                userId = user.Id;
                            }

                            string rodzajMedalu = string.Empty;
                            UserMedal newUserMedal = new UserMedal() { MedalId = medal, UserId = userId };
                            context.UserMedals.Add(newUserMedal);
                            context.SaveChanges();


                            string filePath = context.Medals.First(m => m.Id == medal).ImagePath;
                            photoBytes = System.IO.File.ReadAllBytes(filePath);


                            context.SaveChanges();

                            string text = "I award a  " + rodzajMedalu + " to the user " + nick;

                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                await botClient.SendPhotoAsync(
                                    chatId: group,
                                    photo: InputFileId.FromStream(fileStream),       
                                    parseMode: ParseMode.Html,
                                    caption: text);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            await DeleteMessageTG(botClient, message);

        }

    }

}

