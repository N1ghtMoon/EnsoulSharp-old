namespace SharpShooter.MyBase
{
    #region

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using EnsoulSharp;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using SharpShooter.MyCommon;

    #endregion

    public class MyChampions
    {
        private static readonly string[] all =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista", "KogMaw", "MissFortune",
            "Lucian", "Quinn", "Sivir", "Tristana", "TwistedFate", "Twitch", "Urgot", "Varus", "Vayne", "Xayah", "Viktor"
        };

        public MyChampions()
        {
            Initializer();
        }

        private static void Initializer()
        {
            MyMenuExtensions.myMenu = new Menu(ObjectManager.Player.CharacterName,
                "SharpShooter: " + ObjectManager.Player.CharacterName, true);
            MyMenuExtensions.myMenu.Attach();

            var supportMenu = new Menu("SharpShooter.SupportChampion", "Support Champion");
            {
                foreach (var name in all)
                {
                    supportMenu.Add(new MenuSeparator("SharpShooter.SupportChampion.SC_" + name, name));
                }
            }
            MyMenuExtensions.myMenu.Add(supportMenu);

            MyMenuExtensions.myMenu.Add(new MenuSeparator("ASDASDG", " "));

            if (
                all.All(
                    x =>
                        !string.Equals(x, ObjectManager.Player.CharacterName,
                            StringComparison.CurrentCultureIgnoreCase)))
            {
                MyMenuExtensions.myMenu.Add(
                    new MenuSeparator("NotSupport_" + ObjectManager.Player.CharacterName,
                        "Not Support: " + ObjectManager.Player.CharacterName));
                Console.WriteLine("SharpShooter: " + ObjectManager.Player.CharacterName +
                       " Not Support!");
                return;
            }

            LoadChampionsPlugin();

            Console.WriteLine("SharpShooter: " + ObjectManager.Player.CharacterName +
                              " Load Successful! Made By NightMoon");
            Console.WriteLine("QQ群: 598027398");

            Chat.Print("<font size='26'><font color='#9999CC'>SharpShooter</font></font> <font color='#FF5640'> Load Successful! Made By NightMoon</font>");
            Chat.Print("<font color='#FF5640'>CN QQ QUN: 598027398 </font>");
        }

        public static object LoadChampionsPlugin()
        {
            var championPlugin = Assembly.GetExecutingAssembly()
                                         .GetTypes()
                                         .Where(t => t.IsClass && 
                                                     t.Namespace == "SharpShooter.MyPlugin" && 
                                                     t.Name == ObjectManager.Player.CharacterName)
                                         .ToList()
                                         .FirstOrDefault();
            if (championPlugin != null)
            {
                return NewInstance(championPlugin);
            }

            return null;
        }

        //Credits to Kurisu
        public static object NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            if (target != null)
            {

                var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
                if (dynamic != null)
                {
                    var il = dynamic.GetILGenerator();

                    il.DeclareLocal(target.DeclaringType);
                    il.Emit(OpCodes.Newobj, target);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ret);

                    var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
                    return method();
                }
            }
            return null;
        }
    }
}
