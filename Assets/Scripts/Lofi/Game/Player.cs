using Lofi.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Player : MovingObject
    {
        private SpriteRenderer renderer;
        private Animator animator;
        public Sprite[] idleSprites;

        private void Awake()
        {
            idleSprites = Resources.LoadAll<Sprite>("Sprites/Player/Player");
            renderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            //renderer.sprite = idleSprites[1]; 
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
                //renderer.sprite = idleSprites[0];
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
                //renderer.sprite = idleSprites[3];
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.X))
            {
                vertical = -1;
                //renderer.sprite = idleSprites[2];
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
                //renderer.sprite = idleSprites[1];
            }
            if (Input.GetKey(KeyCode.Q))
            {
                vertical = 1;
                horizontal = -1;
                //renderer.sprite = idleSprites[0];
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                horizontal = -1;
                vertical = -1;
                //renderer.sprite = idleSprites[2];
            }
            else if (Input.GetKey(KeyCode.C))
            {
                vertical = -1;
                horizontal = 1;
                //renderer.sprite = idleSprites[2];
            }
            else if (Input.GetKey(KeyCode.E))
            {
                horizontal = 1;
                vertical = 1;
                //renderer.sprite = idleSprites[0];
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                GameManager.instance.playersTurn = false; // Skip Turn
            }

            if (horizontal != 0 || vertical != 0)
            {
                animator.SetFloat("Horizontal", horizontal);
                animator.SetFloat("Vertical", vertical);
                animator.SetFloat("dasdsd", vertical);
                //Debug.Log("D:" + horizontal + ", " + vertical);
                AttemptMove(horizontal, vertical);
                GameManager.instance.playersTurn = false;
            }

        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);
        }

        protected override IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;

            animator.SetFloat("Magnitude", 1.0f);

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

            animator.SetFloat("Magnitude", 0.0f);

            isMoving = false;
            GameManager.instance.PlayerMoved(newPostion);
        }
    }
}