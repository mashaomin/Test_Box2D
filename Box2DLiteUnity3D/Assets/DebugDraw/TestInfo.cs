using bluebean.Box2DLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInfo : MonoBehaviour
{
    private static TestInfo _Instance;
    
    public static TestInfo Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject go = new GameObject();
                TestInfo info= go.AddComponent<TestInfo>();
                _Instance = info;
            }
            return _Instance;
        }
    }

    public float Separation;
    public Vec2 fa;
    public Vec2 fb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), fa.ToString());
        GUI.Label(new Rect(0, 50, 100, 50), fb.ToString());
    }
}
