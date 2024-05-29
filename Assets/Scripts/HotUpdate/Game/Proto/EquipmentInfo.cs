using Koakuma.Game.Items;
using System.Collections.Generic;
using System.Text;

public class EquipmentInfo
{
    public int configID;
    public EquipmentQuality quality;
    public bool identified;
    public List<int> baseAttributes;
    public Dictionary<int, int[]> specialAttributes;

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        if (baseAttributes != null)
        {
            foreach (var attr in baseAttributes)
            {
                builder.Append($"[{attr}]");
            }
        }
        string baseAttr = builder.ToString();
        builder.Clear();

        if (specialAttributes != null)
        {
            foreach (var item in specialAttributes)
            {
                builder.Append($"[{item.Key},{item.Value}]");
            }
        }
        string specialAttr = builder.ToString();
        return $"ID:{configID}, Quality:{quality}, Identified:{identified}, BaseAttr:{baseAttr}, SpecialAttr:{specialAttr}";
    }
}

