using Box2DSharp.Common;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// Used to warm start b2Distance.
    /// Set count to zero on first call.
    /// GJK 算法每次执行时都需要找到两个形状之间的最近点，
    /// 而 SimplexCache 保存了上一次计算的结果，从而使算法可以基于这些信息快速地进行后续计算。
    /// 避免了每次都从头开始计算，减少了重复计算的开销。
    /// 当进行 GJK 碰撞检测时：
    /// 1. 如果有缓存的 SimplexCache，则首先检查其有效性。
    /// 2. 如果缓存有效，则从缓存的单纯形开始进行最近点查询，从而跳过初始的计算步骤。
    /// 3. 如果缓存无效或形状变化过大，则会重新计算并更新缓存。
    /// </summary>
    public struct SimplexCache
    {
        /// length or area
        // 用于衡量单纯形大小的度量（比如距离），帮助判断缓存的有效性
        public float Metric;                

        public ushort Count;                // 单纯形中顶点的数量（1、2 或 3）

        /// vertices on shape A
        public FixedArray3<byte> IndexA;    // 两个形状各自顶点的索引，指向各自形状中参与最近点计算的顶点

        /// vertices on shape B
        public FixedArray3<byte> IndexB;
    }
}