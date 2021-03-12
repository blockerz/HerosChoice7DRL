using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class RangedAttackBehavior : IEnemyBehavior
    {
		private static Vector2[] directions =
		{
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(1,0),
			new Vector2(1,-1),
			new Vector2(0,-1),
			new Vector2(-1,-1),
			new Vector2(-1,0),
			new Vector2(-1,1)
		};

        public void DecideNextMove(Enemy self)
        {
			Vector2 originAdjustment = new Vector2(0.5f, 0.5f);
			Vector2 start = self.transform.position;
			self.transform.GetComponent<BoxCollider2D>().enabled = false;
			RaycastHit2D hit;
			var mask = LayerMask.GetMask("Blocking");

			foreach (var vec in directions)
			{
				hit = Physics2D.Raycast(start + originAdjustment, vec, 16, mask);
				//Debug.DrawLine(start + originAdjustment, end + originAdjustment, Color.red, 3);
				self.transform.GetComponent<BoxCollider2D>().enabled = true;

				if (hit.transform != null && hit.transform.CompareTag("Player"))
				{
					Debug.Log(self.gameObject.name + " Shooting Player: " + vec);
					return;
				}
			}

			self.MoveTowardPlayer();
		}
    }
}