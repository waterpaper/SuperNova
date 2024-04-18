using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// 환경 설정 데이터를 관리합니다.
    /// </summary>
    public static class GameSettings
    {
        public static bool HighQuality => PlayerPrefs.GetInt("setting.hq", 0) == 1;
        public static bool Bgm => PlayerPrefs.GetInt("setting.bgm", 0) == 1;
        public static bool Sfx => PlayerPrefs.GetInt("setting.sfx", 0) == 1;
    }
}