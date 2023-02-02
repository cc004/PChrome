using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;

namespace PChrome.Core;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public Plugin(Main game) : base(game)
    {
    }
}