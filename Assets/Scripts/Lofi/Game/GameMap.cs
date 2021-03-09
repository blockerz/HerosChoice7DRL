using Lofi.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public int Width { get; set; }
    public int Height { get; set; }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }

    GameMapSection[,] mapSections;
    GameObject mapSectionPrefab;
    GameMapThemes themes;

    private void Awake()
    {
        mapSectionPrefab = (GameObject)Resources.Load("prefabs/GameMapSection", typeof(GameObject));
        TileWidth = 17;
        TileHeight = 11;

        themes = new GameMapThemes();
        themes.CreateThemes();

    }

    public void CreateMap(Map map)
    {
        Width = map.SectionWidth;
        Height = map.SectionHeight;
        mapSections = new GameMapSection[Width, Height];
        


        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                GameMapSection mapSection = mapSections[x, y] = Instantiate(mapSectionPrefab, this.transform).GetComponent<GameMapSection>();
                mapSection.Section = map.GetSection(x, y);
                mapSection.Initialize(TileWidth, TileHeight, themes.GetThemeForRegion(mapSection.Section));
                mapSection.name = "Section: " + x + ", " + y;
                mapSection.transform.position = new Vector3(x * TileWidth, y * TileHeight, 0);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
