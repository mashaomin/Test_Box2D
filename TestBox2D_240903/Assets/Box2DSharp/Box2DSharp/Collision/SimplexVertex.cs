using System.Numerics;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// ������
    /// </summary>
    public struct SimplexVertex
    {
        /// <summary>
        /// ����������ת������������ ��Ҫ����ת��ƽ��
        /// </summary>
        public Vector2 Wa; // support point in proxyA

        /// <summary>
        /// ����ϵ����̫���
        /// </summary>
        public Vector2 Wb; // support point in proxyB

        /// <summary>
        /// ����ϵ����̫���
        /// </summary>
        public Vector2 W; // wB - wA

        public float A; // barycentric coordinate for closest point

        public int IndexA; // wA index

        public int IndexB; // wB index
    }
}