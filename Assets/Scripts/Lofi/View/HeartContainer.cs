using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class HeartContainer : MonoBehaviour
    {
        int currentHealth;
        Sprite heart;
        Sprite halfHeart;
        Sprite black;
        public Image[] images;

        void Start()
        {
            currentHealth = 0;
            heart = Resources.Load<Sprite>("Sprites/Heart");
            halfHeart = Resources.Load<Sprite>("Sprites/HeartHalf");
            black = Resources.Load<Sprite>("Sprites/Black8");
        }

        // Update is called once per frame
        void Update()
        {
            if(GameManager.instance.player != null)
            {
                int newHealth = GameManager.instance.player.GetComponent<Player>().Health;

                if (newHealth != currentHealth)
                {
                    currentHealth = newHealth;

                    for(int i = 0; i < images.Length; i++)
                    {
                        if (newHealth >= 2)
                        {
                            images[i].sprite = heart;
                            newHealth -= 2;
                        }
                        else if (newHealth == 1)
                        {
                            images[i].sprite = halfHeart;
                            newHealth -= 1;
                        }
                        else
                        {
                            images[i].sprite = black;
                        }
                    }
                }
            }
        }
    }
}