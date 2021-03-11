using Lofi.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Player : MovingObject
    {
        //private SpriteRenderer renderer;
        private Animator animator;
        public Sprite[] idleSprites;

        int lastMoveHorizontal = 0;
        int lastMoveVertical = 0;


        protected override void Start()
        {
            base.Start();
            Health = 6;
            MaxHealth = 6;
            Damage = 1;
        }

        private void Awake()
        {
            base.Awake();
            idleSprites = Resources.LoadAll<Sprite>("Sprites/Player/Player");
            //renderer = GetComponent<SpriteRenderer>();
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
                lastMoveHorizontal = horizontal;
                lastMoveVertical = vertical;
                //Debug.Log("D:" + horizontal + ", " + vertical);
                AttemptMove(horizontal, vertical);
                GameManager.instance.playersTurn = false;
            }

        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);

            if (other != null)
            {
                Enemy enemy = other.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.ReceiveIncomingDamage(gameObject, Damage);

                    RaycastHit2D hit;

                    bool canMove = enemy.Move(lastMoveHorizontal, lastMoveVertical, out hit);

                    if (hit.transform == null)
                        Debug.Log(this.name + " knocked back " + other.name);
                }
            }
        }

        protected override IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;

            animator.SetFloat("Magnitude", 1.0f);

            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            Vector3 newPostion; 

            while (sqrRemainingDistance > 1)
            {
                newPostion = Vector3.MoveTowards(rb2D.position, end, speed * Time.deltaTime);
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

        internal void AddHealth(int healthAmount)
        {
            Health = Math.Min(Health + healthAmount, MaxHealth);
        }

        protected override void OnDeath(GameObject other)
        {
            Debug.Log(this.name + " was killed by " + other.name);
        }
    }

}