using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Projectile : MonoBehaviour
    {
        public SpriteRenderer renderer;

        public float speed;
        public Vector3 direction;
        //LayerMask mask;
        public int damage = 1;

        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            //mask = LayerMask.GetMask("Blocking");
            speed = 15f;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            Debug.Log("Projectile collided with " + other.gameObject.name);
            if (other.gameObject.GetComponentInChildren<Player>() != null)
            {
                other.gameObject.GetComponentInChildren<Player>().ReceiveIncomingDamage(this.gameObject, damage);
            }
            else if (other.gameObject.GetComponentInChildren<Enemy>() != null)
            {
                other.gameObject.GetComponentInChildren<Enemy>().ReceiveIncomingDamage(this.gameObject, damage);
            }
            Destroy(gameObject);           

        }

        //private void OnColliderEnter2D(Collider2D other)
        //{

        //    Debug.Log("Projectile collided with " + other.gameObject.name);
        //    if (other.gameObject.GetComponentInChildren<Player>() != null)
        //    {
        //        other.gameObject.GetComponentInChildren<Player>().ReceiveIncomingDamage(this.gameObject, damage);
        //    }
        //    else if (other.gameObject.GetComponentInChildren<Enemy>() != null)
        //    {
        //        other.gameObject.GetComponentInChildren<Enemy>().ReceiveIncomingDamage(this.gameObject, damage);
        //    }
        //    Destroy(gameObject);

        //}

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}