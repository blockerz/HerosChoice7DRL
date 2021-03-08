using Lofi.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapSection : MonoBehaviour
{


    public int Width { get; set; }
    public int Height { get; set; }
    public SectionTheme Theme { get; set; }
    public Section Section { get; set; }

    private BoxCollider2D activator;
    private SpriteRenderer background;
    private GameObject[,] tiles;
    

    private void Awake()
    {
        activator = GetComponentInChildren<BoxCollider2D>();
        background = GetComponentInChildren<SpriteRenderer>();       
    }

    public void Initialize(int width, int height, SectionTheme theme)
    {
        Width = width;
        Height = height;
        Theme = theme;

        activator.offset = new Vector2(width/2f, height/2f);
        activator.size = new Vector2(width, height);
        background.sprite = theme.GetBackgroundSprite();
        background.transform.localScale = new Vector3(width, height, 0);

        tiles = new GameObject[width, height];
        theme.FillSectionTiles(this);

    }

    public GameObject GetTile(int x, int y)
    {
        if (x < 0 || x >= Width)
            return null;
        if (y < 0 || y >= Height)
            return null;
        return tiles[x,y];
    }

    public void SetTile(int x, int y, GameObject gameobject)
    {
        if (x < 0 || x >= Width)
            return;
        if (y < 0 || y >= Height)
            return;
        tiles[x, y] = gameobject;
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
