using LazyUtils;
using LazyUtils.Commands;
using Newtonsoft.Json.Linq;
using Rests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.Shop;

[Rest("shop")]
public static class Rests
{
    [Permissions("economy.shop.player")]
    public static JToken getshopitems(RestRequestArgs args)
    {
        using var context = Db.Context<ShopItem>();
        return JToken.FromObject(context.Config.OrderByDescending(d => d.id).ToArray());
    }
}