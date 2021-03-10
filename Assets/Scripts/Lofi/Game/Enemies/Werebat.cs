using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class Werebat : Enemy
    {

        protected override void Start()
        {
            base.Start();
            renderer.sprite = Resources.Load<Sprite>("Sprites/Enemies/Werebat");
        }

        protected override void DecideNextMove()
        {
            base.DecideNextMove();
        }
    }
}