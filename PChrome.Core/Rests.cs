using LazyUtils;
using LazyUtils.Commands;
using LinqToDB;
using Newtonsoft.Json.Linq;
using Rests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.Core;

[Rest("economy")]
public static class Rests
{
    [Permissions("economy.admin")]
    public static JToken getmoneyrank(RestRequestArgs args)
    {
        using var context = Db.PlayerContext<Money>();
        return JToken.FromObject(context.Config.AsEnumerable()
            .ToDictionary(d => d.name, d => d.money).OrderByDescending(d => d.Value));
    }
    [Permissions("economy.admin")]
    public static JToken getplayermoney(RestRequestArgs args, string player)
    {
        using var query = Db.Get<Money>(player);
        return query.Single().money;
    }
    [Permissions("economy.admin")]
    public static JToken updateplayermoney(RestRequestArgs args, string player, int amount)
    {
        using var query = Db.Get<Money>(player);
        var result = query.Single().money;
        query.Set(d => d.money, d => amount).Update();
        return result;
    }
}