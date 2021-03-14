using UnityEngine;
using System.Collections;
using Lofi.Maps;

namespace Lofi.Game
{
	public abstract class MovingObject : MonoBehaviour
	{	
		[HideInInspector]
		public float moveTime;
		[HideInInspector]
		public LayerMask blockingLayer;
		[HideInInspector]
		public bool shouldRemainStationary = false;

		protected BoxCollider2D boxCollider;      
		protected Rigidbody2D rb2D;               
		protected Transform placeholder;               
		protected float speed = 30;          
		public bool isMoving;
		public SpriteRenderer renderer;
		public IEnemyBehavior enemyBehavior;

		public static Vector2[] directions =
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

		public static Vector2 GetRandomDirection()
        {
			int index = MapFactory.RandomGenerator.Next(directions.Length - 1);
			return directions[index];
        }

		protected void Awake()
		{
			renderer = GetComponent<SpriteRenderer>();
		}

		public int Health { get; set; }
		public int MaxHealth { get; set; }
		public int Damage { get; set; }

		protected virtual void Start()
		{
			boxCollider = GetComponent<BoxCollider2D>();
			rb2D = GetComponent<Rigidbody2D>();
			placeholder = transform.Find("Placeholder");
			placeholder.gameObject.SetActive(false);			
			blockingLayer = LayerMask.GetMask("Blocking");
			moveTime = 0.2f;
		}


		public virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
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
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, speed * Time.deltaTime);
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

			if (canMove || hit.transform == null)
				return;

			GameObject other = hit.transform.gameObject;

			if (other.name.Equals("Placeholder"))
				other = other.transform.parent.gameObject;

			if (!canMove && other != null && other.transform != this.transform)
				OnCantMove(other);
		}

		public void ReceiveIncomingDamage(GameObject other, int damage)
        {
			if(Health > 0)
				Health -= damage;
			FlashColor(Color.red);

			if (Health <= 0)
				OnDeath(other);
        }

		public void FlashColor(Color color)
		{
			if (renderer == null)
				return;

			renderer.color = color;
			StartCoroutine(Flash());
		}

		private IEnumerator Flash()
		{
			yield return new WaitForSeconds(0.2f);
			renderer.color = Color.white;
		}

		protected abstract void OnCantMove(GameObject other);
		protected abstract void OnDeath(GameObject other);
	}
}