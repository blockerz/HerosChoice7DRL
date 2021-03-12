using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class SceneManager : MonoBehaviour
    {
        Map map;
        MapView mapView;


        private void Awake()
        {
            mapView = GetComponentInChildren<MapView>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void UpdateScene(Map map)
        {
            mapView.UpdateMap(map);
            mapView.gameObject.SetActive(false);

            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}