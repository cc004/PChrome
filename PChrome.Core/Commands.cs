using LazyUtils;
using LazyUtils.Commands;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.Core;

[Command("eco", "经济")]
public static class Commands
{
    [Alias("支付"), Permissions("economy.player"), RealPlayer]
    public static void Pay(CommandArgs args, string player, int amount)
    {
        var target = TShock.Players.SingleOrDefault(u => u?.Account?.Name == player);
        if (target == null)
        {
            args.Player.SendErrorMessage("为防止误操作，你只能支付给在线玩家！");
            return;
        }

        using var query = args.Player.Get<Money>();
        if (query.Single().money < amount || amount < 0)
        {
            args.Player.SendErrorMessage("指定的金额无效");
            return;
        }

        using var query2 = target.Get<Money>();
        query.Set(d => d.money, d => d.money - amount).Update();
        query2.Set(d => d.money, d => d.money + amount).Update();
        args.Player.SendSuccessMessage($"成功向玩家{target.Name}支付{amount}$");
        args.Player.NoticeChange(-amount);
        target.NoticeChange(amount);
    }

    [Alias("给予"), Permissions("economy.admin")]
    public static void Give(CommandArgs args, string player, int amount)
    {
        using var query = Db.Get<Money>(player);
        query.Set(d => d.money, d => d.money + amount).Update();
        args.Player.NoticeChange(amount);
        args.Player.SendSuccessMessage(amount >= 0 ? $"成功给予{player} {amount}$" : $"成功扣除{player} {-amount}$");
    }

    [Alias("查看"), Permissions("economy.admin")]
    public static void Check(CommandArgs args, string player)
    {
        using var query = Db.Get<Money>(player);
        args.Player.SendInfoMessage($"目标玩家的货币数:{query.Single().money}$");
    }

    [Permissions("economy.player"), RealPlayer, Main]
    public static void Main(CommandArgs args)
    {
        using var query = args.Player.Get<Money>();
        args.Player.SendInfoMessage($"当前货币总额:{query.Single().money}$");
    }

}