using Newtonsoft.Json;

/// <summary>
/// ���� ���� �����Դϴ�.
/// </summary>
public class MonsterInfo
{
    [JsonProperty]
    public long MonsterID { get; private set; }
    [JsonProperty]
    public string Path { get; private set; }
}