using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class ThrowBoomerang : MonoBehaviour, IUseItem
    {
        
        public GameObject boomerPrefab;

        public string GetName()
        {
            return "Boomerang";
        }

        public bool UseItemWithDirection(Vector3 direction)
        {
            Boomerang boom = Instantiate(boomerPrefab).GetComponent<Boomerang>();
            boom.transform.position = this.transform.position + direction + new Vector3(0.5f, 0.5f, 0);
            boom.direction = direction;
            return true;
        }

        void Start()
        {
            boomerPrefab = (GameObject)Resources.Load("prefabs/Boomerang", typeof(GameObject));
        }

        void Update()
        {

        }
    }
}