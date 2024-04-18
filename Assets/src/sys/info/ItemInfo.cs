using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Supernova.Unity
{
    /// <summary>
    /// 아이템 enum입니다.
    /// </summary>
    public enum ItemType
    {
        Weapon = 0,
        Armor = 1,
        Accessory = 2,
    }

    /// <summary>
    /// 아이템 정보 정의입니다.
    /// </summary>
    public class ItemInfo
    {
        [JsonProperty]
        public long ItemID { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public string Description { get; private set; }
        [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
        public ItemType ItemType { get; private set; }
        /// <summary>
        /// 아이템 가격
        /// </summary>
        [JsonProperty]
        public long Price { get; private set; }
        /// <summary>
        /// 아이템 스탯
        /// </summary>
        [JsonProperty]
        public Stat Stat { get; private set; }
        /// <summary>
        /// 업그레이드 아이템 스탯
        /// </summary>
        [JsonProperty]
        public Stat UpgradeStat { get; private set; }
        /// <summary>
        /// 아이템이 지원하는 스킬ID 목록
        /// </summary>
        [JsonProperty]
        public IEnumerable<long> Skills { get; private set; }
        /// <summary>
        /// 아이템 드랍 확률
        /// </summary>
        [JsonProperty]
        public double DropRate { get; private set; }
    }



    public class ItemInstance
    {
        [JsonProperty]
        public string Guid { get; private set; }
        [JsonProperty]
        public long ItemID { get; private set; }
        [JsonProperty]
        public long Enchant { get; private set; }

        public ItemInstance() { }

        private ItemInstance(string guid, long itemID, long enchant = 0)
        {
            Guid = guid;
            ItemID = itemID;
            Enchant = enchant;
        }

        /// <summary>
        /// 새로운 아이템을 생성합니다.
        /// </summary>
        public static ItemInstance Create(long itemID)
        {
            return new ItemInstance(System.Guid.NewGuid().ToString(), itemID);
        }

        /// <summary>
        /// 업그레이드 된 아이템을 새로 리턴합니다.
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <returns></returns>
        public static ItemInstance Upgrade(ItemInstance itemInstance)
        {
            var output = new ItemInstance(itemInstance.Guid, itemInstance.ItemID, itemInstance.Enchant + 1);
            return output;
        }
    }
}
