using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using MihaZupan;
using System.Text.RegularExpressions;

namespace penitto
{
    class ConfigFiles
    {
        public static bool CheckProxy(string FilePathProxy)
        {
            string line;
            bool EnableProxy;
            StreamReader sr = new StreamReader(FilePathProxy);
            line = sr.ReadLine();
            if (line != null)
            {
                EnableProxy = true;
                ConLog.ConLogWrite("Конфиг прокси не пустой: " + line);
            }
            else
            {
                EnableProxy = false;
                ConLog.ConLogWrite("Конфиг пустой, работаем напрямую " + line);
            }

            return EnableProxy;
        }
        
        public static string ProxyIP(string FilePathProxy)
        {
            string line;
            string ProxyIpLine;
            int Sep1;
            char Dev1 = ':';
            StreamReader sr = new StreamReader(FilePathProxy);
            line = sr.ReadLine();
            Sep1 = line.IndexOf(Dev1);
            ProxyIpLine = line.Substring(0, Sep1);
            ConLog.ConLogWrite("IP Proxy: " + ProxyIpLine);
            sr.Close();
            return ProxyIpLine;
        }
        
        public static int ProxyPort(string FilePathProxy)
        {
            string line;
            string ProxyPortLine;
            int Sep1;
            char Dev1 = ':';
            StreamReader sr = new StreamReader(FilePathProxy);
            line = sr.ReadLine();
            Sep1 = line.IndexOf(Dev1);
            ProxyPortLine = line.Substring(Sep1+1);
            ConLog.ConLogWrite("Port Proxy: " + ProxyPortLine);
            Sep1 = int.Parse(ProxyPortLine);
            sr.Close();
            return Sep1;

        }

    }


