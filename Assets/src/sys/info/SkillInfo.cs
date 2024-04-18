using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace Supernova.Unity
{
    /// <summary>
    /// 스킬 정보 정의입니다.
    /// </summary>
    public class SkillInfo
    {
        [JsonProperty]
        public long SkillID { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
        public SkillType SkillType { get; set; }
        [JsonProperty]
        public double Cooltime { get; set; }
    }

    public enum SkillType
    {
        Active,
    }

    public interface ISkill
    {
        IEnumerator Invoke();
    }
}
