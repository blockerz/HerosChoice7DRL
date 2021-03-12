using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class WarpPoint : MonoBehaviour
    {
        public Vector2 warpPoint;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (warpPoint == null || warpPoint == Vector2.zero)
                    return;

                StartCoroutine(Teleport(other.transform, warpPoint));
            }
        }

        protected IEnumerator Teleport(Transform transform, Vector2 end)
        {

            Player player = transform.gameObject.GetComponent<Player>();

            while(player.isMoving)
            {
                yield return null;
            }

            player.isMoving = true;

            transform.position = end;

            player.isMoving = false;

            GameManager.instance.PlayerMoved(end);
        }
    }
}