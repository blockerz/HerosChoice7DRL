using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Bomb : MonoBehaviour
    {

        //private float speed;
        public Vector3 direction;
        public int fuseTurns = 2;
        private int startTurn;
        private int Damage = 1;

        void Start()
        {
            startTurn = GameManager.instance.Turns;
        }

        void Update()
        {
            if (GameManager.instance.Turns == startTurn + fuseTurns - 1)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
                if (GameManager.instance.Turns == startTurn + fuseTurns)
            {
                Vector2 originAdjustment = new Vector2(0.5f, 0.5f);
                Vector2 start = transform.position;
                //transform.GetComponent<BoxCollider2D>().enabled = false;
                RaycastHit2D hit;
                var mask = LayerMask.GetMask("Blocking");

                foreach (var vec in MovingObject.directions)
                {
                    hit = Physics2D.Raycast(start + originAdjustment, vec, 1, mask);
                    //Debug.DrawRay(start + originAdjustment, vec, Color.red, 3);

                    if (hit.transform != null && (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Enemy")))
                    {
                        Debug.Log("Bombing : " + hit.transform.name + " "  + vec);
                        //Debug.DrawLine(start + originAdjustment, hit.transform.position, Color.green, 3);

                        if (hit.transform.CompareTag("Enemy"))
                        {
                            var enemy = hit.transform.GetComponent<Enemy>();

                            enemy.ReceiveIncomingDamage(gameObject, Damage);

                            enemy.Knockback((int)vec.x, (int)vec.y); 

                        }
                        else if (hit.transform.CompareTag("Player"))
                        {
                            var player = hit.transform.GetComponent<Player>();

                            player.ReceiveIncomingDamage(gameObject, Damage);
                        }

                        //self.transform.GetComponent<BoxCollider2D>().enabled = true;
                    }
                    //else if (hit.transform != null)
                    //            {
                    //	Debug.DrawLine(start + originAdjustment, hit.transform.position, Color.blue, 3);
                    //}
                }
                //self.transform.GetComponent<BoxCollider2D>().enabled = true;

                var dp = Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/BombParticle"), this.transform);
                dp.transform.position = gameObject.transform.position + new Vector3(-1f,-1f,0);

                Destroy(gameObject, 0.6f);
            }
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

    }
}
