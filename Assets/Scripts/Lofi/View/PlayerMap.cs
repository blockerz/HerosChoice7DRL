using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class PlayerMap : MonoBehaviour
    {

        void Start()
        {
        }

        void Update()
        {
            if (GameManager.instance != null && GameManager.instance.overWorld != null)
            {
                var section = GameManager.instance.ActiveSection;

                if (section != null)
                {
                    RectTransform myRectTransform = GetComponent<RectTransform>();
                    myRectTransform.anchoredPosition = new Vector3(section.mapX * 6, section.mapY * 6, transform.position.z);
                }
            }
        }
    }
}