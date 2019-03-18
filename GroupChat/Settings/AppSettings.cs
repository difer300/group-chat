using System;

namespace GroupChat.Settings
{
    public static class AppSettings
    {
        public static readonly string Cluster = "us2";
        public static readonly string AppId = "736424";
        public static readonly string AppKey = "afd58500ef516b630cad";
        public static readonly string AppSecret = "210ce2baa549868f04b1";
        public static readonly string PartialGroupName = "private-";
        public static readonly string NewMessageEvent = "new_message";
        public static readonly int LimitLoadMessages = 50;
        public static readonly string ConnectionName = "GroupChat";
        public static readonly string CommandChat = $"/stock=APPL";
        public static readonly string BotUser = "Bot";
        public static readonly Uri StooqUrl = new Uri($"https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv");
        public static readonly string BotMessage = $"APPL quote is ${0} per share";
    }
}
