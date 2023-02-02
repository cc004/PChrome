using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using PChrome.Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace PChrome.RPG;

public class MoneyAllocator
{
    private static readonly Random rnd = new Random();
    private static float FloatingCoefficient()
    {
        var cfg = Config.Instance;
        return (((float) rnd.NextDouble() * (cfg.FloatMoneyMax - cfg.FloatMoneyMin)) + cfg.FloatMoneyMin + 1f) * cfg.BaseMoney;
    }

    private readonly Dictionary<NPC, Dictionary<string, float>> damageDictionary =
        new Dictionary<NPC, Dictionary<string, float>>();

    public void AddDamage(NPC npc, string account, float damage)
    {
        if (npc.realLife > 0)
        {
            npc = Main.npc[npc.realLife];
        }

        if (!this.damageDictionary.ContainsKey(npc))
        {
            this.damageDictionary.Add(npc, new Dictionary<string, float>());
        }

        if (!this.damageDictionary[npc].ContainsKey(account))
        {
            this.damageDictionary[npc].Add(account, damage);
        }
        else
        {
            this.damageDictionary[npc][account] += damage;
        }
    }

    public void SettleNPC(NPC npc)
    {
        if (!this.damageDictionary.ContainsKey(npc))
        {
            return;
        }

        var coeff = npc.lifeMax / this.damageDictionary[npc].Sum(p => p.Value);
        using (var context = Db.Context<Money>())
        {
            foreach (var pair in this.damageDictionary[npc])
            {
                var val = (int) (pair.Value * coeff * FloatingCoefficient());
                context.Config.Where(d => d.name == pair.Key).Set(d => d.money, d => d.money + val).Update();
                pair.Key.NoticeChange(val);
            }
        }

        this.damageDictionary.Remove(npc);
    }

    public void Update()
    {
        foreach (var npc in this.damageDictionary.Keys.ToArray())
        {
            if (!Main.npc.Contains(npc) || !npc.active)
            {
                this.damageDictionary.Remove(npc);
            }
        }
    }
}