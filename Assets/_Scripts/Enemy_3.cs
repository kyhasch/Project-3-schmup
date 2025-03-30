using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    [Header("Set in Inspector")]
    public float lifeTime = 5;

    [Header("Set Dynamically")]
    public Vector3[] points;
    public float birthTime;

    private void Start() {
        points = new Vector3[3];

        points[0] = pos;

        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        Vector3 v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = pos.y;
        points[2] = v;

        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime)/lifeTime;

        if(u>1){
            Destroy(gameObject);
            return;
        }

        u = Utils.Ease(u, Utils.EasingType.sinV2, 0.2f);
        pos = Utils.Bezier(u,points);
    }
}
