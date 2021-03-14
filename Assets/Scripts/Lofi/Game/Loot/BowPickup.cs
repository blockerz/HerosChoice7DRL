using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class BowPickup : MonoBehaviour
    {


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().ActivateBow();
                Destroy(gameObject);
            }
        }

    }
}