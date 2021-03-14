using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class DropBomb : MonoBehaviour, IUseItem
    {

        public GameObject bombPrefab;

        public string GetName()
        {
            return "Bomb";
        }

        public bool UseItemWithDirection(Vector3 direction)
        {
            var player = GetComponent<Player>();

            if (player.Gems <= 0)
                return false;

            player.Gems--;

            Bomb boom = Instantiate(bombPrefab).GetComponent<Bomb>();
            boom.transform.position = this.transform.position + direction; // + new Vector3(0.5f, 0.5f, 0);
            boom.direction = direction;
            return true;
        }

        void Start()
        {
            bombPrefab = (GameObject)Resources.Load("prefabs/Bomb", typeof(GameObject));
        }

        void Update()
        {

        }
    }
}