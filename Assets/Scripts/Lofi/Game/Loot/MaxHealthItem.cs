using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class MaxHealthItem : MonoBehaviour
    {
        int healthAmount = 2;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().MaxHealth += healthAmount;
                other.GetComponent<Player>().Health = other.GetComponent<Player>().MaxHealth;
                healthAmount = 0;
                Destroy(gameObject);
            }
        }
    }
}