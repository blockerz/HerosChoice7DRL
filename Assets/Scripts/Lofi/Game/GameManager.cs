using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp.Random;
using Lofi.Maps;

namespace Lofi.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        
        private SceneManager sceneManager;

        public static IRandom Random { get; private set; }
        Map overWorld;

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
                overWorld = AdventurePlanner.PlanOverworld();
                UpdateScene();
            }
        }
    }
}