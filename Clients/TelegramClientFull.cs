using System.Net;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Contacting_Frontend.Clients;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class TelegramClientFull
{
    private TelegramBotClient bot = new TelegramBotClient("5482091592:AAHZy8qk4IJRO2ljGUhPgWJMFWc5e8BZHgs");
    private apiclient api = new apiclient();
    private string currentcommand = "";
    private long chatId;
    
    
    
    
    string city = "";
    int age = 0;
    string description = "";
    string male = "";
    string name = "";
    string _photo = "";
    
    
    
    
    public TelegramClientFull()
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.EditedMessage
            }
        };
        bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
        
    }

    private async Task ErrorHandler(ITelegramBotClient bot, Exception exc, CancellationToken arg3)
    {
        await bot.SendTextMessageAsync(chatId, "Something went wrong");
    }

    private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
    {
        var id = update.Message.From.Id;
        var username = update.Message.From.Username;
        var chatId = update.Message.Chat.Id;
        
        
        if (update.Message.Text == "/start")
        {
            var resp = api.CreateUser(id, username);
            if (resp.Result.StatusCode == HttpStatusCode.OK)
            {
                await bot.SendTextMessageAsync(chatId, text:"Your account was successfully created! Now type your name, which will be in ankete" , cancellationToken:arg3);
                currentcommand = "name";
            } else
            {
                await bot.SendTextMessageAsync(chatId, text:"Error! Maybe you already have an account." , cancellationToken:arg3);
                currentcommand = "";
            }
        } else
        {
            if (update.Message.Type == MessageType.Text)
            {
                switch (currentcommand)
                {
                    case "name":
                        if (currentcommand == "name")
                        {
                            await bot.SendTextMessageAsync(chatId, text:"Type your city!" , cancellationToken:arg3);
                            name = update.Message.Text;
                            currentcommand = "city";

                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                        }
                        break;
                    case "city":
                        if (currentcommand == "city")
                        {
                            await bot.SendTextMessageAsync(chatId, text:"Type some info about you.:)" , cancellationToken:arg3);
                            city = update.Message.Text;
                            currentcommand = "description";
                            
                            
                            
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                        }
                        break;
                    case "description":
                        if (currentcommand == "description")
                        {
                            await bot.SendTextMessageAsync(chatId, text:"Your age!" , cancellationToken:arg3);
                            description = update.Message.Text;
                            currentcommand = "age";
                            
                            
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                        }
                        break;
                    case "age":
                        if (currentcommand == "age")
                        {
                            await bot.SendTextMessageAsync(chatId, text:"Type your male: (M/F)" , cancellationToken:arg3);
                            age = int.Parse(update.Message.Text);
                            currentcommand = "male";
                            
                            
                            
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                        }
                        break;
                    case "male":
                        if (currentcommand == "male")
                        {
                            await bot.SendTextMessageAsync(chatId, text:"Now send your photo!" , cancellationToken:arg3);
                            male = update.Message.Text;
                            //currentcommand = "photo";
                            
                            
                            
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                            
                            
                            currentcommand = "";
                            api.FillData(id, age:age, city, male, description, name);
                            Contacting_Frontend.ApiModels.User user = api.ShowAnketa(id);
                            if (user != null)
                            {
                                bot.SendTextMessageAsync(update.Message.Chat.Id, text:"Your ankete is ready! ", cancellationToken:arg3);
                                bot.SendTextMessageAsync(update.Message.Chat.Id,
                                    text:
                                    $"Name: {user.Name}, Age: {user.age} \\n. City: {user.city}, Male: {user.Male}, Descrip: {user.description} ",
                                    cancellationToken: arg3);
                            }
                        }
                        break;
                    /*case "photo":
                        if (currentcommand == "photo")
                        {
                            string filePath = @"E:\Bot_Znakomstva\Frontend\Contacting_Frontend\Photos";
                            
                            var photo = bot.GetFileAsync(update.Message.Photo.Last().FileId, cancellationToken:arg3).Result;
                            Console.WriteLine(photo.FilePath);
                            /*Stream photo = new FileStream(filePath + update.Message.From.Id, FileMode.OpenOrCreate);
                            if (update.Message.Type == MessageType.Photo)
                            {
                                bot.DownloadFileAsync(update.Message.Photo.Last().FileId, photo);
                            }#1#
                            
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Current command: " + currentcommand);
                            Console.WriteLine("Name: " + name);
                            Console.WriteLine("Male: " + male);
                            Console.WriteLine("Age: " + age);
                            Console.WriteLine("City: " + city);
                            Console.WriteLine("Descrip: " + description);
                            Console.WriteLine(new string('-', 50));
                            
                            currentcommand = "";
                            api.FillData(id, age:age, city, male, description, name);
                            await bot.SendTextMessageAsync(chatId, text:"Nice! That's your anketa:" , cancellationToken:arg3);
                            api.ShowAnketa(id);
                        }
                        break;*/
                    default:
                        await bot.SendTextMessageAsync(chatId, text:"Unknown command", cancellationToken: arg3);
                        break;
                }
            }
        }
    }
}