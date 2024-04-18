using System.Collections.Generic;
using UnityEngine;

namespace Supernova.Unity
{
    public enum TileMapEnum
    {
        NULL,
        TileGrassLand,
        TileDesert,
        TileForest,
        TileGrassLandB,
        TileIce,
        TILELAST
    }

    public class TileInfo : MonoBehaviour
    {
        [SerializeField]
        private int mapID;
        [SerializeField]
        private float mapLength;
        [SerializeField]
        private TileMapEnum mapIndex;

        private int maxMonster = BattleConfig.maxTileThemeEnemyCount;
        private GameObject spawnObj;
        private List<GameObject> spawnPoint;

        public float MapID { get { return mapID; } private set { } }
        public float MapLength { get { return mapLength; } private set { } }
        public TileMapEnum MapIndex { get { return mapIndex; } private set { } }
        public List<GameObject> SpawnPoint { get { return spawnPoint; } private set { } }


        private void Awake()
        {
            spawnObj = new GameObject();
            spawnObj.transform.parent = this.transform;

            spawnPoint = new List<GameObject>();

            float po = mapLength / maxMonster;
            float poZ = po - 1;

            for (int i = 0; i < maxMonster; i++)
            {
                GameObject newGameObject = new GameObject(name);
                newGameObject.transform.parent = spawnObj.transform;
                newGameObject.transform.position = new Vector3(0, 0, poZ);
                spawnPoint.Add(newGameObject);
                poZ += po;
            }
        }
    }
}
