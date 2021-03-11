using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class LootFactory : MonoBehaviour
    {
        public static void GetLootForTheme(string theme, GameObject parent, Vector3 position)
        {
            if (Random.value > .50)
            {
                var loot = Instantiate((GameObject)Resources.Load("Prefabs/Loot/HeartPickup", typeof(GameObject)), parent.transform);
                loot.transform.position = position;
            }
        }
    }
}