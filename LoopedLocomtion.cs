using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopedLocomtion : MonoBehaviour
{
    FloatKeyframeAnimator animatorX;
    FloatKeyframeAnimator animatorY;
    // Use this for initialization
    void Start()
    {

        animatorX = new FloatKeyframeAnimator();
        animatorY = new FloatKeyframeAnimator();

        //Demo 2: Loop the movement on a triangle


        animatorX.AddKey(6, 0);
        animatorY.AddKey(0, 0);

        animatorX.AddKey(Mathf.Sqrt(18), 2);
        animatorY.AddKey(Mathf.Sqrt(18), 2);

        animatorX.AddKey(0, 4);
        animatorY.AddKey(6, 4);

        animatorX.AddKey(-Mathf.Sqrt(18), 6);
        animatorY.AddKey(Mathf.Sqrt(18), 6);

        animatorX.AddKey(-6, 8);
        animatorY.AddKey(0, 8);

        animatorX.AddKey(-Mathf.Sqrt(18), 10);
        animatorY.AddKey(-Mathf.Sqrt(18), 10);

        animatorX.AddKey(0, 12);
        animatorY.AddKey(-6, 12);

        animatorX.AddKey(Mathf.Sqrt(18), 14);
        animatorY.AddKey(-Mathf.Sqrt(18), 14);

        animatorX.AddKey(6, 16);
        animatorY.AddKey(0, 16);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 current_pos = gameObject.transform.position;


        //demo 2: move on a triangle and loop the animation
        float time = Time.time;
        float time_mod16 = time - ((int)(time / 16)) * 16;
        current_pos.x = animatorX.Sample(time_mod16);
        current_pos.y = animatorY.Sample(time_mod16);

        gameObject.transform.position = current_pos;
    }
}