
using Lofi.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class GameMapSection : MonoBehaviour
    {


        public int Width { get; set; }
        public int Height { get; set; }
        public SectionTheme Theme { get; set; }
        public Section Section { get; set; }

        private BoxCollider2D activator;
        private SpriteRenderer background;
        private GameObject[,] tiles;
        private List<Enemy> enemies;
        private LayerMask layerMask;
        public bool preventEnemySpawns = false;

        private void Awake()
        {
            activator = GetComponentInChildren<BoxCollider2D>();
            background = GetComponentInChildren<SpriteRenderer>();
            enemies = new List<Enemy>();
            layerMask = LayerMask.GetMask("Blocking");
        }

        public void Initialize(int width, int height, SectionTheme theme)
        {
            Width = width;
            Height = height;
            Theme = theme;

            activator.offset = new Vector2(width / 2f, height / 2f);
            activator.size = new Vector2(width - 1, height - 1);
            background.sprite = theme.GetBackgroundSprite();
            background.transform.localScale = new Vector3(width, height, 0);
            //background.transform.position = new Vector3(0, 0, 0);

            tiles = new GameObject[width, height];
            theme.FillSectionTiles(this);

        }

        public void AddEnemyToList(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void RemoveEnemyFromList(Enemy enemy)
        {
            enemies.Remove(enemy);
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }

        public GameObject GetTile(int x, int y)
        {
            if (x < 0 || x >= Width)
                return null;
            if (y < 0 || y >= Height)
                return null;
            return tiles[x, y];
        }

        public void SetTile(int x, int y, GameObject gameobject)
        {
            if (x < 0 || x >= Width)
                return;
            if (y < 0 || y >= Height)
                return;
            tiles[x, y] = gameobject;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void Deactivate()
        {
            activator.GetComponent<SectionActivator>().playerPresent = false;
        }

        internal void Activate()
        {
            GenerateMonsters();
        }

        public Vector3 GetTilePositionInWorldSpace(int x, int y)
        {
            return new Vector3(x + this.transform.position.x, y + this.transform.position.y);
        }

        public GameObject GetEntityAtPosition(int x, int y)
        {
            Vector2 bottom = GetTilePositionInWorldSpace(x, y) + new Vector3(0.05f, 0.05f);
            Vector2 top = bottom + new Vector2(0.9f, 0.9f);

            var entity = Physics2D.OverlapArea(bottom, top, layerMask);

            if (entity != null)
                return entity.gameObject;

            return null;
        }

        public Vector3 GetRandomOpenTile()
        {

            int attempts = 100;

            do
            {
                int x = MapFactory.RandomGenerator.Next(1, Width - 2);
                int y = MapFactory.RandomGenerator.Next(1, Height - 2);

                if (GetTile(x, y) == null)
                {
                    if (GetEntityAtPosition(x, y) != null)
                    {
                        continue;
                    }

                    return new Vector3(x, y, -1);
                }

            } while (attempts-- >= 0);

            return Vector3.zero;
        }

        internal void DropLoot(GameObject source)
        {
            LootFactory.GetLootForTheme(Theme.name, gameObject, source.transform.position);
        }

        private void GenerateMonsters()
        {
            if (enemies.Count > 0 || preventEnemySpawns)
                return;

            int maxEnemies = 3;

            for (int n = 0; n < maxEnemies; n++)
            {
                Vector3 randTile = GetRandomOpenTile();
                if (randTile != Vector3.zero)
                {
                    GameObject enemy = EnemyFactory.GetEnemyForTheme(Theme.name, this.transform.gameObject);
                    enemy.transform.position = randTile + this.transform.position;
                    AddEnemyToList(enemy.GetComponent<Enemy>());
                }
            }
        }
    }
}