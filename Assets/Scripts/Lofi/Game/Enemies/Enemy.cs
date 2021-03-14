using RogueSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lofi.Game
{
    public class Enemy : MovingObject
    {
        private Transform target;
        public Map visionMap;
        GameMapSection section;
        public IEnemyBehavior enemyBehavior;
        private int stunned = 0;

        protected override void Start()
        {
            base.Start();
            //Health = 2;
            //MaxHealth = 2;
            //Damage = 1;
        }

        public virtual void UpdateVisionMap()
        {
            section = GameManager.instance.ActiveSection;
            visionMap = new Map(section.Width, section.Height);

            for (int y = 0; y < section.Height; y++)
            {
                for(int x = 0; x < section.Width; x++)
                {
                    if (section.GetTile(x,y) == null)
                    {
                        if(section.GetEntityAtPosition(x,y) != null)
                            visionMap.SetCellProperties(x, y, false, false);
                        else
                            visionMap.SetCellProperties(x, y, true, true);
                    }
                }
            }
        }

        public virtual bool EnemyTurn()
        {
            if (target == null)
                target = GameObject.FindGameObjectWithTag("Player").transform;
            try
            {
                if (stunned > 0)
                {
                    FlashColor(Color.yellow);
                    stunned--;
                    return true;
                }

                if (enemyBehavior == null)
                    DecideNextMove();
                else
                    enemyBehavior.DecideNextMove(this);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }

        public void GetStunned(int turns)
        {
            stunned = turns;
        }

        protected virtual void DecideNextMove()
        {
            MoveTowardPlayer();
        }

        public void MoveTowardPlayer()
        {
            UpdateVisionMap();

            Vector2Int playerLocation = new Vector2Int(Mathf.RoundToInt(target.position.x - section.transform.position.x),
                Mathf.RoundToInt(target.position.y - section.transform.position.y));
            Vector2Int enemyLocation = new Vector2Int(Mathf.RoundToInt(transform.position.x - section.transform.position.x),
                Mathf.RoundToInt(transform.position.y - section.transform.position.y));

            visionMap.SetCellProperties(playerLocation.x, playerLocation.y, true, true);
            visionMap.SetCellProperties(enemyLocation.x, enemyLocation.y, true, true);

            PathFinder pathfinder = new PathFinder(visionMap, 1.0);
            Path path = pathfinder.ShortestPath(visionMap.GetCell(enemyLocation.x, enemyLocation.y),
                visionMap.GetCell(playerLocation.x, playerLocation.y));
            var nextStep = path.StepForward();

            int xDir = nextStep.X - enemyLocation.x;
            int yDir = nextStep.Y - enemyLocation.y;

            AttemptMove(xDir, yDir);
        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);

            if (other != null)
            {
                Player player = other.GetComponent<Player>();

                if (player != null)
                    player.ReceiveIncomingDamage(gameObject, Damage);
            }
        }

        protected override void OnDeath(GameObject other)
        {
            Debug.Log(this.name + " was killed by " + other.name);

            if (this.name.ToUpper().Contains("BOSS"))
                GameManager.instance.BossBeaten();

            var dp = Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/DeathParticle"), section.transform);
            dp.transform.position = gameObject.transform.position;

            section.DropLoot(gameObject);
            gameObject.SetActive(false);

            Destroy(gameObject, 0.1f);

            section.RemoveEnemyFromList(this);
        }

        public bool Knockback(int x, int y)
        {
            if (this.name.ToUpper().Contains("BOSS"))
                return false;

            if (Health > 0)
            {
                RaycastHit2D hit;

                bool canMove = Move(x, y, out hit);

                if (hit.transform == null)
                {
                    Debug.Log(this.name + " knocked back.");
                    return true;
                }
            }
            return false;
        }
        public override bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector3 newPos = transform.position + new Vector3(xDir, yDir);

            GameMapSection section = GameManager.instance.ActiveSection;
            
            if (shouldRemainStationary || newPos.x < section.transform.position.x || newPos.y < section.transform.position.y ||
                newPos.x >= section.transform.position.x + section.Width || newPos.y >= section.transform.position.y + section.Height)
            {
                hit = new RaycastHit2D();
                return true; // true to pass the turn without an attack
            }

            return base.Move(xDir, yDir, out hit);
        }
    }
}