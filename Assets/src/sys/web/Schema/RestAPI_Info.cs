using System.Collections.Generic;

namespace Supernova.Api.Rest.Info
{
    [RequestAttribute("info/load", NetworkMethod.Get)]
    public class UserGameInfoReq : ReqMessage<UserGameInfo>
    {

    }

    [RequestAttribute("info/save", NetworkMethod.Post)]
    public class UserGameInfoSaveReq : ReqMessage<UserGameInfo>
    {
        public string ID { get; set; }

        public decimal Gold { get; set; }
        public long Soul { get; set; }
        public long Ruby { get; set; }

        public long Kill { get; set; }

        public List<Unity.ItemInstance> Items { get; set; }
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

    public class UserGameInfo
    {
        public string ID { get; set; }

        public decimal Gold { get; set; }
        public long Soul { get; set; }
        public long Ruby { get; set; }

        public long Kill { get; set; }

        public List<Unity.ItemInstance> Items { get; set; }
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
        public UserGameInfo()
        {

        }

        public UserGameInfo(DummyUserData dummyData)
        {
            Gold = dummyData.Gold;
            Soul = dummyData.Soul;
            Ruby = dummyData.Ruby;

            Kill = dummyData.Kill;
            Items = new();
            foreach (var item in dummyData.Items) { Items.Add(item); }
            Weapon = dummyData.Weapon;
            Armor = dummyData.Armor;
            AccessoryL = dummyData.AccessoryL;
            AccessoryR = dummyData.AccessoryR;

            Attack = dummyData.Attack;
            AttackSpeed = dummyData.AttackSpeed;
            AttackSpeedAmp = dummyData.AttackSpeedAmp;
            CooltimeDecrease = dummyData.CooltimeDecrease;
            CriticalProbability = dummyData.CriticalProbability;
            CriticalDamage = dummyData.CriticalDamage;
            Penetration = dummyData.Penetration;
            DamageAmpForBoss = dummyData.DamageAmpForBoss;
            DamageAmp = dummyData.DamageAmp;
            AdditionalDamage = dummyData.AdditionalDamage;
            GoldGain = dummyData.GoldGain;
            GoldGainAmp = dummyData.GoldGainAmp;
            LuckRate = dummyData.LuckRate;
            LuckForce = dummyData.LuckForce;
            GoldPerSec = dummyData.GoldPerSec;
        }
    }
}
