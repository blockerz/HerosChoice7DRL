using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimator : MonoBehaviour
{

    float speed;
    public Vector3 target;
    bool animating = false;
    Vector3 origin;

    void Awake()
    {
        speed = 5f;
        origin = transform.localPosition;
        //transform.SetParent(transform.parent, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

            if(Vector3.Distance(transform.localPosition, target) <= 0.6f)
            {
                animating = false;
                transform.localPosition = origin;
                gameObject.SetActive(false);
            }
        }
    }

    public void AnimateSword(Vector3 direction)
    {
        target = transform.localPosition + direction;
        animating = true;
        gameObject.SetActive(true);
    }
}
