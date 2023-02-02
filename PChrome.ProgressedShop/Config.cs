using LazyUtils;
using PChrome.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PChrome.ProgressedShop;

public class Config : Config<Config>
{
    public ProgressItem[] items;
}
public class ProgressItem
{
    public string[] include;
    public string[] exclude;
    public ProtoItemWithPrice[] items;

    private Func<bool> predict;
    internal bool lastpred;

    internal bool Predict
    {
        get
        {
            this.predict ??= LazyUtils.Utils.Eval(this.include, this.exclude);
            return this.predict();
        }
    }
}