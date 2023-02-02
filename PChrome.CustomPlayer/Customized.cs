using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PChrome.CustomPlayer;

public class Customized : PlayerConfigBase<Customized>
{
    public string permission { get; set; }
    public string prefix { get; set; }
    public string suffix { get; set; }
    public string color { get; set; }
}