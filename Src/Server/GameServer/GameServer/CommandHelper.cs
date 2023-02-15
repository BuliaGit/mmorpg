using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace GameServer
{
    /// <summary>
    /// 控制台帮助类
    /// </summary>
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine().ToLower().Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Help();
                    continue;
                }
                try
                {
                    string[] cmd = line.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    switch (cmd[0])
                    {
                        case "addexp":
                            AddExp(int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "exit":
                            run = false;
                            break;
                        //todo:加钱
                        default:
                            Help();
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        private static void AddExp(int characterId, int exp)
        {
            var cha = Managers.CharacterManager.Instance.GetCharacter(characterId);
            if (cha == null)
            {
                Console.WriteLine("characterId {0} not found", characterId);
                return;
            }
            cha.AddExp(exp);
        }

        public static void Help()
        {
            Console.Write(@"
Help:
    exit    Exit Game Server
    help    Show Help
");
        }
    }
}
