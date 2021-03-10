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
        
        private SceneManager sceneManager;
        private List<Enemy> enemies;
        private float turnDelay = 0.3f;

        public static IRandom Random { get; private set; }
        Map overWorld;
        internal bool playersTurn = true;
        internal bool enemiesTurn = false;
        internal bool initializing = true;

        public GameMapSection ActiveSection;
        public GameMapSection LastSection;


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            sceneManager = GetComponentInChildren<SceneManager>();

            enemies = new List<Enemy>();


        }

        private void CreateMaps()
        {
            overWorld = AdventurePlanner.PlanOverworld();            
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
            UpdateScene();
            initializing = false;
        }

        private void UpdateScene()
        {
            sceneManager.UpdateScene(overWorld);
            //map = AdventurePlanner.PlanAdventure());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //overWorld = AdventurePlanner.PlanOverworld();
                //UpdateScene();
                //Debug.Log("SDASDAD");
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

        public void AddEnemyToList(Enemy enemy)
        {            
            enemies.Add(enemy);
        }

        IEnumerator MoveEnemies()
        {
            enemiesTurn = true;
            yield return new WaitForSeconds(turnDelay);

            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].EnemyTurn();
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
            playersTurn = true;
            enemiesTurn = false;
        }
    }
}