using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombParticle : MonoBehaviour
{
    bool destroy = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!destroy)
        {
            Destroy(gameObject, 0.4f);
            destroy = true;
        }
    }
}
