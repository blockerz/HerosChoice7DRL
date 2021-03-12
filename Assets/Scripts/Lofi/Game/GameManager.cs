using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp.Random;
using Lofi.Maps;
using System;

namespace Lofi.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        
        //private SceneManager sceneManager;
        private float turnDelay = 0.2f;

        public static IRandom Random { get; private set; }
        private Map overWorld;
        //private Map dungeon1;
        internal bool playersTurn = true;
        internal bool enemiesTurn = false;
        internal bool initializing = true;

        public GameMapSection ActiveSection;
        public GameMapSection LastSection;
        public GameObject player;
        GameObject playerPrefab;
        GameObject GameMapPrefab;
        GameMap overworldGameMap;
        GameMap[] dungeonGameMaps;
        public List<Section> dungeonSections;


        public int Turns { get; set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;

            //sceneManager = GetComponentInChildren<SceneManager>();
            GameMapPrefab = (GameObject)Resources.Load("prefabs/GameMap", typeof(GameObject));
            playerPrefab = (GameObject)Resources.Load("prefabs/Player", typeof(GameObject));
            //overworldGameMap = GetComponentInChildren<GameMap>();
            Turns = 0;
        }

        private void CreateMaps()
        {
            GameObject entrancePrefab = (GameObject)Resources.Load("prefabs/DungeonEntrance", typeof(GameObject));
            GameObject exitPrefab = (GameObject)Resources.Load("prefabs/WarpPoint", typeof(GameObject));

            overWorld = AdventurePlanner.PlanOverworld();
            overworldGameMap = Instantiate(GameMapPrefab, this.transform).GetComponent<GameMap>();
            overworldGameMap.transform.position = Vector3.zero;
            overworldGameMap.gameObject.name = "Overworld Map";
            overworldGameMap.CreateMap(overWorld);

            Vector3 mapOffset = new Vector3(overworldGameMap.Width * overworldGameMap.TileWidth, 0, 0);

            dungeonGameMaps = new GameMap[dungeonSections.Count];

            for(int d = 0; d < dungeonSections.Count; d++)
            {
                Map dungeonMap = AdventurePlanner.PlanDungeon();
                dungeonGameMaps[d] = Instantiate(GameMapPrefab, this.transform).GetComponent<GameMap>();
                dungeonGameMaps[d].gameObject.name = "Dungeon "+ d + " Map";
                dungeonGameMaps[d].CreateMap(dungeonMap, true);
                dungeonGameMaps[d].gameObject.transform.position = overworldGameMap.transform.position + mapOffset;
                mapOffset = mapOffset + new Vector3(dungeonGameMaps[d].Width * dungeonGameMaps[d].TileWidth, 0, 0);

                var entranceSection = overworldGameMap.mapSections[dungeonSections[d].OriginX, dungeonSections[d].OriginY];
                entranceSection.ClearArea(7, 4, 3, 3);
                var entranceGo = Instantiate(entrancePrefab, entranceSection.transform);
                entranceSection.AddGameObject(7, 5, entranceGo);

                var exitSection = dungeonGameMaps[d].mapSections[dungeonMap.startSection.OriginX, dungeonMap.startSection.OriginY];
                exitSection.ClearArea(1, 1, 1, 2);
                var exitGo = Instantiate(exitPrefab, exitSection.transform);
                exitSection.AddGameObject(1, 1, exitGo);

                entranceGo.GetComponentInChildren<WarpPoint>().warpPoint = new Vector2(exitGo.transform.position.x, exitGo.transform.position.y + 1);
                exitGo.GetComponent<WarpPoint>().warpPoint = new Vector2(entranceGo.transform.position.x + 1, entranceGo.transform.position.y - 1);

                exitSection = dungeonGameMaps[d].mapSections[dungeonMap.endSection.OriginX, dungeonMap.endSection.OriginY];
                exitSection.ClearArea(1, 1, 1, 2);
                exitGo = Instantiate(exitPrefab, exitSection.transform);
                exitSection.AddGameObject(1, 1, exitGo);
                exitGo.GetComponent<WarpPoint>().warpPoint = new Vector2(entranceGo.transform.position.x + 1, entranceGo.transform.position.y - 1);
            }
                


        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            initializing = true;
            CreateMaps();
            SpawnPlayer(overworldGameMap);
            initializing = false;
        }

        private void UpdateScene()
        {
            //sceneManager.UpdateScene(overWorld);
            //map = AdventurePlanner.PlanAdventure());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {

            }

            if (playersTurn || enemiesTurn || initializing)
                return;

            StartCoroutine(MoveEnemies());
        }

        public void UpdateActiveSection(GameMapSection activeSection)
        {
            if (activeSection != ActiveSection)
            {
                LastSection = ActiveSection;
                ActiveSection = activeSection;
                
                LastSection?.Deactivate();
                ActiveSection?.Activate();
            }

        }

        internal void PlayerMoved(Vector3 newPostion)
        {
            //Debug.Log("Player Moved");

        }

        internal void SpawnPlayer(GameMap gameMap)
        {
            int startRegion = overWorld.regionCriticalPath[0];
            Region region;
            overWorld.regions.TryGetValue(startRegion, out region);

            if (region != null)
            {
                gameMap.startSection = gameMap.mapSections[gameMap.map.startSection.OriginX, gameMap.map.startSection.OriginY];
                gameMap.startSection.preventEnemySpawns = true;

                Vector3 startPos = gameMap.startSection.GetRandomOpenTile() + gameMap.startSection.transform.position;
                GameManager.instance.player = Instantiate(playerPrefab);
                GameManager.instance.player.name = "Player";
                GameManager.instance.player.transform.position = startPos;

                gameMap.UpdateSectionDifficultiesBasedOnStart();

                //startSection.gameObject.SetActive(true);
            }
        }

        IEnumerator MoveEnemies()
        {
            enemiesTurn = true;
            yield return new WaitForSeconds(turnDelay);

            List<Enemy> activeEnemies = ActiveSection.GetEnemies();

            if (activeEnemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].EnemyTurn();
                //Debug.Log("Waiting for " + activeEnemies[i].moveTime);
                yield return new WaitForSeconds(activeEnemies[i].moveTime);
            }

            FixOverlappingEntities();

            //Debug.Log("Players turn");
            playersTurn = true;
            enemiesTurn = false;
        }

        private void FixOverlappingEntities()
        {
            List<Enemy> activeEnemies = ActiveSection.GetEnemies();
            List<GameObject> potentialCollitions = new List<GameObject>();
            RaycastHit2D hit;

            foreach (var enemy in activeEnemies)
            {
                potentialCollitions.Clear();
                potentialCollitions.Add(player);

                foreach (var otherenemy in activeEnemies)
                {
                    if(otherenemy != enemy)
                        potentialCollitions.Add(otherenemy.gameObject);
                }

                foreach(var col in potentialCollitions)
                {
                    if(col.transform.position == enemy.transform.position)
                    {
                        Debug.LogError("OVERLAP DETECTED: " + enemy.name + " overlapped " + col.name);
                        //foreach(var dir in MovingObject.directions)
                        //{
                        //    if (enemy.Move((int)dir.x, (int)dir.y, out hit))
                        //    {
                        //        Debug.LogError("Moved entity due to overlap: " + enemy.name + " overlapped " + col.name);
                        //        break;
                        //    }
                        //}
                    }
                }
            }
        }
    }
}