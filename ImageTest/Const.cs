using System;

namespace ImageTest
{
    public static class Const
    {
        public static event Action<string> OnLogMessage;

        private static void FireOnLogMessage(string obj)
        {
            Action<string> handler = OnLogMessage;
            if (handler != null) handler(obj);
        }

        public const string Url = "http://theworldsnotes.com/images/Spoils_logo_trans.png";

        public static void Log(string message, params object[] args)
        {
            FireOnLogMessage(string.Format(message,args));
        }
    }
}