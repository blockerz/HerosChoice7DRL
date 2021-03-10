using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Enemy : MovingObject
    {
        private Transform target;

        protected override void Start()
        {
            base.Start();

            GameManager.instance.AddEnemyToList(this);
        }

        public virtual bool EnemyTurn()
        {
            if (target == null)
                target = GameObject.FindGameObjectWithTag("Player").transform;

            int xDir = 0;
            int yDir = 0;

            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

                yDir = target.position.y > transform.position.y ? 1 : -1;

            else
                xDir = target.position.x > transform.position.x ? 1 : -1;

            AttemptMove(xDir, yDir);

            return true;
        }

        protected override void OnCantMove(GameObject other)
        {
            Debug.Log(this.name + " hit " + other.name);
        }
    }
}