using UnityEngine;
using System.Collections;

namespace Lofi.Game
{
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
		public LayerMask blockingLayer;         //Layer on which collision will be checked.


		protected BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
		protected Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
		protected float inverseMoveTime;          //Used to make movement more efficient.
		protected bool isMoving;                  //Is the object currently moving.


		protected virtual void Start()
		{
			boxCollider = GetComponent<BoxCollider2D>();
			rb2D = GetComponent<Rigidbody2D>();
			inverseMoveTime = 1f / moveTime;
		}


		protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
		{
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2(xDir, yDir);
			boxCollider.enabled = false;

			hit = Physics2D.Linecast(start, end, blockingLayer);

			boxCollider.enabled = true;

			if (hit.transform == null && !isMoving)
			{
				StartCoroutine(SmoothMovement(end));
				return true;
			}

			return false;
		}


		protected virtual IEnumerator SmoothMovement(Vector3 end)
		{
			isMoving = true;

			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			while (sqrRemainingDistance > 1)
			{
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition(newPostion);

				sqrRemainingDistance = (transform.position - end).sqrMagnitude;

				yield return null;
			}

			rb2D.MovePosition(new Vector3(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), end.z));

			isMoving = false;
		}


		//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
		protected virtual void AttemptMove(int xDir, int yDir)
		{
			RaycastHit2D hit;

			bool canMove = Move(xDir, yDir, out hit);

			if (hit.transform == null)
				return;

			GameObject other = hit.transform.gameObject;

			if (!canMove && other != null)
				OnCantMove(other);
		}


		protected abstract void OnCantMove(GameObject other);
	}
}