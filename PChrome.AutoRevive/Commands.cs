using LazyUtils;
using LazyUtils.Commands;
using LinqToDB;
using System.Linq;
using TShockAPI;

namespace PChrome.AutoRevive;

[Command("arc", "复活币")]
public static class Commands
{
    [Alias("给予"), Permissions("autorevive.admin")]
    public static void Give(CommandArgs args, string player, int amount)
    {
        using var query = Db.Get<AutoReviveCoin>(player);
        query.Set(d => d.count, d => d.count + amount).Update();
        args.Player.SendSuccessMessage(amount >= 0 ? $"成功给予{player} {amount}个" : $"成功扣除{player} {-amount}个");
    }
    [Alias("查看"), Permissions("autorevive.admin")]
    public static void Check(CommandArgs args, string player)
    {
        using var query = Db.Get<AutoReviveCoin>(player);
        args.Player.SendInfoMessage($"目标玩家的复活币数:{query.Single().count}个");
    }
    [Permissions("autorevive.player"), RealPlayer, Main]
    public static void Main(CommandArgs args)
    {
        using var query = args.Player.Get<AutoReviveCoin>();
        args.Player.SendInfoMessage($"当前复活币总额:{query.Single().count}个");
    }
    [Permissions("autorevive.player")]
    public static void Default(CommandArgs args)
    {
        args.Player.SendInfoMessage("/arc give <玩家> <数量>\n" +
                                    "/arc check <玩家>");
    }

}