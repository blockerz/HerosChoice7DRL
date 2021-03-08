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

    List<SectionTheme> themes;
    GameMapSection[,] mapSections;
    GameObject mapSectionPrefab; 

    private void Awake()
    {
        mapSectionPrefab = (GameObject)Resources.Load("prefabs/GameMapSection", typeof(GameObject));
        TileWidth = 17;
        TileHeight = 11;

        CreateThemes();

    }

    public void CreateThemes()
    {
        themes = new List<SectionTheme>();
        SectionTheme theme = new DesertTheme();
        theme.Initialize("Desert", "DesertDefault");
        themes.Add(theme);

        theme = new ForestTheme();
        theme.Initialize("Forest", "ForestDefault");
        themes.Add(theme);
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
                mapSection.Initialize(TileWidth, TileHeight, GetThemeForRegion(mapSection.Section.RegionID));
                mapSection.name = "Section: " + x + ", " + y;
                mapSection.transform.position = new Vector3(x * TileWidth, y * TileHeight, 0);
            }
        }
    }

    public SectionTheme GetThemeForRegion(int region)
    {
        if (region <= 3)
            return themes[0];
        return themes[1];
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
