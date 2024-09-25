using System.Numerics;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// 汇总了两个形状之间的关系信息
    /// DistanceOutput 结构用于存储 GJK 算法计算出的结果，包括两个形状之间的最近点、它们之间的距离以及算法的迭代次数。
    /// </summary>
    public struct DistanceOutput
    {
        /// closest point on shapeA
        // 形状 A 上的最近点
        public Vector2 PointA;

        /// closest point on shapeB
        // 形状 B 上的最近点
        public Vector2 PointB;
        // // 两个最近点之间的距离
        public float Distance;

        /// GJK迭代次数
        public int Iterations;
    }
}