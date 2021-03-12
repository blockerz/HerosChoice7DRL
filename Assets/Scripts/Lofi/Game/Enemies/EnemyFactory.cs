
using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class EnemyFactory : MonoBehaviour
    {

        public static GameObject GetEnemyForTheme(SectionTheme theme, int difficulty, GameObject parent)
        {
            string[] enemySprites = { "Sprites/Enemies/CatSpirit",
                        "Sprites/Enemies/DarkWizard",
                        "Sprites/Enemies/Frogman",
                        "Sprites/Enemies/Guardian",
                        "Sprites/Enemies/GuardianSeer",
                        "Sprites/Enemies/Werebat"};

            int index = MapFactory.RandomGenerator.Next(0, enemySprites.Length - 1);

            return SpawnEnemy(parent, enemySprites[index], new RangedAttackBehavior());
        }

        private static GameObject SpawnEnemy(GameObject parent, string spriteName, IEnemyBehavior behavior = null)
        {
            var go = Instantiate((GameObject)Resources.Load("Prefabs/Enemies/Enemy", typeof(GameObject)), parent.transform);
            var enemy = go.GetComponent<Enemy>();
            enemy.renderer.sprite = Resources.Load<Sprite>(spriteName);

            if(behavior != null)
                enemy.enemyBehavior = behavior;
            return go;
        }
    }
}