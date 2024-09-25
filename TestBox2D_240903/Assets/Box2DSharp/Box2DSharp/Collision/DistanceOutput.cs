using System.Numerics;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// ������������״֮��Ĺ�ϵ��Ϣ
    /// DistanceOutput �ṹ���ڴ洢 GJK �㷨������Ľ��������������״֮�������㡢����֮��ľ����Լ��㷨�ĵ���������
    /// </summary>
    public struct DistanceOutput
    {
        /// closest point on shapeA
        // ��״ A �ϵ������
        public Vector2 PointA;

        /// closest point on shapeB
        // ��״ B �ϵ������
        public Vector2 PointB;
        // // ���������֮��ľ���
        public float Distance;

        /// GJK��������
        public int Iterations;
    }
}