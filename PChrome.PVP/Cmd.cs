using LazyUtils;
using LazyUtils.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Utilities;
using TShockAPI;
using TShockAPI.DB;

namespace PChrome.PVP;

[Command("join")]
internal class Cmd2
{
    [Permissions("cnspvp.use")]
    public static void Pvp(CommandArgs args)
    {
        var canSpawn = false;
        var settings = new Player.RandomTeleportationAttemptSettings
        {
            avoidLava = true,
            avoidHurtTiles = true,
            maximumFallDistanceFromOrignalPoint = 100,
            attemptsBeforeGivingUp = 1000
        };
        Main.rand ??= new UnifiedRandom();
        var vector = args.Player.TPlayer.CheckForGoodTeleportationSpot(ref canSpawn, Utils.PvPArea.Left, Utils.PvPArea.Width, Utils.PvPArea.Top, Utils.PvPArea.Height, settings);
        if (canSpawn)
        {
            args.Player.Teleport(vector.X, vector.Y, 1);
        }
        else
        {
            args.Player.SendErrorMessage("传送失败");
        }
    }
}

[Command("cnspvp")]
public static class Cmd
{
    [Permissions("cnspvp.admin")]
    public static void Info(CommandArgs args, string name)
    {
        using var query = Db.Get<PvpInfo>(name);
        args.Player.SendInfoMessage(query.Single().GetQueryString());
    }

    [Permissions("cnspvp.use"), RealPlayer]
    public static void Info(CommandArgs args)
    {
        using var query = args.Player.Get<PvpInfo>();
        args.Player.SendInfoMessage(query.Single().GetQueryString());
    }

    [Permissions("cnspvp.use")]
    public static void Rank(CommandArgs args)
    {
        using var query = Db.Context<PvpInfo>();
        args.Player.SendInfoMessage("排行:\n" + string.Join("\n", query.Config
            .OrderByDescending(i => i.killcount).ThenByDescending(i => i.deathcount)
            .Take(10).Select((inf, i) => $"{i + 1}. {inf.GetShortMessage()}")));
    }
}