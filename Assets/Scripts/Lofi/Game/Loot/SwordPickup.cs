using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class SwordPickup : MonoBehaviour
    {
        int damageAmount = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().Damage += damageAmount;
                damageAmount = 0;
                Destroy(gameObject);
            }
        }

        
    }
}