using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// 전체 맵을 관리하는 클래스입니다.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        [SerializeField]
        private float mapSize = 50;
        [SerializeField]
        private List<GameObject> activeMap;

        private int maxMapActiveCount = 3;
        private int changeTileCount = 10;
        private float startMapZ;

        private Dictionary<int, GameObject> mapCache = new();
        private Dictionary<int, TileInfo> mapInfo = new();


        private void Awake()
        {
            startMapZ = -mapSize;
        }

        private void OnDestroy()
        {
            this.activeMap.Clear();

            foreach (var key in mapCache)
            {
                key.Value.Release();
            }
        }

        /// <summary>
        /// 초기 생성된 맵을 풀링하며 현 스테이지에 맞는 맵과 위치를 조정합니다.
        /// </summary>
        public async UniTask CreateMap()
        {
            foreach (var mapInfo in Root.GameInfo.MapInfos.Values)
            {
                GameObject newGameObject = new GameObject(mapInfo.Path);
                newGameObject.transform.parent = this.transform;

                string str = $"{BattleConfig.mapPath}{mapInfo.Path}.prefab";

                await Res.InstantiateAssetAsCoroutineThen<Transform>(str, newGameObject.transform, (prefab) =>
                {
                    GameObject mapObject = prefab.gameObject;

                    TileInfo tileInfo = prefab.GetComponent<TileInfo>();
                    prefab.gameObject.SetActive(false);

                    mapCache.Add((int)tileInfo.MapIndex, (prefab.gameObject));
                    this.mapInfo.Add((int)tileInfo.MapIndex, tileInfo);
                });
            }
        }

        public void StartSetting()
        {
            for (int i = -1; i < maxMapActiveCount - 1; i++)
                SpawnMap(Root.State.General.Kill.Value + changeTileCount * i);

            Vector3 settingMovePosition = Vector3.back * (Root.State.General.Kill.Value % changeTileCount * mapSize / changeTileCount);

            foreach (GameObject map in activeMap)
                map.transform.Translate(settingMovePosition);

            startMapZ += settingMovePosition.z;

            if (activeMap.Count > 0 && 0 > activeMap[0].transform.position.z + (this.mapSize * 1.5f))
            {
                activeMap[0].SetActive(false);
                activeMap.RemoveAt(0);
            }
        }

        private void SpawnMap(long stage)
        {
            int remainder = (int)Math.Abs(stage % (BattleConfig.mapCount * BattleConfig.maxTileThemeEnemyCount)) / 10;
            TileMapEnum nowMapIndex = (TileMapEnum)remainder + 1;

            if (mapCache.ContainsKey((int)nowMapIndex) == false) return;

            var map = mapCache[(int)nowMapIndex];

            if (map.activeSelf == true) return;

            activeMap.Add(map);
            map.transform.position = Vector3.forward * startMapZ;

            startMapZ += mapSize;
            map.SetActive(true);
        }

        /// <summary>
        /// 플레이어 이동시 대신 맵을 움직이며 무한과 같은 느낌의 효과를 줍니다.
        /// </summary>
        public void MapMove(float speed)
        {
            MoveLogic(-speed);
        }

        private void MoveLogic(float speed)
        {
            startMapZ += speed;

            foreach (GameObject map in activeMap)
            {
                map.transform.Translate(new Vector3(0, 0, -speed));
            }

            if (activeMap.Count > 0 && 0 > activeMap[0].transform.position.z + (this.mapSize * 1.5f))
            {
                activeMap[0].SetActive(false);
                activeMap.RemoveAt(0);
            }

            if (activeMap.Count < maxMapActiveCount)
                SpawnMap(Root.State.General.Kill.Value + changeTileCount);
        }


        public TileInfo PostionMap(Vector3 pos)
        {
            for (int i = 0; i < activeMap.Count; i++)
            {
                if (pos.z > activeMap[i].transform.position.z && pos.z < activeMap[i].transform.position.z + mapSize)
                {
                    return activeMap[i].GetComponent<TileInfo>();
                }
            }

            return null;
        }
    }
}

