using Box2DSharp.Common;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// Used to warm start b2Distance.
    /// Set count to zero on first call.
    /// GJK �㷨ÿ��ִ��ʱ����Ҫ�ҵ�������״֮�������㣬
    /// �� SimplexCache ��������һ�μ���Ľ�����Ӷ�ʹ�㷨���Ի�����Щ��Ϣ���ٵؽ��к������㡣
    /// ������ÿ�ζ���ͷ��ʼ���㣬�������ظ�����Ŀ�����
    /// ������ GJK ��ײ���ʱ��
    /// 1. ����л���� SimplexCache�������ȼ������Ч�ԡ�
    /// 2. ���������Ч����ӻ���ĵ����ο�ʼ����������ѯ���Ӷ�������ʼ�ļ��㲽�衣
    /// 3. ���������Ч����״�仯����������¼��㲢���»��档
    /// </summary>
    public struct SimplexCache
    {
        /// length or area
        // ���ں��������δ�С�Ķ�����������룩�������жϻ������Ч��
        public float Metric;                

        public ushort Count;                // �������ж����������1��2 �� 3��

        /// vertices on shape A
        public FixedArray3<byte> IndexA;    // ������״���Զ����������ָ�������״�в�����������Ķ���

        /// vertices on shape B
        public FixedArray3<byte> IndexB;
    }
}