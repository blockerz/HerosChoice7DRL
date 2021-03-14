using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class ShootArrow : MonoBehaviour, IUseItem
    {
        public GameObject arrowPrefab;

        public string GetName()
        {
            return "Arrow";
        }

        public bool UseItemWithDirection(Vector3 direction)
        {
            var player = GetComponent<Player>();

            if (player.Gems <= 0)
                return false;

            player.Gems--;

            BowAndArrow arrow = Instantiate(arrowPrefab).GetComponent<BowAndArrow>();
            arrow.transform.position = this.transform.position + direction + new Vector3(0.5f, 0.5f, 0);
            arrow.transform.rotation = Quaternion.Euler(Vector3.forward * GetDegrees(direction));
            arrow.direction = direction;
            return true;
        }


        private float GetDegrees(Vector3 direction)
        {
            Vector3 dir = -direction;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            return angle+90;
        }

        void Start()
        {
            arrowPrefab = (GameObject)Resources.Load("prefabs/Arrow", typeof(GameObject));
        }

        void Update()
        {

        }
    }

}
