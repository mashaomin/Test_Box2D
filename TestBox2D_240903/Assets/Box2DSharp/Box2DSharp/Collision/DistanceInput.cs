using Box2DSharp.Common;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// 用于存储执行距离计算所需的输入信息。它包含了两个形状的代理、变换信息和是否使用半径的标志。
    /// </summary>
    public struct DistanceInput
    {
        // 形状 A 的距离代理
        public DistanceProxy ProxyA;

        // // 形状 B 的距离代理
        public DistanceProxy ProxyB;

        // 形状 A 的位置信息
        public Transform TransformA;

        // // 形状B的位置信息
        public Transform TransformB;

        public bool UseRadii;
    }
}