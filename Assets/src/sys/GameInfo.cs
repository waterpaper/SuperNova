using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Supernova.Unity
{
    /// <summary>
    /// 게임 data 정의합니다
    /// </summary>
    public sealed class GameInfo
    {
        public static GameInfo GameParseFrom(GameInfo gameInfo, string jsonText)
        {
            var json = JToken.Parse(jsonText);

            var itemInfos = json["ItemInfos"] as JArray;
            foreach (var itemInfo in itemInfos.ToObject<IEnumerable<ItemInfo>>())
            {
                gameInfo.itemInfos.Add(itemInfo.ItemID, itemInfo);
            }

            var skillInfos = json["SkillInfos"] as JArray;
            foreach (var skillInfo in skillInfos.ToObject <IEnumerable<SkillInfo>>())
            {
                gameInfo.skillInfos.Add(skillInfo.SkillID, skillInfo);
            }

            return gameInfo;
        }

        public static GameInfo PrefabParseFrom(GameInfo gameInfo, string jsonText)
        {
            var json = JToken.Parse(jsonText);

            var monsterInfos = json["MonsterInfos"] as JArray;
            foreach (var itemInfo in monsterInfos.ToObject<IEnumerable<MonsterInfo>>())
            {
                gameInfo.monsterInfo.Add(itemInfo.MonsterID, itemInfo);
            }

            var mapInfos = json["MapInfos"] as JArray;
            foreach (var skillInfo in mapInfos.ToObject<IEnumerable<MapInfo>>())
            {
                gameInfo.mapInfos.Add(skillInfo.MapID, skillInfo);
            }

            return gameInfo;
        }

        private Dictionary<long, ItemInfo> itemInfos = new Dictionary<long, ItemInfo>();
        private Dictionary<long, SkillInfo> skillInfos = new Dictionary<long, SkillInfo>();
        private Dictionary<long, MonsterInfo> monsterInfo = new Dictionary<long, MonsterInfo>();
        private Dictionary<long, MapInfo> mapInfos = new Dictionary<long, MapInfo>();

        public IReadOnlyDictionary<long, ItemInfo> ItemInfos => itemInfos;
        public IReadOnlyDictionary<long, SkillInfo> SkilInfos => skillInfos;
        public IReadOnlyDictionary<long, MonsterInfo> MonsterInfo => monsterInfo;
        public IReadOnlyDictionary<long, MapInfo> MapInfos => mapInfos;
    }
}