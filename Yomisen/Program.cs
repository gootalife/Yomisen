using Discord;
using Discord.WebSocket;
using FNF.Utility;
using System;
using System.Threading.Tasks;

namespace Yomisen
{
    class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// 非同期Mainメソッド
        /// </summary>
        static async Task MainAsync()
        {
            // トークンのチェック
            await CheckTokenAsync();
            // ログイン処理
            Console.WriteLine("ログイン中…");
            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.User, Properties.Settings.Default.Token);
            await client.StartAsync();
            Console.WriteLine("ログイン完了");
            // 始めのあいさつ(大事)
            var task = Task.Run(() =>
            {
                using (var start = new BouyomiChanClient())
                {
                    start.AddTalkTask("棒読みちゃん起動～！");
                }
            });
            // メッセージ受信時のイベントを追加
            client.MessageReceived += Talk;
            // 各種コマンド
            InputCommand();
            // 終わりのあいさつ(大事)
            task = Task.Run(() =>
            {
                using (var end = new BouyomiChanClient())
                {
                    end.AddTalkTask("棒読みちゃん終了～！");
                }
            });
            Console.WriteLine("キー入力で終了");
            Console.ReadLine();
        }

        /// <summary>
        /// メッセージを受け取った時の処理
        /// </summary>
        static async Task Talk(SocketMessage arg)
        {
            await Task.Run(() =>
            {
                // チャンネル一覧にある・ユーザーID一覧にあるなら
                if (Properties.Settings.Default.TextChannels.IndexOf(arg.Channel.Id.ToString()) >= 0 ||
                Properties.Settings.Default.UserIDs.IndexOf(arg.Author.Id.ToString()) >= 0)
                {
                    // 読み上げ
                    using (var bc = new BouyomiChanClient())
                    {
                        bc.AddTalkTask(arg.Content);
                    }
                }
            });
        }

        /// <summary>
        /// トークンのチェックをします
        /// </summary>
        static async Task CheckTokenAsync()
        {
            // トークンが空ならメールアドレスとパスワードでトークンを取得
            while (string.IsNullOrEmpty(Properties.Settings.Default.Token) == true)
            {
                Console.Write("メールアドレス : ");
                var email = Console.ReadLine();
                Console.Write("パスワード : ");
                var password = Console.ReadLine();
                try
                {
                    Properties.Settings.Default.Token = await DiscordApiHelper.LogInAsync(email, password);
                    Properties.Settings.Default.Save();
                }
                catch
                {
                    Console.WriteLine("ログイン失敗");
                }
            }
        }

        /// <summary>
        /// コマンドをチェックします
        /// </summary>
        static void InputCommand()
        {
            const int idLength = 18;
            Console.WriteLine("help でコマンド一覧を表示\n");
            while (true)
            {
                Console.Write("> ");
                var str = Console.ReadLine();
                var input = str.Split(' ');
                ulong i;
                int pad = 19;
                switch (input[0])
                {
                    case "help":
                        // コマンド一覧を表示
                        Console.WriteLine("コマンド一覧");
                        Console.WriteLine($"{"  help".PadRight(pad)} : コマンド一覧を表示");
                        Console.WriteLine($"{"  add TextChannelID".PadRight(pad)} : 読み上げるテキストチャンネルの追加");
                        Console.WriteLine($"{"  del TextChannelID".PadRight(pad)} : 読み上げるテキストチャンネルの削除");
                        Console.WriteLine($"{"  list".PadRight(pad)} : 読み上げるテキストチャンネルの列挙");
                        Console.WriteLine($"{"  adduser UserID".PadRight(pad)} : 読み上げるユーザーIDの追加");
                        Console.WriteLine($"{"  deluser UserID".PadRight(pad)} : 読み上げるユーザーIDの削除");
                        Console.WriteLine($"{"  userlist".PadRight(pad)} : 読み上げるユーザーIDの列挙");
                        Console.WriteLine($"{"  reset".PadRight(pad)} : トークンのリセット(要再ログイン)");
                        Console.WriteLine($"{"  exit".PadRight(pad)} : 終了");
                        break;
                    case "add":
                        // 追加
                        // 引数の判定
                        if (input.Length == 2 && ulong.TryParse(input[1], out i) && input[1].Length == idLength)
                        {
                            // ダブってないなら追加
                            if (Properties.Settings.Default.TextChannels.IndexOf(input[1]) < 0)
                            {
                                Properties.Settings.Default.TextChannels.Add(input[1]);
                                Properties.Settings.Default.Save();
                                Console.WriteLine($"{input[1]} を追加");
                            }
                            else
                            {
                                Console.WriteLine("既に登録されています");
                            }
                        }
                        else
                        {
                            Console.WriteLine("引数が不正です");
                        }
                        break;
                    case "del":
                        // 削除
                        // 引数の判定
                        if (input.Length == 2 && ulong.TryParse(input[1], out i) && input[1].Length == idLength)
                        {
                            // あったら削除
                            if (Properties.Settings.Default.TextChannels.IndexOf(input[1]) >= 0)
                            {
                                Properties.Settings.Default.TextChannels.Remove(input[1]);
                                Properties.Settings.Default.Save();
                                Console.WriteLine($"{input[1]} を削除");
                            }
                            else
                            {
                                Console.WriteLine("登録されていません");
                            }
                        }
                        else
                        {
                            Console.WriteLine("引数が不正です");
                        }
                        break;
                    case "adduser":
                        // 追加
                        // 引数の判定
                        if (input.Length == 2 && ulong.TryParse(input[1], out i) && input[1].Length == idLength)
                        {
                            // ダブってないなら追加
                            if (Properties.Settings.Default.UserIDs.IndexOf(input[1]) < 0)
                            {
                                Properties.Settings.Default.UserIDs.Add(input[1]);
                                Properties.Settings.Default.Save();
                                Console.WriteLine($"{input[1]} を追加");
                            }
                            else
                            {
                                Console.WriteLine("既に登録されています");
                            }
                        }
                        else
                        {
                            Console.WriteLine("引数が不正です");
                        }
                        break;
                    case "deluser":
                        // 削除
                        // 引数の判定
                        if (input.Length == 2 && ulong.TryParse(input[1], out i) && input[1].Length == idLength)
                        {
                            // あったら削除
                            if (Properties.Settings.Default.UserIDs.IndexOf(input[1]) >= 0)
                            {
                                Properties.Settings.Default.UserIDs.Remove(input[1]);
                                Properties.Settings.Default.Save();
                                Console.WriteLine($"{input[1]} を削除");
                            }
                            else
                            {
                                Console.WriteLine("登録されていません");
                            }
                        }
                        else
                        {
                            Console.WriteLine("引数が不正です");
                        }
                        break;
                    case "list":
                        // 全部表示
                        foreach (var s in Properties.Settings.Default.TextChannels)
                        {
                            Console.WriteLine($"  {s}");
                        }
                        break;
                    case "userlist":
                        // 全部表示
                        foreach (var s in Properties.Settings.Default.UserIDs)
                        {
                            Console.WriteLine($"  {s}");
                        }
                        break;
                    case "reset":
                        // トークンをリセット
                        Properties.Settings.Default.Token = null;
                        Properties.Settings.Default.Save();
                        Console.WriteLine("トークンをリセットしました");
                        return;
                    case "exit":
                        // さようなら
                        return;
                }
                Console.WriteLine();
            }
        }
    }
}
