using System.Numerics;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// 单纯形
    /// </summary>
    public struct SimplexVertex
    {
        /// <summary>
        /// 从自身坐标转换到世界坐标 需要先旋转再平移
        /// </summary>
        public Vector2 Wa; // support point in proxyA

        /// <summary>
        /// 坐标系还不太清楚
        /// </summary>
        public Vector2 Wb; // support point in proxyB

        /// <summary>
        /// 坐标系还不太清楚
        /// </summary>
        public Vector2 W; // wB - wA

        public float A; // barycentric coordinate for closest point

        public int IndexA; // wA index

        public int IndexB; // wB index
    }
}