using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class BowAndArrow : MonoBehaviour
    {

        private float speed;
        public Vector3 direction;
        public int damage = 1;

        void Start()
        {
            speed = 15f;
        }

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        public void RotateTowards(Vector2 target)
        {
            var offset = 90f;
            Vector2 direction = target - (Vector2)transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            Debug.Log("Arrow collided with " + other.gameObject.name);

            if (other.gameObject.GetComponentInChildren<Enemy>() != null)
            {
                other.gameObject.GetComponentInChildren<Enemy>().ReceiveIncomingDamage(this.gameObject, damage);
            }
            Destroy(gameObject);

        }
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

    }
}