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


        protected override void Start()
        {
            base.Start();
            Health = 2;
            MaxHealth = 2;
            Damage = 1;
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

            section.RemoveEnemyFromList(this);

            Destroy(gameObject);
        }
    }
}