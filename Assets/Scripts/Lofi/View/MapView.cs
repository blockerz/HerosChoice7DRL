using Lofi.Game;
using Lofi.Maps;
using UnityEngine;


public class MapView : MonoBehaviour
{
    [HideInInspector]
    private Map map;
    [HideInInspector]
    private GameObject mapViewGO;
    [HideInInspector]
    private bool updated = false; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateMap(Map map)
    {
        this.map = map;
        updated = true;
    }

    private void CreateMap()
    {        

        if (mapViewGO != null)
            Destroy(mapViewGO);

        mapViewGO = new GameObject("Map Visualization");
        mapViewGO.transform.parent = this.transform;
        mapViewGO.transform.position = this.transform.position;
        
        for (int x, y = 0; y < map.SectionHeight; y++)
        {
            for (x = 0; x < map.SectionWidth; x++)
            {
                AddSectionSprite(map.GetSection(x, y));
            }
        }
    }

    private void AddSectionSprite(Section section)
    {
        //Texture2D tex = Texture2D.
        //Sprite sprite = Sprite.Create(tex, 
        //                    new Rect(0, 0, 16, 16), 
        //                    new Vector2(0.0f, 0.0f), 16);
        Sprite sprite =  Resources.Load("Sprites/White16", typeof(Sprite)) as Sprite;
        Sprite maskSprite =  Resources.Load("Sprites/Black6", typeof(Sprite)) as Sprite;
        GameObject mapSection = new GameObject("Section (" + section.OriginX + " , " + section.OriginY + ") : " + section.TileID);
        mapSection.transform.parent = mapViewGO.transform;
        mapSection.transform.position = new Vector3(section.OriginX, section.OriginY, 0);
        SpriteRenderer renderer = mapSection.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = GetRegionColor(section.RegionID);

        Vector3[,] positions =
        {
            {new Vector3(0,10f/16,-1), new Vector3(0,5f/16,-1), new Vector3(0,0,-1)},
            {new Vector3(5f/16,10f/16,-1), new Vector3(5f/16,5f/16,-1), new Vector3(5f/16,0,-1)},
            {new Vector3(10f/16,10f/16,-1), new Vector3(10f/16,5f/16,-1), new Vector3(10f/16,0,-1)}
        };

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if ((WangTileGenerator.bitmasks[x,y] & section.TileID) == 0)
                {
                    GameObject maskSection = new GameObject("maskSection (" + x + " , " + y + ")");
                    maskSection.transform.parent = mapSection.transform;
                    maskSection.transform.position = new Vector3(mapSection.transform.position.x + positions[x, y].x,
                                                        mapSection.transform.position.y + positions[x, y].y, -1);
                    SpriteRenderer renderer2 = maskSection.AddComponent<SpriteRenderer>();
                    renderer2.sprite = maskSprite;
                }
            }
        }
        //mapSection.AddComponent<TextMesh>().text = section.OriginX + " , " + section.OriginY;
    }

    private Color GetRegionColor(int regionID)
    {
        switch (regionID)
        {
            case 1: return Color.red;
            case 2: return Color.green;
            case 3: return Color.blue;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.yellow;
            case 7: return new Color(0.1f, 0.2f, 0.3f);
            case 8: return new Color(0.2f, 0.3f, 0.4f);
            case 9: return new Color(0.3f, 0.4f, 0.5f);
            case 10: return new Color(0.4f, 0.5f, 0.6f);
            case 11: return new Color(0.5f, 0.6f, 0.7f);
            case 12: return new Color(0.6f, 0.7f, 0.8f);
            case 13: return new Color(0.7f, 0.8f, 0.9f);
            case 14: return new Color(0.9f, 0.8f, 0.7f);
            case 15: return new Color(0.8f, 0.7f, 0.6f);
            case 16: return new Color(0.7f, 0.6f, 0.5f);
            
        }
        return Color.white;
    }



    // Update is called once per frame
    void Update()
    {
        if (updated && map != null)
        {
            CreateMap();
            updated = false;
        }
    }
}
