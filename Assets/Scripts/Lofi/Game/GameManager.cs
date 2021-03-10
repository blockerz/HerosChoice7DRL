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
            Debug.Log("Players turn");
            playersTurn = true;
            enemiesTurn = false;
        }
    }
}