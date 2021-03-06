using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class ShootProjectile : MonoBehaviour
    {
        public GameObject projectilePrefab;

        public void Shoot(Vector3 direction, GameObject bullet = null)
        {
            Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
            projectile.transform.position = this.transform.position + direction + new Vector3(0.5f, 0.5f, 0);
            projectile.direction = direction;
        }


        // Start is called before the first frame update
        void Start()
        {
            projectilePrefab = (GameObject)Resources.Load("prefabs/Projectile", typeof(GameObject));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}