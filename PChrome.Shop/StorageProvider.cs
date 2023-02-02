using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.Shop;

public abstract class StorageProvider<T> : IStorageProvider
{
    public abstract string Name { get; }
    protected abstract bool TryGiveTo(TSPlayer player, T content);
    protected abstract bool TryTakeFrom(TSPlayer player, int count, out T content, bool inf);
    protected abstract string SerializeToText(T content);

    public bool TryGiveTo(TSPlayer player, string content)
    {
        return this.TryGiveTo(player, JsonConvert.DeserializeObject<T>(content));
    }

    public bool TryTakeFrom(TSPlayer player, int count, out string content, bool inf)
    {
        var result = this.TryTakeFrom(player, count, out T t, inf);
        content = JsonConvert.SerializeObject(t);
        return result;
    }

    public string SerializeToText(string content)
    {
        return this.SerializeToText(JsonConvert.DeserializeObject<T>(content));
    }
}