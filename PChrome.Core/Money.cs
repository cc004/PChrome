using LazyUtils;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PChrome.Core;

public class Money : PlayerConfigBase<Money>
{
    [Column]
    public int money { get; set; }
}