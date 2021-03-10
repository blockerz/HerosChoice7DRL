
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class EnemyFactory : MonoBehaviour
    {

        public static GameObject GetEnemyForTheme(string theme, GameObject parent)
        {
            var go = Instantiate((GameObject)Resources.Load("Prefabs/Enemies/Enemy", typeof(GameObject)), parent.transform);
            var enemy = go.GetComponent<Enemy>();
            enemy.renderer.sprite = Resources.Load<Sprite>("Sprites/Enemies/Werebat");
            //enemy.enemyBehavior = new DoNothingBehavior();
            return go;
        }
    }
}