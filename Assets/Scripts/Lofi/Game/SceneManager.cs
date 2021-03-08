using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class SceneManager : MonoBehaviour
{
    Map map;
    MapView mapView;
    GameMap gameMap;

    private void Awake()
    {
        mapView = GetComponentInChildren<MapView>();
        gameMap = GetComponentInChildren<GameMap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateScene(Map map)
    {
        mapView.UpdateMap(map);
        mapView.gameObject.SetActive(false);

        gameMap.CreateMap(map);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

