using System;
using System.Collections.Generic;

namespace Hifumi.Personality
{
    public class Emotes
    {
        public static string GetEmote(EmoteType emoteType)
        {
            Random r = new Random();
            switch (emoteType)
            {
                case EmoteType.Happy: return happyEmotes[r.Next(happyEmotes.Count)];
                case EmoteType.Sad: return sadEmotes[r.Next(sadEmotes.Count)];
            }
            return string.Empty;
        }

        private static List<string> happyEmotes = new List<string>()
        {   // TODO: More please
            @"(\*^▽^\*)",
            @"(^▽^)",
            @"(＾▽＾)",
            @"(\*≧▽≦)",
            @"(≧∇≦)/",
            @"(ノ\*゜▽゜\*)",
            @"o(≧∇≦o)",
            @"Ｏ(≧∇≦)Ｏ",
            @"(๑꒪▿꒪)\*",
            @"(=^▽^=)"
        };

        private static List<string> sadEmotes = new List<string>()
        {   // TODO: More sad?
            @"( ≧Д≦)",
            @"(｡•́︿•̀｡)",
            @"(っ- ‸ – ς)",
            @"(⌯˃̶᷄ ﹏ ˂̶᷄⌯)",
            @"(;\*△\*;)",
            @"꒰๑•̥﹏•̥๑꒱",
            @"(╯︵╰,)",
            @"(ᗒᗣᗕ)՞",
            @"(◞‸◟；)"
        };
    }

    public enum EmoteType
    {
        Happy,
        Sad
    }
}
