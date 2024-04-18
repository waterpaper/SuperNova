using System;
using System.Collections;
using System.Collections.Generic;

namespace Supernova.Unity
{
    /// <summary>
    /// 필요 고정 데이터를 미리 정의해둔 클래스입니다.
    /// </summary>
    public class BattleConfig
    {
        public readonly static string INGAME = "Assets/res/prefab/game/manager/Ingame.prefab";
        public readonly static string TILEMANAGER = "Assets/res/prefab/game/manager/Environment.prefab";

        public readonly static string mapPath = "Assets/res/prefab/game/map/";
        public readonly static string playerPath = "Assets/res/prefab/game/character/Dragoon.prefab";
        public readonly static string enemyPath = "Assets/res/prefab/game/enemys/";
        public readonly static string bossPath = "Assets/res/prefab/game/enemys/";

        public readonly static List<string> enemyPrefabName = new();
        public readonly static List<string> bossPrefabName = new();

        public readonly static int maxEnemyCount = 3;
        public readonly static int maxTileThemeEnemyCount = 10;
        public readonly static int mapCount = 5;
        public readonly static int mapIDStart = 10001;
    }
}
