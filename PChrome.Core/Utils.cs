using LazyUtils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace PChrome.Core;

public static class Utils
{
    private static void SendCombatText(Vector2 pos, string text, Color color, int remoteClient = -1, int ignoreClient = -1)
    {
        NetMessage.SendData(119, remoteClient, ignoreClient, NetworkText.FromLiteral(text), (int) color.PackedValue, pos.X, pos.Y);
    }

    public static void NoticeChange(this TSPlayer plr, int val)
    {
        if (val == 0)
        {
            return;
        }

        if (plr.GetData<long>("spamTimer") - TimingUtils.Timer > -60)
        {
            return;
        }

        plr.SetData("spamTimer", TimingUtils.Timer);

        SendCombatText(plr.TPlayer.Top, val > 0 ? $"+{val}$" : $"{val}$", Color.Yellow, plr.Index);
    }
    public static void NoticeChange(this string account, int val)
    {
        var plr = TShock.Players.FirstOrDefault(p => p?.Account?.Name == account);
        if (plr == null)
        {
            return;
        }

        NoticeChange(plr, val);
    }
}