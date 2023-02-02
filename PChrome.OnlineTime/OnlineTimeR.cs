using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace PChrome.OnlineTime;

public class OnlineTimeR : PlayerConfigBase<OnlineTimeR>
{
    public int daily { get; set; }
    public int total { get; set; }
    public int downgrade_count { get; set; }
    public bool is_admin { get; set; }
}