using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using Box2DSharp.Collision.Shapes;

namespace Box2DSharp.Collision
{
    /// <summary>
    /// A distance proxy is used by the GJK algorithm.
    /// It encapsulates any shape.
    /// ��һ�����ڱ�ʾ��״���ε��������ṹ���ر������ھ�����ص��㷨�У��� GJK �㷨�������ѯ�ȣ���
    /// ���ṩ��һ������㣬�����򻯶Ը�����״�������κ�Բ�Σ����о������Ĺ��̣����ұ���ֱ�Ӳ������ӵ���״�ṹ��
    /// 
    /// DistanceProxy ����Ҫ������Ϊ�������ײ����㷨�ṩһ��ͨ�õġ��򻯵Ľӿڣ�
    /// ����Щ�㷨����Ҫ������״�ľ������ͣ�ֻ��ͨ�� DistanceProxy ����ȡ��״�Ķ���Ͱ뾶��Ϣ���Ӷ����п��ٵľ������
    /// </summary>
    public struct DistanceProxy
    {
        /// Initialize the proxy using the given shape. The shape
        /// must remain in scope while the proxy is in use.
        public void Set(Shape shape, int index)
        {
            switch (shape)
            {
                case CircleShape circle:
                    {
                        Vertices = new[] { circle.Position };
                        Count = 1;
                        Radius = circle.Radius;
                    }
                    break;

                case PolygonShape polygon:
                    {
                        Vertices = polygon.Vertices;
                        Count = polygon.Count;
                        Radius = polygon.Radius;
                    }
                    break;

                case ChainShape chain:
                    {
                        Debug.Assert(0 <= index && index < chain.Count);
                        Count = 2;
                        Vertices = new Vector2[Count];
                        Vertices[0] = chain.Vertices[index];
                        if (index + 1 < chain.Count)
                        {
                            Vertices[1] = chain.Vertices[index + 1];
                        }
                        else
                        {
                            Vertices[1] = chain.Vertices[0];
                        }

                        Radius = chain.Radius;
                    }
                    break;

                case EdgeShape edge:
                    {
                        Vertices = new[]
                        {
                    edge.Vertex1,
                    edge.Vertex2
                };
                        Count = 2;
                        Radius = edge.Radius;
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void Set(Vector2[] vertices, int count, float radius)
        {
            Vertices = new Vector2[vertices.Length];
            Array.Copy(vertices, Vertices, vertices.Length);
            Count = count;
            Radius = radius;
        }

        /// <summary>
        /// ����������ڷ�����״��ĳ����������Զ�Ķ�������������ν��֧�ֵ㡣
        /// ֧�ֵ�����ײ����㷨���� GJK���зǳ���Ҫ������ȷ����״��ĳһ��������ⲿ�߽硣
        /// ��ȻSAT Ҳ�����õ�
        /// </summary>
        /// <param name="d">ָ������</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure] // Pure��Ƿ����Ǳ������ܴ��� �����޸Ķ�����κοɼ�״̬
        public int GetSupport(in Vector2 d)
        {
            var bestIndex = 0;// ��Щ������������ϵ�ĵ㣬����d��Ҫ������ת�ص�Proxy A��������ϵ��
            var bestValue = Vector2.Dot(Vertices[0], d);
            for (var i = 1; i < Count; ++i)
            {
                var value = Vector2.Dot(Vertices[i], d);
                if (value > bestValue)
                {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// GetSupport ����չ�汾��������������������ֱ�ӷ�����ĳ����������Զ�Ķ���λ�ã����ö�������꣩
        /// </summary>
        /// <param name="d">ָ������</param>
        /// <returns>���������</returns>
        public ref readonly Vector2 GetSupportVertex(in Vector2 d)
        {
            var bestIndex = 0;
            var bestValue = Vector2.Dot(Vertices[0], d);
            for (var i = 1; i < Count; ++i)
            {
                var value = Vector2.Dot(Vertices[i], d);
                if (value > bestValue)
                {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            return ref Vertices[bestIndex];
        }

        /// Get the vertex count.
        public int GetVertexCount()
        {
            return Count;
        }

        /// <summary>
        /// ����������ڻ�ȡ�ض��������Ķ��㣬���ظö��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ref readonly Vector2 GetVertex(int index)
        {
            Debug.Assert(0 <= index && index < Count);
            return ref Vertices[index];
        }

        /// <summary>
        /// �������飬���ڱ�ʾ��״�Ķ��㡣����Բ�Σ���ͨ��ֻ��һ���㣬����������ɶ�����㹹��
        /// </summary>
        public Vector2[] Vertices;
        /// <summary>
        /// ��������
        /// </summary>
        public int Count;
        /// <summary>
        /// ��״��͹�߽�뾶��һ������״��Ƥ����Ȼ����ͱ߽磩������ģ������Ӵ�ʱ�ı�Ե
        /// </summary>
        public float Radius;
    }

    public class GJkProfile
    {
        // GJK using Voronoi regions (Christer Ericson) and Barycentric coordinates.
        public int GjkCalls;

        public int GjkIters;

        public int GjkMaxIters;
    }
}