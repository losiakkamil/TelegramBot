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
using TelegramBot.Models;
using TelegramBot;

//using Telegram.Bot.Types.InputFiles;


//WebProxy proxy = new("socks5://142.93.68.63:2434")
//{
//    Credentials = new NetworkCredential("vpn", "unlimited")
//};
//HttpClient httpClient = new(
//    new SocketsHttpHandler { Proxy = proxy, UseProxy = true, }
//);

//var botClient = new TelegramBotClient("YOUR_API_TOKEN", httpClient);

var botClient = new TelegramBotClient("6760821855:AAELwM8Z7Y-56QsomxnfH65TMe90-YkWFNw");//, httpClient);


using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();


BotMethods methods = null;
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;
    methods = new BotMethods();
    //var chatId = message.Chat.Id;

    //Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    messageText = DeleteBotName(messageText);

    Message sentMessage = null;

    if (messageText.ToLower() == "/randomtext")
    {
        await methods.RandomText(botClient, sentMessage, cancellationToken, message);
    }
    else if (messageText.ToLower() == "/cat")
    {
        await methods.CatMethod(botClient, sentMessage, cancellationToken, message);
    }
    else if (messageText.ToLower().Contains("/hello"))  
    {
        await methods.HelloMethod(botClient, sentMessage, cancellationToken, messageText, message);
    }
    else if (messageText.ToLower().Contains("/medal"))
    {
        await methods.MedalMethod(botClient, sentMessage, cancellationToken, messageText, message);
    }
    else if (messageText.ToLower().Contains("/statute"))
    {
        await methods.RegulaminMethod(botClient, sentMessage, cancellationToken, message);
    }
    else if (messageText.ToLower().Contains("/getuserswithmedals"))
    {
        await methods.GetUsersWithMedals(botClient, sentMessage, cancellationToken, message);
    }
    else if (messageText.ToLower().Contains("/advertisement"))
    {
        await methods.Advertisement(botClient, sentMessage, cancellationToken, message);
    }

    long messageId = message.MessageId;

}

string DeleteBotName(string text)
{
    return text.Replace("@FuhrerMonaruBot", "");
}


Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}