using bluebean.Box2DLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw01 
{
    #region
    private static DebugDraw01 m_instance = new DebugDraw01();
    public static DebugDraw01 Instance { get { return m_instance; } }

    private List<DDVertex> m_vertexListBatch = new List<DDVertex>();
    private List<DDVertex> m_lineListBatch = new List<DDVertex>();

    #endregion

    #region public 

    public void Clear()
    {
        m_vertexListBatch.Clear();
        m_lineListBatch.Clear();
    }

    public void DrawBatch()
    {
        foreach (var ddVertex in m_vertexListBatch)
        {
            _DrawPoint(new Vector2(ddVertex.m_positions[0].x, ddVertex.m_positions[0].y), 0.04f, 36,
                ddVertex.m_color);
        }
        //draw line
        foreach (var ddVertex in m_lineListBatch)
        {
            var p1 = ddVertex.m_positions[0];
            var p2 = ddVertex.m_positions[1];
            _DrawLine(new Vector2(p1.x, p1.y), new Vector2(p2.x, p2.y), ddVertex.m_color);
        }
    }
    #endregion

    #region public 

    public void DrawLine(Vec2 p1, Vec2 p2, Color color)
    {
        m_lineListBatch.Add(DDVertex.FromLine(p1, p2, color));
    }

    public void DrawBox(Vec2 center, Vec2 size, float rotation, Color color)
    {
        Mat22 mR = new Mat22(rotation);
        Vec2 halfSize = size * 0.5f;
        Vec2 p1 = new Vec2(halfSize.x, halfSize.y);
        Vec2 p2 = new Vec2(-halfSize.x, halfSize.y);
        Vec2 p3 = new Vec2(-halfSize.x, -halfSize.y);
        Vec2 p4 = new Vec2(halfSize.x, -halfSize.y);
        p1 = mR * p1;
        p2 = mR * p2;
        p3 = mR * p3;
        p4 = mR * p4;
        p1 += center;
        p2 += center;
        p3 += center;
        p4 += center;
        DrawLine(p1, p2, color);
        DrawLine(p2, p3, color);
        DrawLine(p3, p4, color);
        DrawLine(p4, p1, color);
    }

    public void DrawPoint(Vec2 point, Color color)
    {
        m_vertexListBatch.Add(DDVertex.FromPoint(point, color));
    }

    #endregion

    #region static 绘制方法


    public static void _DrawPoint(Vector2 pos, float radius, int segment, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(pos.ToV3(), 0.1f);
    }

    

    private static void _DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(p1.ToV3(), p2.ToV3());
    }

    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public static class Vector2Extend
{
    public static Vector3 ToV3(this Vector2 a)
    {
        return new Vector3(a.x, 0, a.y);
    }
}