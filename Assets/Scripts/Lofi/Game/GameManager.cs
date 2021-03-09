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

        public static IRandom Random { get; private set; }
        Map overWorld;
        internal bool playersTurn = true;

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
            CreateMaps();
            UpdateScene();
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

            if (!playersTurn)
                playersTurn = true;
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
    }
}