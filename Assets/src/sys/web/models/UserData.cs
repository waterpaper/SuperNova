using System.Collections.Generic;

namespace Supernova.Api.Models
{
    public class UserData
    {
        public decimal Gold { get; set; }
        public long Soul { get; set; }
        public long Ruby { get; set; }

        public long Kill { get; set; }

        public IEnumerable<Unity.ItemInstance> Items { get; set; }
        public string Weapon { get; set; }
        public string Armor { get; set; }
        public string AccessoryL { get; set; }
        public string AccessoryR { get; set; }

        public long Attack { get; set; }
        public long AttackSpeed { get; set; }
        public long AttackSpeedAmp { get; set; }
        public long CooltimeDecrease { get; set; }
        public long CriticalProbability { get; set; }
        public long CriticalDamage { get; set; }
        public long Penetration { get; set; }
        public long DamageAmpForBoss { get; set; }
        public long DamageAmp { get; set; }
        public long AdditionalDamage { get; set; }
        public long GoldGain { get; set; }
        public long GoldGainAmp { get; set; }
        public long LuckRate { get; set; }
        public long LuckForce { get; set; }
        public long GoldPerSec { get; set; }
    }
}
