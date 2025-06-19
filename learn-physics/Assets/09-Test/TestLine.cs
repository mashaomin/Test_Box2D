using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLine : MonoBehaviour
{
    public Vector2 pos01;
    public Vector2 pos02;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(To3(pos01), To3(pos02), Color.white);

        Vector2 l = pos01 - pos02;
        float sqrDistanceL = l.sqrMagnitude;
        float r2 = -Vector2.Dot(l, pos01) / sqrDistanceL;

        Vector2 t03 = pos01 + l * r2;

        Debug.DrawLine(Vector3.zero,To3(t03),Color.red);
    }

    public Vector3 To3(Vector2 pos)
    {
        return new Vector3(pos.x, 0, pos.y);
    }
}
