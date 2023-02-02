using LazyUtils;
using LazyUtils.Commands;
using LinqToDB;
using PChrome.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.Shop;

[Command("shop")]
public static class Commands
{
    private const int Pagelimit = 20;

    private static TSPlayer GetOnline(string name)
    {
        return TShock.Players.FirstOrDefault(plr => plr?.Account?.Name == name);
    }

    [Alias("列表"), Permissions("economy.shop.player")]
    public static void List(CommandArgs args, int page)
    {
        using var context = Db.Context<ShopItem>();
        var sb = new StringBuilder();
        sb.AppendLine($"当前商店商品({page}/{(int) Math.Round(context.Config.Count() / (double) Pagelimit)}):");
        sb.Append(string.Join("\n",
            context.Config.OrderByDescending(d => d.id).Skip((page - 1) * Pagelimit).Take(Pagelimit)));
        args.Player.SendInfoMessage(sb.ToString());
    }

    [Alias("列表"), Permissions("economy.shop.player")]
    public static void List(CommandArgs args)
    {
        List(args, 1);
    }

    [Alias("购买"), Permissions("economy.shop.player"), RealPlayer]
    public static void Buy(CommandArgs args, int index)
    {
        using var context = Db.Context<ShopItem>();
        var item = context.Config.SingleOrDefault(d => d.id == index);
        if (item == null)
        {
            args.Player.SendErrorMessage("商品索引无效");
            return;
        }

        using var query = args.Player.Get<Money>();
        var money = query.Single().money;
        if (money < item.price)
        {
            args.Player.SendErrorMessage($"拥有的货币不足,还需要{item.price - money}$");
            return;
        }

        if (!item.TryGiveTo(args.Player))
        {
            args.Player.SendErrorMessage("背包已满");
            return;
        }

        query.Set(d => d.money, d => d.money - item.price).Update();
        args.Player.NoticeChange(-item.price);
        ;
        if (!item.infinity)
        {
            context.Config.Where(d => d.id == item.id).Delete();
        }

        args.Player.SendSuccessMessage("购买成功！");

        if (string.IsNullOrEmpty(item.owner))
        {
            return;
        }

        using (var query2 = Db.Get<Money>(item.owner))
        {
            query2.Set(d => d.money, d => d.money + item.price).Update();
        }

        var target = GetOnline(item.owner);
        if (target == null)
        {
            return;
        }

        target.SendSuccessMessage($"玩家[{args.Player.Name}]购买了您的{item}");
        target.NoticeChange(item.price);
    }

    private static void AddShopItem(CommandArgs args, string provider, int count, int price, bool infinity)
    {
        var pro = Plugin.GetProvider(provider);
        if (!pro.TryTakeFrom(args.Player, count, out var content, infinity))
        {
            args.Player.SendErrorMessage("物品数量不足");
            return;
        }

        using (var context = Db.Context<ShopItem>())
        {
            context.Config.Insert(() => new ShopItem
            {
                content = content,
                price = price,
                owner = infinity ? null : args.Player.Account.Name,
                provider = provider,
                infinity = infinity
            });
        }

        args.Player.SendSuccessMessage("商品已添加！");
    }

    [Alias("出售"), Permissions("economy.shop.player"), RealPlayer]
    public static void Sell(CommandArgs args, string provider, int count, int price)
    {
        AddShopItem(args, provider, count, price, false);
    }

    [Alias("添加"), Permissions("economy.shop.admin"), RealPlayer]
    public static void Add(CommandArgs args, string provider, int count, int price)
    {
        AddShopItem(args, provider, count, price, true);
    }

    [Alias("出售"), Permissions("economy.shop.player"), RealPlayer]
    public static void Sell(CommandArgs args, int count, int price)
    {
        Sell(args, "物品", count, price);
    }

    [Alias("添加"), Permissions("economy.shop.admin"), RealPlayer]
    public static void Add(CommandArgs args, int count, int price)
    {
        Add(args, "物品", count, price);
    }

    [Alias("删除"), Permissions("economy.shop.player"), RealPlayer]
    public static void Del(CommandArgs args, int index)
    {
        using var context = Db.Context<ShopItem>();
        var item = context.Config.SingleOrDefault(d => d.id == index);
        if (item == null)
        {
            args.Player.SendErrorMessage("商品索引无效");
            return;
        }

        if (item.owner != args.Player.Account.Name && !args.Player.HasPermission("economy.shop.admin"))
        {
            args.Player.SendErrorMessage("你没有权限删除别人的商品");
            return;
        }

        if (!item.TryGiveTo(args.Player))
        {
            args.Player.SendErrorMessage("背包已满");
            return;
        }

        context.Config.Where(d => d.id == index).Delete();
        args.Player.SendSuccessMessage("商品已下架！");
    }

    public static void Default(CommandArgs args)
    {
        args.Player.SendInfoMessage("用法:\n" +
                                    "/shop add [类型] <个数> <价格>\n" +
                                    "/shop del <商品索引>\n" +
                                    "/shop sell [类型] <个数> <价格>\n" +
                                    "/shop buy <商品索引>\n" +
                                    "/shop list [页码]");
    }
}