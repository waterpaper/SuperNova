using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// �� ���� �����Դϴ�.
/// </summary>
public class MapInfo
{
    [JsonProperty]
    public long MapID { get; private set; }
    [JsonProperty]
    public string Path { get; private set; }
    [JsonProperty]
    public List<long> Monsters { get; private set; }
    [JsonProperty]
    public long Boss { get; private set; }
}