    class Bot
    {
        private TelegramBotClient bot;
        public Bot()
        {
            bool chP;
            string PrIp;
            int PrPort;
            chP = ConfigFiles.CheckProxy("c:\\t-bot\\proxy.txt");
            if (chP == false)
            {
                PrIp = "ХУЕТА";
                PrPort = 0;
            }
            else
            {
                PrIp = ConfigFiles.ProxyIP("c:\\t-bot\\proxy.txt");
                PrPort = ConfigFiles.ProxyPort("c:\\t-bot\\proxy.txt");
            }
            var Proxy = new HttpToSocks5Proxy(PrIp, PrPort);
            Proxy.ResolveHostnamesLocally = true;
            if (PrPort == 0)
            {
                Proxy = null;
            }
            //ConLog.ConLogWrite(PrIp);
            bot = new TelegramBotClient("533511163:AAHi87I6Ub6wtrVdtr3k2yakQChYeppUsQc", Proxy);


            //            if (ConfigFiles.CheckProxy("c:\\t-bot\\proxy.txt") == false)
            //            {
            //                bot = new TelegramBotClient("533511163:AAHi87I6Ub6wtrVdtr3k2yakQChYeppUsQc");
            //            }
            //            else
            //            {
            //                var Proxy = new HttpToSocks5Proxy(ConfigFiles.ProxyIP("c:\\t-bot\\proxy.txt"), ConfigFiles.ProxyPort("c:\\t-bot\\proxy.txt"));
            //                Proxy.ResolveHostnamesLocally = true;
            //                bot = new TelegramBotClient("533511163:AAHi87I6Ub6wtrVdtr3k2yakQChYeppUsQc", Proxy);
            //            }
        }
        public async Task TestApiAsync()
        {
            // https://api.telegram.org/bot533511163:AAHi87I6Ub6wtrVdtr3k2yakQChYeppUsQc/getupdates
            int offset = 0;
            while (true)
            {
                var updates = await bot.GetUpdatesAsync(offset); // получаем массив обновлений
                foreach (var update in updates) // Перебираем все обновления
                {
                    if (update.Message != null && update.Message.Text != null)
                    {
                        //ConLog.ConLogWrite("Принят текст: " + update.Message.Text + " " + offset + " " + update.Message.Type);  //Проверка, читает ли вообще сообщения бот
                        //if (update.Message.Text.Contains("/help") || update.Message.Text.Contains("красавчик помоги!")) //в ответ на команду  выводим сообщение
                        //{
                        //await botClient.SendTextMessageAsync(message.Chat.Id, "тест", replyToMessageId: message.MessageId);  //С Цитатой
                        //    await bot.SendTextMessageAsync(update.Message.Chat.Id, HelpPost.BotHelp()); //без цитаты
                        //    ConLog.ConLogWrite("Отправлен ответ в чат:" + update.Message.Chat.Id + " На команду: " + update.Message.Text);
                        //}

                        //if (update.Message.Text.Contains("ИНК") || update.Message.Text.Contains("усач")) //в ответ на команду  выводим сообщение
                        //{
                        //    await bot.SendTextMessageAsync(update.Message.Chat.Id, RNDAnswer.troll("us.txt")); //без цитаты
                        //    ConLog.ConLogWrite("Отправлен ответ в чат:" + update.Message.Chat.Id + " На команду: " + update.Message.Text);
                        //}
                        string MainMSG = BotAnswer.AnswerRead(update.Message.Text);
                        if (update.Message.Text != null && MainMSG != null) //в ответ на команду  выводим сообщение
                        {

                            await bot.SendTextMessageAsync(update.Message.Chat.Id, MainMSG); //без цитаты
                            ConLog.ConLogWrite("Отправлен ответ в чат:" + update.Message.Chat.Id + " На команду: " + update.Message.Text);
                        }

                        //if (update.Message.Text.Contains("/azk")) //в ответ на команду  выводим сообщение
                        //{
                        //    await bot.SendTextMessageAsync(update.Message.Chat.Id, AZKAnswer.AZKinfo(update.Message.Text)); //без цитаты
                        //    ConLog.ConLogWrite("Отправлен ответ в чат:" + update.Message.Chat.Id + " На команду: " + update.Message.Text);
                        //}



                    }
                    offset = update.Id + 1;
                }
            }
        }
    }
    class BotAnswer
    {
        public static string AnswerRead(string MsgText)
        {
            string CfgFile = "c:\\t-bot\\t-bot.cfg";
            string line;
            char Dev1 = ':';
            char Dev2 = '$';
            int Sep1;
            int Sep2;
            ushort LineNum;
            StreamReader sr = new StreamReader(CfgFile);
            while ((line = sr.ReadLine()) != null)
            {
                if (!line.Contains("//"))
                {
                    Sep1 = line.IndexOf(Dev1);
                    Sep2 = line.IndexOf(Dev2);
                    string Ask = line.Substring(0, Sep1);
                    string FileName = line.Substring(Sep1 + 1, line.Length - Sep1 - 3);
                    string ConfOpt = line.Substring(Sep2 + 1, 1);
                    if (MsgText.Contains(Ask) && (ConfOpt == "1"))
                    {
                        ConLog.ConLogWrite("Случайный ответ из файла " + FileName + " на запрос: " + MsgText);
                        line = BotAnswer.Troll(FileName);
                        break;
                    }
                    if (MsgText.Contains(Ask) && (ConfOpt == "0"))
                    {
                        ConLog.ConLogWrite("Единичный ответ из файла " + FileName + " на запрос: " + MsgText);
                        line = BotAnswer.One(FileName);
                        break;
                    }
                    if (MsgText.Contains(Ask) && ConfOpt == "2" && (MsgText.Length < (Ask.Length + 3)))
                    {
                        line = ("Неверный формат запроса. Используй: /" + Ask + "XXX, где XXX = 001, 002, ..., 999");
                        break;
                    }
                    if (MsgText.Contains(Ask) && ConfOpt == "2" && (MsgText.Length >= (Ask.Length + 3)))
                        {
                        if (!Regex.IsMatch(MsgText.Substring(Ask.Length, 3), @"\d{3}"))
                        {
                            line = ("Неверный формат запроса. Используй: /" + Ask + "XXX, где XXX = 001, 002, ..., 999");
                            break;
                        }
                        else
                        {
                            LineNum = ushort.Parse(MsgText.Substring(Ask.Length, 3));
                            ConLog.ConLogWrite("Выборочный ответ из файла " + FileName + " на запрос: " + MsgText + ", номер строки: " + LineNum);
                            line = BotAnswer.TargetLine(LineNum, FileName);
                            break;
                        }
                    }
                    if (MsgText.Contains(Ask) && ConfOpt == "3")
                    {
                        string FindKey;
                        string Patt;
                        Patt = @"\s\w+";
                        
                        Regex CutLine = new Regex(Patt);
                        Match match = CutLine.Match(MsgText);
                        //ConLog.ConLogWrite("ЧТО НАШЕЛ:"+match.Value+":"+match.Length+":");
                        //FindKey = MsgText.Substring(Ask.Length+1, Ask.Length+match.Length-1);
                        //FindKey = match.Value.Substring(1, match.Value.Length-1);
                        if (match.Value == "") 
                        {
                            line = ("Неверный формат запроса. Используй: /" + Ask + "[пробел]искомая_строка");
                            ConLog.ConLogWrite("Неверный формат запроса. Используй: /" + Ask + "[пробел]искомая_строка");
                            break;
                        }
                        else
                        {
                            FindKey = match.Value.Substring(1, match.Value.Length-1);
                            ConLog.ConLogWrite("Поиск строки в файле " + FileName + " по ключу " + FindKey + ", на запрос: " + MsgText);
                            line = BotAnswer.FindByKey(FileName, FindKey);
                            break;
                        }
                    }
                }
            }
                sr.Close();
            return line;
        }


