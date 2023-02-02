using LazyUtils;
using LazyUtils.Commands;
using Newtonsoft.Json.Linq;
using PChrome.OnlineTime;
using Rests;
using System;
using System.Linq;
using TShockAPI;

namespace PChrome.Ranking;

[Rest("ranking")]
public static class Rests
{
    [Permissions("ranking.query")]
    public static JToken query(RestRequestArgs args, string name)
    {
        var usr = TShock.UserAccounts.GetUserAccountByName(name);
        var data = TShock.CharacterDB.GetPlayerData(null, usr.ID);
        using var query = Db.Get<OnlineTimeR>(usr.Name);
        var q = query.Single();
        return new JObject
        {
            ["group"] = usr.Group,
            ["ip"] = usr.KnownIps,
            ["statLife"] = data.health,
            ["statLifeMax"] = data.maxHealth,
            ["statMana"] = data.mana,
            ["statManaMax"] = data.maxMana,
            ["questsCompleted"] = data.questsCompleted,
            ["daily"] = q.daily,
            ["onlinetime"] = q.total,
            ["inventory"] = new JArray(data.inventory.Select((item) => new JObject
            {
                ["id"] = item.NetId,
                ["prefix"] = item.PrefixId,
                ["stack"] = item.Stack
            })),
            ["online"] = TShock.Players.Any(p => p?.Account?.Name == usr.Name)
        };
    }
    [Permissions("ranking.quest")]
    public static JToken quest(RestRequestArgs args)
    {
        return new JArray
        (
            TShock.UserAccounts.GetUserAccounts()
                .Select(user => new Tuple<string, PlayerData>(user.Name, TShock.CharacterDB.GetPlayerData(null, user.ID)))
                .Where(tuple => tuple.Item2.exists)
                .Select(tuple => new Tuple<string, int>(tuple.Item1, tuple.Item2.questsCompleted))
                .OrderByDescending(tuple => tuple.Item2).Select((tuple, i) => new JObject
                {
                    ["times"] = tuple.Item2,
                    ["rank"] = i + 1,
                    ["name"] = tuple.Item1
                })
        );
    }

    [Permissions("ranking.item")]
    public static JToken item(RestRequestArgs args, int id)
    {
        return new JArray
        (
            TShock.UserAccounts.GetUserAccounts()
                .Select(user => new Tuple<string, PlayerData>(user.Name, TShock.CharacterDB.GetPlayerData(null, user.ID)))
                .Where(tuple => tuple.Item2.exists)
                .Select(tuple => new Tuple<string, int>(tuple.Item1, tuple.Item2.inventory
                    .Where(item => item.NetId == id)
                    .Sum(item => item.Stack)))
                .OrderByDescending(tuple => tuple.Item2).Select((tuple, i) => new JObject
                {
                    ["count"] = tuple.Item2,
                    ["rank"] = i + 1,
                    ["name"] = tuple.Item1
                })
        );
    }

    [Permissions("ranking.deathtimes")]
    public static JToken deathtimes(RestRequestArgs args)
    {
        using var context = Db.Context<DeathTimes.DeathTimes>();
        return new JArray
        (
            context.Config.OrderByDescending(tuple => tuple.times)
                .AsEnumerable().Select((tuple, i) => new JObject
                {
                    ["times"] = (long) tuple.times,
                    ["rank"] = i + 1,
                    ["name"] = tuple.name
                })
        );
    }
    [Permissions("ranking.onlinetime")]
    public static JToken totalonline(RestRequestArgs args)
    {
        using var context = Db.Context<OnlineTimeR>();
        return JToken.FromObject(context.Config.OrderByDescending(d => d.total).AsEnumerable()
            .Select((d, i) => new JObject
            {
                ["time"] = d.total * 3600L,
                ["rank"] = i + 1,
                ["name"] = d.name
            }));
    }
    [Permissions("ranking.onlinetime")]
    public static JToken dailyonline(RestRequestArgs args)
    {
        using var context = Db.Context<OnlineTimeR>();
        return JToken.FromObject(context.Config.OrderByDescending(d => d.daily).AsEnumerable()
            .Select((d, i) => new JObject
            {
                ["time"] = d.daily * 3600L,
                ["rank"] = i + 1,
                ["name"] = d.name
            }));
    }
}