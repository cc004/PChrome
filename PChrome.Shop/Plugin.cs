using LazyUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;

namespace PChrome.Shop;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    private static readonly Dictionary<string, IStorageProvider> providers = new Dictionary<string, IStorageProvider>();
    public static void RegisterProvider(IStorageProvider provider)
    {
        providers.Add(provider.Name, provider);
    }

    public static IStorageProvider GetProvider(string name)
    {
        return providers[name];
    }

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        RegisterProvider(new InventoryProvider());
    }
}