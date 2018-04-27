﻿using Hifumi.Addons;
using System;
using System.Linq;

namespace Hifumi.Helpers
{
    public class IntHelper
    {
        static double Percentage { get => 0.085; }
        public static int GetLevel(int xp) => (int)Math.Floor(Percentage * Math.Sqrt(xp));
        public static int NextLevelXP(int level) => (int)Math.Pow((level + 1) / Percentage, 2);

        public static int GetGuildRank(IContext context, ulong userId)
        {
            var profile = context.Server.Profiles.OrderByDescending(x => x.Value.ChatXP).FirstOrDefault(x => x.Key == userId);
            return context.Server.Profiles.ToList().IndexOf(profile);
        }
    }
}
