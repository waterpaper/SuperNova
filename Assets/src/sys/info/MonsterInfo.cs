using Newtonsoft.Json;

/// <summary>
/// 몬스터 정보 정의입니다.
/// </summary>
public class MonsterInfo
{
    [JsonProperty]
    public long MonsterID { get; private set; }
    [JsonProperty]
    public string Path { get; private set; }
}