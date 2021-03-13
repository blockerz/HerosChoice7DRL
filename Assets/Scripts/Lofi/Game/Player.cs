using Lofi.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Player : MovingObject
    {
        private SpriteRenderer swordRenderer;
        private Animator animator;
        public Sprite[] SwordSprites;
        private SwordAnimator swordAnimator;
        private ThrowBoomerang throwBoomerang;
        private ShootArrow shootArrow;
        public List<IUseItem> items;
        public int activeItem = 0;

        int lastMoveHorizontal = 0;
        int lastMoveVertical = 0;

        protected override void Start()
        {
            base.Start();
            Health = 8;
            MaxHealth = 8;
            Damage = 1;
        }

        private void Awake()
        {
            base.Awake();
            SwordSprites = Resources.LoadAll<Sprite>("Sprites/Player/Sword");
            swordRenderer = transform.Find("Sword").gameObject.GetComponent<SpriteRenderer>();
            swordAnimator = transform.Find("Sword").gameObject.GetComponent<SwordAnimator>();
            throwBoomerang = transform.gameObject.GetComponent<ThrowBoomerang>();
            shootArrow = transform.gameObject.GetComponent<ShootArrow>();
            animator = GetComponent<Animator>();
            swordRenderer.sprite = SwordSprites[0];
            swordAnimator.gameObject.SetActive(false);

            items = new List<IUseItem>();

            items.Add(shootArrow);
            items.Add(throwBoomerang);

        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.instance.playersTurn) return;

            if (isMoving) return;

            int horizontal = 0;
            int vertical = 0;

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                items[activeItem].UseItemWithDirection(new Vector3(0, 1f, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.W))
            {
                vertical = 1;
                swordRenderer.sprite = SwordSprites[0];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            {
                items[activeItem].UseItemWithDirection(new Vector3(-1f, 0, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
                swordRenderer.sprite = SwordSprites[3];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.X)))
            {
                items[activeItem].UseItemWithDirection(new Vector3(0, -1f, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.X))
            {
                vertical = -1;
                swordRenderer.sprite = SwordSprites[2];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
            {
                items[activeItem].UseItemWithDirection(new Vector3(1f, 0, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
                swordRenderer.sprite = SwordSprites[1];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Q))
            {
                items[activeItem].UseItemWithDirection(new Vector3(-1f, 1, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                horizontal = -1;
                vertical = 1;
                swordRenderer.sprite = SwordSprites[7];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Z))
            {
                items[activeItem].UseItemWithDirection(new Vector3(-1f, -1f, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                horizontal = -1;
                vertical = -1;
                swordRenderer.sprite = SwordSprites[6];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
            {
                items[activeItem].UseItemWithDirection(new Vector3(1f, -1f, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.C))
            {
                horizontal = 1;
                vertical = -1;
                swordRenderer.sprite = SwordSprites[5];
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.E))
            {
                items[activeItem].UseItemWithDirection(new Vector3(1f, 1, 0));
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.E))
            {
                horizontal = 1;
                vertical = 1;
                swordRenderer.sprite = SwordSprites[4];
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                EndTurn();
            }
            else if (Input.GetKey(KeyCode.B))
            {
                activeItem = (activeItem + 1) % (items.Count);
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

                EndTurn();
            }

        }

        private static void EndTurn()
        {
            GameManager.instance.Turns++;
            GameManager.instance.playersTurn = false; // Skip Turn
        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);

            if (other != null)
            {
                Enemy enemy = other.GetComponent<Enemy>();

                if (enemy != null)
                {
                    //swordAnimator.gameObject.SetActive(true);
                    swordAnimator.AnimateSword(new Vector3(lastMoveHorizontal, lastMoveVertical, 0));
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