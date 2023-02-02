using LazyUtils;
using LazyUtils.Commands;
using Newtonsoft.Json.Linq;
using Rests;
using System.Linq;
using TShockAPI;

namespace PChrome.DeathTimes;

[Rest("deathtimes")]
public static class Rests
{
    [Permissions("deathtimes.admin")]
    public static JToken rankboard(RestRequestArgs args)
    {
        var i = 0;
        using var context = Db.Context<DeathTimes>();
        return new JArray
        (
            context.Config.OrderByDescending((tuple) => tuple.times)
                .AsEnumerable().Select((tuple) => new JObject
                {
                    ["times"] = (long) tuple.times,
                    ["rank"] = ++i,
                    ["name"] = tuple.name
                })
        );
    }
}