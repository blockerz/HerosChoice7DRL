using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class MiniMap : MonoBehaviour
    {
        //Texture2D mapTexture;
        Image renderer;
        bool update = true;

        void Start()
        {
            //mapTexture = Resources.Load<Texture2D>("Sprites/MiniMap");
            renderer = GetComponent<Image>();
        }

        void Update()
        {
        }

        public void NotWorkingUpdate()
        { 
            if (GameManager.instance.overWorld != null &&  update)
            {
                int width = 16 * 3;
                int height = 8 * 3;
                Color32[] pixels = new Color32[width * height];

                var newMapTex = new Texture2D(width, height);

                Vector3[,] positions =
                {
                    {new Vector3(0,10f/9,-1), new Vector3(0,5f/9,-1), new Vector3(0,0,-1)},
                    {new Vector3(5f/9,10f/9,-1), new Vector3(5f/9,5f/9,-1), new Vector3(5f/9,0,-1)},
                    {new Vector3(10f/9,10f/9,-1), new Vector3(10f/9,5f/9,-1), new Vector3(10f/9,0,-1)}
                };

                var world = GameManager.instance.overWorld;

                for (int secY = 0; secY < world.SectionHeight; secY++)
                {
                    for (int secX = 0; secX < world.SectionWidth; secX++)
                    {
                        Section section = world.GetSection(secX, secY);

                        for (int y = 0; y < 3; y++)
                        {
                            for (int x = 0; x < 3; x++)
                            {
                                if ((WangTileGenerator.bitmasks[x, y] & section.TileID) == 0)
                                {
                                    int secIndex = (secY * 3) + secX;

                                    pixels[secIndex + (y * 3) + x] = Color.white;
                                    //GameObject maskSection = new GameObject("maskSection (" + x + " , " + y + ")");
                                    //maskSection.transform.parent = mapSection.transform;
                                    //maskSection.transform.position = new Vector3(mapSection.transform.position.x + positions[x, y].x,
                                    //                                    mapSection.transform.position.y + positions[x, y].y, -1);
                                    //SpriteRenderer renderer2 = maskSection.AddComponent<SpriteRenderer>();
                                    //renderer2.sprite = maskSprite;
                                }
                            }
                        }
                    }
                }

                newMapTex.SetPixels32(pixels);
                newMapTex.Apply();

                renderer.sprite = Sprite.Create(newMapTex, new Rect(0.0f, 0.0f, newMapTex.width, newMapTex.height), Vector2.zero);
                update = false;
            }
        }
    }
}