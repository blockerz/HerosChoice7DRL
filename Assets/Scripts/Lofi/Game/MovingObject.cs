using UnityEngine;
using System.Collections;

namespace Lofi.Game
{
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.03f;           
		public LayerMask blockingLayer;         


		protected BoxCollider2D boxCollider;      
		protected Rigidbody2D rb2D;               
		protected Transform placeholder;               
		protected float inverseMoveTime;          
		protected bool isMoving;                  

		public int Health { get; set; }
		public int Damage { get; set; }

		protected virtual void Start()
		{
			boxCollider = GetComponent<BoxCollider2D>();
			rb2D = GetComponent<Rigidbody2D>();
			placeholder = transform.Find("Placeholder");
			placeholder.gameObject.SetActive(false);
			inverseMoveTime = 1f / moveTime;
			blockingLayer = LayerMask.GetMask("Blocking");
		}


		protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
		{
			Vector2 originAdjustment = new Vector2(0.5f, 0.5f);
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2(xDir, yDir);
			boxCollider.enabled = false;

			hit = Physics2D.Linecast(start + originAdjustment, end + originAdjustment, blockingLayer);
			Debug.DrawLine(start + originAdjustment, end + originAdjustment, Color.red, 3);
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

			placeholder.gameObject.SetActive(true);
			placeholder.transform.position = end;

			while (sqrRemainingDistance > 1)
			{
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition(newPostion);

				sqrRemainingDistance = (transform.position - end).sqrMagnitude;

				yield return null;
			}

			rb2D.MovePosition(new Vector3(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), end.z));

			placeholder.gameObject.SetActive(false);
			isMoving = false;
		}


		protected virtual void AttemptMove(int xDir, int yDir)
		{
			RaycastHit2D hit;

			bool canMove = Move(xDir, yDir, out hit);

			if (hit.transform == null)
				return;

			GameObject other = hit.transform.gameObject;

			if (other.name.Equals("Placeholder"))
				other = other.transform.parent.gameObject;

			if (!canMove && other != null)
				OnCantMove(other);
		}


		protected abstract void OnCantMove(GameObject other);
	}
}