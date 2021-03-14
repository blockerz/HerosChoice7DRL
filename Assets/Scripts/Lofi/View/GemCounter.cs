using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class GemCounter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.instance != null && GameManager.instance.player != null)
            {
                string newCount = GameManager.instance.player.GetComponent<Player>().Gems + "";
                GetComponent<Text>().text = newCount;
            }
        }
    }
}