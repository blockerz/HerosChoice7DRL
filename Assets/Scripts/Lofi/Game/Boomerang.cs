using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Boomerang : MonoBehaviour
    {

        private float speed;
        public Vector3 direction;
        public int stunTurns = 2;

        void Start()
        {
            speed = 15f;
        }

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.Rotate(0, 0, 20 * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            Debug.Log("Boomerang collided with " + other.gameObject.name);

            if (other.gameObject.GetComponentInChildren<Enemy>() != null)
            {
                other.gameObject.GetComponentInChildren<Enemy>().GetStunned(stunTurns);
            }
            Destroy(gameObject);

        }
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

    }
}