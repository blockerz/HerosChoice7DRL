using Lofi.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionActivator : MonoBehaviour
{
    BoxCollider2D activator;
    public bool playerPresent = false;

    // Start is called before the first frame update
    void Start()
    {
        activator = GetComponent<BoxCollider2D>();    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerPresent && other.CompareTag("Player"))
        {
            Debug.Log("Player entered: " + this.transform.parent.name);
            playerPresent = true;
            GameManager.instance.UpdateActiveSection(GetComponentInParent<GameMapSection>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (playerPresent && other.CompareTag("Player"))
        //{
        //    Debug.Log("Player exited: " + this.transform.parent.name);
        //    playerPresent = false;
        //}
    }
}
