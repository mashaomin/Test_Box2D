using bluebean.Box2DLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawView01 : MonoBehaviour
{
    DebugDraw01 m_debugDraw = DebugDraw01.Instance;
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        m_debugDraw.DrawBatch();
        m_debugDraw.Clear();
    }
}
