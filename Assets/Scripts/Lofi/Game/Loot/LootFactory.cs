using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class LootFactory : MonoBehaviour
    {
        static List<string> Treasures;

        static LootFactory()
        {
            Treasures = new List<string>();

            Treasures.Add("Prefabs/Loot/BowPickup");
            Treasures.Add("Prefabs/Loot/BoomerangPickup");
            Treasures.Add("Prefabs/Loot/BombPickup");
            Treasures.Add("Prefabs/Loot/SwordPickup");
        }

        public static void GetTreasureForDungeon(GameObject parent, Vector3 position)
        {
            string prefab = "Prefabs/Loot/GemPickup";// other loot is priority but give gems if all thats left

            if (Treasures.Count > 0)
            {
                int index = MapFactory.RandomGenerator.Next(0, Treasures.Count - 1);
                prefab = Treasures[index];
                Treasures.RemoveAt(index);
            }

            var loot = Instantiate((GameObject)Resources.Load(prefab, typeof(GameObject)), parent.transform);
            loot.transform.position = position;

            if (prefab.Equals("Prefabs/Loot/GemPickup"))
            {
                var pu = loot.GetComponent<GemPickup>();
                if (pu != null)
                    pu.gemAmount = 10;
            }
        }

        public static void GetLootForTheme(string theme, GameObject parent, Vector3 position)
        {
            Enemy enemy = parent.GetComponentInChildren<Enemy>();

            if (enemy != null)
            {
                if (enemy.name.ToUpper().Contains("BOSS"))
                {
                    var loot = Instantiate((GameObject)Resources.Load("Prefabs/Loot/HeartContainer", typeof(GameObject)), parent.transform);
                    loot.transform.position = position;
                    return;
                }
            }

            float roll = Random.value;

            if (roll > .50)
            {
                var loot = Instantiate((GameObject)Resources.Load("Prefabs/Loot/HeartPickup", typeof(GameObject)), parent.transform);
                loot.transform.position = position;
            }
            else if (roll > .40)
            {
                var loot = Instantiate((GameObject)Resources.Load("Prefabs/Loot/GemPickup", typeof(GameObject)), parent.transform);
                loot.transform.position = position;
            }
        }
    }
}