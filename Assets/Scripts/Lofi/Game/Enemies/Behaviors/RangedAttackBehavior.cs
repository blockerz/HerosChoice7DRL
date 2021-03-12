using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class RangedAttackBehavior : IEnemyBehavior
    {
        public void AddAbilites(Enemy self)
        {
			self.gameObject.AddComponent<ShootProjectile>();
        }

        public void DecideNextMove(Enemy self)
        {
			Vector2 originAdjustment = new Vector2(0.5f, 0.5f);
			Vector2 start = self.transform.position;
			self.transform.GetComponent<BoxCollider2D>().enabled = false;
			RaycastHit2D hit;
			var mask = LayerMask.GetMask("Blocking");

			foreach (var vec in MovingObject.directions)
			{
				hit = Physics2D.Raycast(start + originAdjustment, vec, 256, mask);
				//Debug.DrawRay(start + originAdjustment, vec, Color.red, 3);

				if (hit.transform != null && hit.transform.CompareTag("Player"))
				{
					Debug.Log(self.gameObject.name + " Shooting Player: " + vec);
					Debug.DrawLine(start + originAdjustment, hit.transform.position, Color.green, 3);

					if(self.gameObject.GetComponent<ShootProjectile>() != null)
                    {
						self.gameObject.GetComponent<ShootProjectile>().Shoot(vec);

					}
                    else
                    {
						Debug.Log(self.gameObject.name + " attempted to shoot but has no ShootProjectile Ability");
					}
					self.transform.GetComponent<BoxCollider2D>().enabled = true;
					return;
				}
				//else if (hit.transform != null)
    //            {
				//	Debug.DrawLine(start + originAdjustment, hit.transform.position, Color.blue, 3);
				//}
			}
			self.transform.GetComponent<BoxCollider2D>().enabled = true;

			self.MoveTowardPlayer();
		}
    }
}