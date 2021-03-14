using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class ActiveItemUI : MonoBehaviour
    {
        Sprite bomb;
        Sprite arrow;
        Sprite boomerang;
        Image renderer;

        // Start is called before the first frame update
        void Start()
        {
            bomb = Resources.Load<Sprite>("Sprites/Player/Bomb");
            arrow = Resources.Load<Sprite>("Sprites/Player/Bow");
            boomerang = Resources.Load<Sprite>("Sprites/Player/Boomerang");
            renderer = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.instance != null && GameManager.instance.player != null)
            {
                Player player = GameManager.instance.player.GetComponent<Player>();

                if (player.items.Count == 0)
                {
                    renderer.sprite = null;
                }
                else
                {
                    string itemname = player.items[player.activeItem].GetName();

                    switch(itemname)
                    {
                        case "Bomb":
                            {
                                renderer.sprite = bomb;
                                break;
                            }
                        case "Arrow":
                            {
                                renderer.sprite = arrow;
                                break;
                            }
                        case "Boomerang":
                            {
                                renderer.sprite = boomerang;
                                break;
                            }
                    }
                }
            }
        }
    }
}