        public static string One(string OneLineFileName)
        {
            string HelpFilele = ("c:\\t-bot\\" + OneLineFileName);
            string line;
            string TrLine;
            StreamReader sr5 = new StreamReader(HelpFilele);
            line = sr5.ReadLine();
            TrLine = line.Replace("&&", "\n");
            sr5.Close();
            return TrLine;
        }
        public static string TargetLine(ushort NumLineReq, string TargetFileName)
        {
            string TargetFileinfo = ("c:\\t-bot\\" + TargetFileName);
            string line;
            ushort s = 0;
            StreamReader sr = new StreamReader(TargetFileinfo);
            while ((line = sr.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                s++;
                if (s == NumLineReq)
                {
                    break;
                }
            }
            if (s < NumLineReq) line = "Запрошенный номер строки превышает общее количество!!!!1";
            sr.Close();
            return line;
        }

        public static string Troll(string RNDFileName)
        {
            string RNDFilePath = "c:\\t-bot\\";
            string line;
            int s = 0;
            RNDFileName = RNDFilePath + RNDFileName;
            int count = System.IO.File.ReadAllLines(RNDFileName).Length;
            Random rnd = new Random();
            int RNDint = rnd.Next(1, count);
            //Console.WriteLine("Случайное число " + RNDint);
            StreamReader sr2 = new StreamReader(RNDFileName);
            while ((line = sr2.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                s++;
                if (s == RNDint)
                {
                    break;
                }
            }
            sr2.Close();
            return line;
        }


        public static string FindByKey(string BdFileName, string FindKey)
        {
            string BDFilePath = "c:\\t-bot\\" + BdFileName;
            int Ind;
            string line;
            StreamReader sr6 = new StreamReader(BDFilePath);
            while ((line = sr6.ReadLine()) != null)
            {
                    if (line.Contains(FindKey + "::"))
                    {
                    Ind = line.IndexOf("::");
                    line = line.Substring(Ind+2);
                    line = line.Replace("&&", "\n");
                    break;
                    }
                            
            }
            if (line == null) 
                line = "Запись в базе не найдена! попробуй еще раз. ;)";
            sr6.Close();
            return line;
        }


    }



    class ConLog
    {
        public static void ConLogWrite(string info)
        {
            Console.WriteLine(info);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ConLog.ConLogWrite("T-Bot by Penitto v 0.06 alpha");
            Bot bot = new Bot();
            for (int i = 0; i < 5000; i++)
            {
                try
                {
                    bot.TestApiAsync().Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " №" + i + " из 5000");
                    Console.WriteLine("Какаято ниибическая ошибка связи, рестарт сервис((");
                }
            }
            //bot.TestApiAsync().Wait();


            Console.WriteLine("СТОП, очень много ошибок");
            //var ReadCon = Console.ReadLine();
            //ConLog.ConLogWrite(ReadCon);
            Console.ReadKey();
        }
    }
}