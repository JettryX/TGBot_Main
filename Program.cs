using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TGBot_Main
{
    class Program
    {
        static String AUTHOR_MSG = "Alex Odinokov (Frei), alexfrei21@gmail.com, https://github.com/JettryX";

        static Dictionary<string, BasicCommand> botCommands = new Dictionary<string, BasicCommand>();
        static Dictionary<long, string> lastCommandsPerUser = new Dictionary<long, string>();
        static INN_Client innClient;
        static TelegramBotClient telegramBot;
        static Configuration _configuration = new Configuration("app.Settings.json"); // подгрузка конфигурации

        static void Main(string[] args)
        {
            // Инициализация основных функций бота
            try
            {
                InitCommands();
                innClient = new INN_Client(_configuration.getConfigValue("INN_Token"), "https://api.ofdata.ru/v2/");
                innClient.InitClient();

                using var cts = new CancellationTokenSource();
                string TG_token = _configuration.getConfigValue("TG_Token");
                TelegramBotClientOptions tgClientOptions = new TelegramBotClientOptions(TG_token);
                telegramBot = new TelegramBotClient(tgClientOptions, cancellationToken: cts.Token);
                telegramBot.OnMessage += OnMessageHandler;

                Console.WriteLine("Bot is running");
                Console.ReadLine();
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        // обработка текстовых команд
        static async Task OnMessageHandler(Message msg, UpdateType type)
        {
            if (msg.Text == null) return;

            Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");

            if (msg.Text.StartsWith('/'))
            {
                switch (type)
                {
                    case UpdateType.Message:
                        String response = getTextResponse(msg);
                        await telegramBot.SendTextMessageAsync(msg.Chat, response);
                        lastCommandsPerUser[msg.Chat.Id] = response;
                        break;
                    // можно захендлить другие типы дальше    
                    default:
                        break;
                }
            }
        }

        // Инициализация команд и биндинг их к обработчикам
        static void InitCommands()
        {
            BasicCommand startCommand = new BotCommand<Message>("/start", "Начало работы с ботом", (Message msg) => { return _configuration.getConfigValue("GREET_MSG");});
            botCommands.Add(startCommand.command, startCommand);

            BasicCommand helpCommand = new BotCommand<Message>("/help", "Показывает все доступные команды", HandleHelp);
            botCommands.Add(helpCommand.command, helpCommand);

            BasicCommand helloCommand = new BotCommand<Message>("/hello", "Показывает автора, его email и ссылку на github", (Message msg) => { return AUTHOR_MSG;});
            botCommands.Add(helloCommand.command, helloCommand);

            BasicCommand innCommand = new BotCommand<Message>("/inn", "Поиск информации о компании по ИНН на сайте ofdata.ru", HandleInnSearch);
            botCommands.Add(innCommand.command, innCommand);

            BasicCommand lastCommand = new BotCommand<Message>("/last", "Повторяет последнее выполненое действие", HandleLast);
            botCommands.Add(lastCommand.command, lastCommand);
        }

        static String getTextResponse(Message msg)
        {
            String response = "";
            try
            {
                String[] strParams = msg.Text.Split(" ");
                if (botCommands.ContainsKey(strParams[0]))
                {
                    BasicCommand x = botCommands[strParams[0]];
                    response = ((BotCommand<Message>)x).Handle(msg);
                }
             
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return response;
        }

        static String HandleHelp(Message msg)
        {
            String strHelp = "";
            foreach (String s in botCommands.Keys)
            {
                strHelp += botCommands[s].command + " - " + botCommands[s].description + '\n';
            }
            return strHelp;
        }

        static String HandleLast(Message msg)
        {
            return lastCommandsPerUser.ContainsKey(msg.Chat.Id) ? lastCommandsPerUser[msg.Chat.Id] : "Предыдущие команды не найдены";
        }

        static String HandleInnSearch(Message msg)
        {
            String[] msgParams = msg.Text.Split(" ");
            if (msgParams.Length > 1)
            {
                String res = innClient.GetResponse("inn", msgParams[1]).GetAwaiter().GetResult();
                return res;
            }
            return "Пожалуйста укажите искомый ИНН /inn <искомый инн>";
        }

    }

}
