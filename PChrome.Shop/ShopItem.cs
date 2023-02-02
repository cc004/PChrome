using LazyUtils;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace PChrome.Shop;

public class ShopItem : ConfigBase<ShopItem>
{
    internal IStorageProvider Provider => Plugin.GetProvider(this.provider);

    [Identity, PrimaryKey]
    public int id { get; set; }
    [Column]
    public string provider { get; set; }
    [Column]
    public string content { get; set; }
    [Column]
    public int price { get; set; }
    [Column]
    public bool infinity { get; set; }
    [Column]
    public string owner { get; set; }

    public override string ToString()
    {
        return $"{this.id:D3}.{Plugin.GetProvider(this.provider).SerializeToText(this.content)} {this.price}$({(this.infinity ? "无限" : $"由{this.owner}出售")})";
    }

    internal bool TryGiveTo(TSPlayer player)
    {
        return this.Provider.TryGiveTo(player, this.content);
    }
}