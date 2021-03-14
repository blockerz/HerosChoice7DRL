using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class GemPickup : MonoBehaviour
    {
        public int gemAmount = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().Gems += gemAmount;
                gemAmount = 0;
                Destroy(gameObject);
            }
        }

    }
}