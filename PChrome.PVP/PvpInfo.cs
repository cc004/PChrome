using LazyUtils;
using System.Linq;
using System.Text;
using TShockAPI;

namespace PChrome.PVP;

internal class PvpInfo : PlayerConfigBase<PvpInfo>
{
    public int score { get; set; }
    public int killcount { get; set; }
    public int deathcount { get; set; }
    public long causedamage { get; set; }
    public long getdamage { get; set; }
    public int kill { get; set; }
    public int death { get; set; }

    private float GetPercent()
    {
        return this.killcount * 100f / (this.killcount + this.deathcount);
    }

    public string GetQueryString()
    {
        var groupname = TShock.UserAccounts.GetUserAccountByName(this.name)?.Group;
        var group = TShock.Groups.FirstOrDefault(g => g.Name == groupname);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("--- ����֮��-����PVP ---");
        stringBuilder.AppendLine($"- ��ң�{this.name} ��λ��{((@group != null) ? @group.Prefix : "δ֪")}");
        stringBuilder.AppendLine($"- ����: {this.score}");
        stringBuilder.AppendLine(
            $"- ս����{this.killcount.Color("A2D883")} ʤ {this.deathcount.Color("D8A183")} �� ʤ�ʣ�{this.GetPercent():0.00}��");
        return stringBuilder.ToString();
    }

    public string GetShortMessage()
    {
        return $"{this.name}: {this.killcount} ʤ {this.deathcount} ��, ʤ�� {this.GetPercent():0.00}��";
    }
}