using Box2DSharp.Common;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// ���ڴ洢ִ�о�����������������Ϣ����������������״�Ĵ����任��Ϣ���Ƿ�ʹ�ð뾶�ı�־��
    /// </summary>
    public struct DistanceInput
    {
        // ��״ A �ľ������
        public DistanceProxy ProxyA;

        // // ��״ B �ľ������
        public DistanceProxy ProxyB;

        // ��״ A ��λ����Ϣ
        public Transform TransformA;

        // // ��״B��λ����Ϣ
        public Transform TransformB;

        public bool UseRadii;
    }
}