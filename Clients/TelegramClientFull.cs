using System.Net;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace Contacting_Frontend.Clients;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class TelegramClientFull
{
    private static string _token = "5482091592:AAHZy8qk4IJRO2ljGUhPgWJMFWc5e8BZHgs";
    private TelegramBotClient bot = new TelegramBotClient(token:_token);
    private apiclient api = new apiclient();
    private string currentcommand = "";
    private long chatId;
    
    string city = "";
    int age = 0;
    string description = "";
    string male = "";
    string name = "";
    string photo_name = "";
    string wantToSearch = "";


    private long currentUserAnketeSearchId;
    
    public TelegramClientFull()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        bot.StartReceiving(
            UpdateHandler,
            ErrorHandler,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
    }

    private async Task ErrorHandler(ITelegramBotClient bot, Exception exc, CancellationToken arg3)
    {
        await bot.SendTextMessageAsync(chatId, "Something went wrong", cancellationToken: arg3);
    }

    private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        if (update.Type == UpdateType.Message)
        {
            var id = update.Message.From.Id;
            var username = update.Message.From.Username;
            var chatId = update.Message.Chat.Id;
            
            if (update.Message.Text == "/start" || update.Message.Text == "/changemyankete")
            {
                var resp = api.CreateUser(id, username);
                if (resp.Result.StatusCode == HttpStatusCode.OK)
                {
                    ReplyKeyboardMarkup startOrChangeAnkete = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("/deleteaccount"),
                        }
                    });
                    await bot.SendTextMessageAsync(chatId, text:"Your account was successfully created! Now type your name, which will be in ankete" , cancellationToken:arg3, replyMarkup: startOrChangeAnkete);
                    currentcommand = "name";
                } else
                {
                    await bot.SendTextMessageAsync(chatId, text:"Error! Maybe you already have an account." , cancellationToken:arg3, replyMarkup: null);
                    currentcommand = "";
                }
            } else if (update.Message.Text == "/deleteaccount")
            {
                try
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(
                        new[] {
                            new[] {
                                new KeyboardButton("/start"),
                            }
                        });
                    api.DeleteMyAccount(id);
                    bot.SendTextMessageAsync(chatId, text:"Your account was successfully deleted." , cancellationToken:arg3, replyMarkup: replyKeyboardMarkup);
                    currentcommand = "";
                }
                catch
                {
                    bot.SendTextMessageAsync(chatId, text:"Error! Maybe you don't have an account." , cancellationToken:arg3);
                }
            } else if (update.Message.Text == "/myankete")
            {
                try
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(
                        new[]
                        {
                            new[]
                            {
                                new KeyboardButton("/lookforanketes"),
                                new KeyboardButton("/changemyankete")
                            }
                        });
                    Contacting_Frontend.ApiModels.User user = api.ShowAnketa(id);
                    await using var stream = new FileStream(user?.photo_path, FileMode.Open, FileAccess.Read);
                    await bot.SendPhotoAsync(chatId, new InputOnlineFile(stream, user?.photo_path),
                        caption: $"{user?.Name}, {user?.city}, {user?.age} - {user?.description}",
                        cancellationToken: arg3, replyMarkup: replyKeyboardMarkup);
                    Console.WriteLine(user.IsAllFieldsFillled());
                    currentcommand = "";
                }
                catch
                {
                    await bot.SendTextMessageAsync(chatId, text:"You don't have an account. Please, choose /start to create one." , cancellationToken:arg3, replyMarkup: new ReplyKeyboardMarkup(
                        new[]
                        {
                            new[]
                            {
                                new KeyboardButton("/start"),
                            }
                        }));
                }
            } else if (update.Message.Text == "/lookforanketes")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new[]
                    {
                        new[]
                        {
                            new KeyboardButton("👍"),
                            new KeyboardButton("👎")
                        }
                    });
                currentcommand = "";
                var anketes = api.GetMales().Result;
                foreach (var user in anketes)
                {
                    await using var stream = new FileStream(user?.photo_path, FileMode.Open, FileAccess.Read);
                    await bot.SendPhotoAsync(chatId, new InputOnlineFile(stream, user?.photo_path),
                        caption: $"{user?.Name}, {user?.city}, {user?.age} - {user?.description}",
                        cancellationToken: arg3, replyMarkup: replyKeyboardMarkup);

                    currentUserAnketeSearchId = user.TGID;
                }
            } else if (update.Message.Text == "👍")
            {
                SendLike(id, currentUserAnketeSearchId, bot);
            }
            else
            {
                if (update.Message.Type == MessageType.Text)
                {
                    switch (currentcommand)
                    {
                        case "name":
                            name = update.Message.Text;
                            currentcommand = "age";
                            await bot.SendTextMessageAsync(chatId, text:"Type your age!" , cancellationToken:arg3);
                            break;
                        case "city":
                            city = update.Message.Text;
                            currentcommand = "description";
                            await bot.SendTextMessageAsync(chatId, text:"Type some info about you.:)" , cancellationToken:arg3);
                            break;
                        case "description":
                            description = update.Message.Text;
                            currentcommand = "photo";
                            await bot.SendTextMessageAsync(chatId, text:"Send your photo" , cancellationToken:arg3);
                            break;
                        case "age":
                            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(
                                new[] {
                                    new[] {
                                        new KeyboardButton("M"),
                                        new KeyboardButton("F")
                                    }
                                });
                            try
                            {
                                age = int.Parse(update.Message.Text);
                                currentcommand = "male";
                                await bot.SendTextMessageAsync(chatId, text:"Choose your male:" , replyMarkup: replyKeyboardMarkup, cancellationToken:arg3);
                            }
                            catch
                            {
                                await bot.SendTextMessageAsync(chatId, text: "Incorrect value. Try one more time.", cancellationToken: arg3);
                            }
                            break;
                        case "male":
                            male = update.Message.Text;
                            currentcommand = "city";
                            await bot.SendTextMessageAsync(chatId, text:"Type your city:" , cancellationToken:arg3, replyMarkup:null);
                            break;
                        default:
                            await bot.SendTextMessageAsync(chatId, text:"Unknown command", cancellationToken: arg3);
                            break;
                    }
                } else if (update.Message.Type == MessageType.Photo)
                {
                    if (currentcommand == "photo")
                    {
                        Console.WriteLine(update.Message.Photo.Last().FileId);
                        
                        string filePath_folder = @"E:/Bot_Znakomstva/Frontend/Contacting_Frontend/photos/";
                          
                        string file_path_from_telegram = bot.GetFileAsync(update.Message.Photo.Last().FileId, cancellationToken:arg3).Result.FilePath;
                        
                        string fullTelegramPath = $@"https://api.telegram.org/file/bot{_token}/{file_path_from_telegram}";
                        Console.WriteLine(fullTelegramPath);
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileAsync(new Uri(fullTelegramPath), filePath_folder+$"{id}.jpg");

                        currentcommand = "";
                        api.FillData(id, age:age, city, male, description, name, photopath: filePath_folder +$"{id}.jpg");
                        await bot.SendTextMessageAsync(chatId, text:"Nice! Type '/myankete' to see your ankete." , cancellationToken:arg3);

                        city = "";
                        age = 0;
                        name = "";
                        description = "";
                        male = "";
                        photo_name = "";
                    }
                }
            }
        }
    }


    private void SendLike(long from, long to, ITelegramBotClient bot)
    {
        //api.Like(from, to);
    }

    private async void GetLike(ITelegramBotClient bot)
    {
        
    }
}