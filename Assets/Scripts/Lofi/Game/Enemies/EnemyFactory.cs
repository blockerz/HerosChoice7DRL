
using Lofi.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class EnemyFactory : MonoBehaviour
    {

        public static List<EnemyTemplate> AllEnemies;
        public static List<EnemyTemplate> BossEnemies;

        static EnemyFactory()
        {
            AllEnemies = new List<EnemyTemplate>();
            BossEnemies = new List<EnemyTemplate>();

            // ALL
            AddEnemy("Werebat", "Sprites/Enemies/Werebat", 2, 1, "ALL");

            //AUTUMNFOREST
            AddEnemy("Druid", "Sprites/Enemies/Druid", 2, 1, "AutumnForest");
            AddEnemy("Motheran", "Sprites/Enemies/Motheran", 2, 1, "AutumnForest");

            //BEACH
            AddEnemy("TwoLegs", "Sprites/Enemies/TwoLegs", 2, 1, "Beach");
            AddEnemy("Frogman", "Sprites/Enemies/Frogman", 2, 1, "Beach");

            //BOSS
            AddBossEnemy("Boss1", "Sprites/Enemies/Boss1", 5, 1, "Boss");
            AddBossEnemy("Boss2", "Sprites/Enemies/Boss2", 5, 1, "Boss");
            AddBossEnemy("Boss3", "Sprites/Enemies/Boss3", 5, 1, "Boss");
            AddBossEnemy("Boss4", "Sprites/Enemies/Boss4", 5, 1, "Boss");
            AddBossEnemy("Boss5", "Sprites/Enemies/Boss5", 5, 1, "Boss");
            AddBossEnemy("Boss6", "Sprites/Enemies/Boss6", 5, 1, "Boss");

            //DESERT
            AddEnemy("DarkWizard", "Sprites/Enemies/DarkWizard", 2, 1, "Desert", "RangedAttackBehavior");

            //DUNGEON
            AddEnemy("PsyClone", "Sprites/Enemies/PsyClone", 2, 1, "Dungeon");
            AddEnemy("PsyGem", "Sprites/Enemies/PsyGem", 2, 1, "Dungeon");
            AddEnemy("Roborg", "Sprites/Enemies/Roborg", 2, 1, "Dungeon");
            AddEnemy("Pillar1", "Sprites/Enemies/Pillar1", 2, 1, "Dungeon", "RangedStationaryBehavior");
            AddEnemy("Pillar2", "Sprites/Enemies/Pillar2", 2, 1, "Dungeon", "RangedStationaryBehavior");
            AddEnemy("Pillar3", "Sprites/Enemies/Pillar3", 2, 1, "Dungeon", "RangedStationaryBehavior");
            AddEnemy("Pillar4", "Sprites/Enemies/Pillar4", 2, 1, "Dungeon", "RangedStationaryBehavior");


            // FOREST
            AddEnemy("Guardian", "Sprites/Enemies/Guardian", 2, 1, "Forest");
            AddEnemy("GuardianSeer", "Sprites/Enemies/GuardianSeer", 2, 1, "Forest", "RangedAttackBehavior");

            //GRAVEYARD
            AddEnemy("Ghast", "Sprites/Enemies/Ghast", 2, 1, "Graveyard");
            AddEnemy("Wraith", "Sprites/Enemies/Wraith", 2, 1, "Graveyard");

            //ROCKY
            AddEnemy("Dogman", "Sprites/Enemies/Dogman", 2, 1, "Rocky");
            AddEnemy("Spider", "Sprites/Enemies/Spider", 2, 1, "Rocky");

            //SNOW
            AddEnemy("CatSpirit", "Sprites/Enemies/CatSpirit", 2, 1, "Snow");
            AddEnemy("Dogman", "Sprites/Enemies/Dogman", 2, 1, "Snow");

            //SWAMP
            AddEnemy("SlimeBear", "Sprites/Enemies/SlimeBear", 2, 1, "Swamp");
            AddEnemy("Birdman", "Sprites/Enemies/Birdman", 2, 1, "Swamp");

        }

        private static void AddEnemy(string name, string sprite, int health, int damage, string habitat = "ALL", string behavior = "")
        {
            EnemyTemplate template = new EnemyTemplate(
                name,
                sprite,
                health,
                damage,
                habitat,
                behavior
                );
            AllEnemies.Add(template);
        }

        private static void AddBossEnemy(string name, string sprite, int health, int damage, string habitat = "ALL", string behavior = "")
        {
            EnemyTemplate template = new EnemyTemplate(
                name,
                sprite,
                health,
                damage,
                habitat,
                behavior
                );
            BossEnemies.Add(template);
        }

        public static void GetEnemyForTheme(GameMapSection section)
        {

            List < EnemyTemplate > selectedEnemies = AllEnemies.FindAll(enemy => 
                (enemy.Habitat.ToUpper().Equals(section.Theme.ThemeName.ToUpper()) 
                || enemy.Habitat.ToUpper().Equals("ALL")));

            int maxEnemies = 2 + (section.difficulty / 3);
            int enemyCount = MapFactory.RandomGenerator.Next(2, maxEnemies);

            for (int n = 0; n < enemyCount; n++)
            {
                Vector3 randTile = section.GetRandomOpenTile();
                if (randTile != Vector3.zero)
                {
                    int index = MapFactory.RandomGenerator.Next(0, selectedEnemies.Count - 1);
                    var enemy = SpawnEnemy(section.gameObject, selectedEnemies[index]);
                    enemy.transform.position = randTile + section.transform.position;
                    section.AddEnemyToList(enemy.GetComponent<Enemy>());
                }
            }
        }

        public static void GetBossEnemy(GameMapSection section)
        {

            Vector3 randTile = section.GetRandomOpenTile();
            if (randTile != Vector3.zero)
            {
                int index = MapFactory.RandomGenerator.Next(0, BossEnemies.Count - 1);
                var enemy = SpawnEnemy(section.gameObject, BossEnemies[index]);
                enemy.transform.position = randTile + section.transform.position;
                section.AddEnemyToList(enemy.GetComponent<Enemy>());
                BossEnemies.RemoveAt(index);
            }
            
        }

        private static GameObject SpawnEnemy(GameObject parent, EnemyTemplate template)
        {
            var go = Instantiate((GameObject)Resources.Load("Prefabs/Enemies/Enemy", typeof(GameObject)), parent.transform);
            var enemy = go.GetComponent<Enemy>();
            enemy.renderer.sprite = Resources.Load<Sprite>(template.Sprite);
            enemy.gameObject.name = template.Name;
            enemy.Health = template.Health;
            enemy.Damage = template.Damage;

            if (template.Behavior != null && template.Behavior.Length > 0)
            {
                switch(template.Behavior)
                {
                    case "RangedStationaryBehavior":
                    {
                        enemy.enemyBehavior = new RangedStationaryBehavior();
                    }
                    break;
                    case "RangedAttackBehavior":
                    {
                        enemy.enemyBehavior = new RangedAttackBehavior();
                    }
                    break;
                }
                if (enemy.enemyBehavior != null)
                    enemy.enemyBehavior.AddAbilites(enemy);
            }

            return go;
        }
    }
}