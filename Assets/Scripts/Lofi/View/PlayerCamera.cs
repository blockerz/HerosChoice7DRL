using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour
{
    public int Width = 17;
    public int Height = 11;
    public GameObject player;
    public Vector3 newPosition = Vector3.zero;
    bool isMoving = false;
    public float inverseMoveTime = 1 / 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        //float x = ((Mathf.RoundToInt(player.transform.position.x) / Width) * Width) + Width / 2f;
        //float y = ((Mathf.RoundToInt(player.transform.position.y) / Height) * Height) + Height / 2f;
        //this.transform.position = new Vector3(x, y, this.transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null) return;
        }

        float x = ((Mathf.RoundToInt(player.transform.position.x) / Width) * Width) + Width / 2f;
        float y = 1.5f + ((Mathf.RoundToInt(player.transform.position.y) / Height) * Height) + Height / 2f;

        newPosition = new Vector3(x, y, this.transform.position.z);

        if (Mathf.Abs(newPosition.x - this.transform.position.x) > Width + 1 ||
            Mathf.Abs(newPosition.y - this.transform.position.y) > Height + 1)
        {
            this.transform.position = newPosition;
        }
        else if (!isMoving && this.transform.position != newPosition)
        {
            StartCoroutine(SmoothMovement(newPosition));
        }
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > 1)
        {
            Vector3 nextPostion = Vector3.MoveTowards(this.transform.position, end, inverseMoveTime * Time.deltaTime);
            this.transform.position = nextPostion;

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        this.transform.position = new Vector3(end.x, end.y, end.z);

        isMoving = false;
    }
}
