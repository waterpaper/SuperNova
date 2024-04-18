using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Api
{
    public class DummyUserData
    {
        public decimal Gold { get; set; }
        public long Soul { get; set; }
        public long Ruby { get; set; }

        public long Kill { get; set; }

        public List<Unity.ItemInstance> Items { get; set; } = new();
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


        /// <summary>
        /// ���丮������ �����͸� �ҷ��ɴϴ�.
        /// </summary>
        /// <returns></returns>
        public async UniTask<string> LoadAsync()
        {
            var path = $"{Application.persistentDataPath}/.dat";
            if (!System.IO.File.Exists(path))
            {
                Log.Info("dat not found");
                return null;
            }

            try
            {
                var text = await System.IO.File.ReadAllTextAsync(path);
                var data = JsonConvert.DeserializeObject<DummyUserData>(text);

                Gold = data.Gold;
                Soul = data.Soul;
                Ruby = data.Ruby;

                Kill = data.Kill;
                Items = new();

                foreach (var item in data.Items) { Items.Add(item); }
                Weapon = data.Weapon;
                Armor = data.Armor;
                AccessoryL = data.AccessoryL;
                AccessoryR = data.AccessoryR;

                Attack = data.Attack;
                AttackSpeed = data.AttackSpeed;
                AttackSpeedAmp = data.AttackSpeedAmp;
                CooltimeDecrease = data.CooltimeDecrease;
                CriticalProbability = data.CriticalProbability;
                CriticalDamage = data.CriticalDamage;
                Penetration = data.Penetration;
                DamageAmpForBoss = data.DamageAmpForBoss;
                DamageAmp = data.DamageAmp;
                AdditionalDamage = data.AdditionalDamage;
                GoldGain = data.GoldGain;
                GoldGainAmp = data.GoldGainAmp;
                LuckRate = data.LuckRate;
                LuckForce = data.LuckForce;
                GoldPerSec = data.GoldPerSec;

                /*
                // validate
                if (this.Inventory.Items.Any(p => this.Inventory.Items.Where(q => q.Guid == p.Guid).Count() > 1))
                {
                    Log.Warning("���� ����Ű�� �������� ���� �� �߰ߵǾ����ϴ�. �κ��丮�� �ʱ�ȭ �մϴ�.");
                    this.Inventory.Items.Clear();
                    this.Inventory.Weapon.Value = null;
                    this.Inventory.Armor.Value = null;
                    this.Inventory.AccessoryLeft.Value = null;
                    this.Inventory.AccessoryRight.Value = null;
                }*/

                Log.Info("LOAD DATA : " + text);
                return text;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
                //Log.Warning("������ �ε忡 �����߽��ϴ�. ������ ���� �����մϴ�.");
            }
        }

        /// <summary>
        /// �����͸� ���丮���� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        public async void Save(string str)
        {
            var info = JsonConvert.DeserializeObject<Rest.Info.UserGameInfo>(str);

            Gold = info.Gold;
            Soul = info.Soul;
            Ruby = info.Ruby;

            Kill = info.Kill;

            Items = info.Items;
            Weapon = info.Weapon;
            Armor = info.Armor;
            AccessoryL = info.AccessoryL;
            AccessoryR = info.AccessoryR;

            Attack = info.Attack;
            AttackSpeed = info.AttackSpeed;
            AttackSpeedAmp = info.AttackSpeedAmp;
            CooltimeDecrease = info.CooltimeDecrease;
            CriticalProbability = info.CriticalProbability;
            CriticalDamage = info.CriticalDamage;
            Penetration = info.Penetration;
            DamageAmpForBoss = info.DamageAmpForBoss;
            DamageAmp = info.DamageAmp;
            AdditionalDamage = info.AdditionalDamage;
            GoldGain = info.GoldGain;
            GoldGainAmp = info.GoldGainAmp;
            LuckRate = info.LuckRate;
            LuckForce = info.LuckForce;
            GoldPerSec = info.GoldPerSec;

            var text = JsonConvert.SerializeObject(this);
            Log.Info("SAVE DATA : " + text);

            await System.IO.File.WriteAllTextAsync($"{Application.persistentDataPath}/.dat", text);
        }
    }
}

