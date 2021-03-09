using Lofi.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Player : MovingObject
    {
        private SpriteRenderer renderer;
        public Sprite[] idleSprites;

        private void Awake()
        {
            idleSprites = Resources.LoadAll<Sprite>("Sprites/Player/Player");
            renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = idleSprites[1]; 
        }
        // Update is called once per frame
        void Update()
        {
            if (!GameManager.instance.playersTurn) return;

            if (isMoving) return;

            int horizontal = 0;     
            int vertical = 0;

            if (Input.GetKey(KeyCode.W))
            {
                vertical = 1;
                renderer.sprite = idleSprites[0];
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
                renderer.sprite = idleSprites[3];
            }
            else if (Input.GetKey(KeyCode.S))
            {
                vertical = -1;
                renderer.sprite = idleSprites[2];
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
                renderer.sprite = idleSprites[1];
            }

            if (horizontal != 0 || vertical != 0)
            {
                Debug.Log("D:" + horizontal + ", " + vertical);
                AttemptMove(horizontal, vertical);
            }

            GameManager.instance.playersTurn = false;
        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);
        }

        protected override IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;

            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            Vector3 newPostion; 

            while (sqrRemainingDistance > 1)
            {
                newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPostion);

                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                yield return null;
            }

            newPostion = new Vector3(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), end.z);
            rb2D.MovePosition(newPostion);

            isMoving = false;
            GameManager.instance.PlayerMoved(newPostion);
        }
    }
}