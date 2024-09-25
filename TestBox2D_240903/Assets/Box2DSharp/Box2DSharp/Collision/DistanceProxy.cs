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
    /// 是一个用于表示形状几何的轻量级结构，特别是用在距离相关的算法中（如 GJK 算法、距离查询等）。
    /// 它提供了一个抽象层，用来简化对复杂形状（如多边形和圆形）进行距离计算的过程，并且避免直接操作复杂的形状结构体
    /// 
    /// DistanceProxy 的主要功能是为距离和碰撞检测算法提供一种通用的、简化的接口，
    /// 让这些算法不需要关心形状的具体类型，只需通过 DistanceProxy 来获取形状的顶点和半径信息，从而进行快速的距离计算
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
        /// 这个函数用于返回形状在某个方向上最远的顶点索引，即所谓的支持点。
        /// 支持点在碰撞检测算法（如 GJK）中非常重要，用于确定形状沿某一方向的最外部边界。
        /// 当然SAT 也可以用到
        /// </summary>
        /// <param name="d">指定方向</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure] // Pure标记方法是本方法很纯粹 不会修改对象的任何可见状态
        public int GetSupport(in Vector2 d)
        {
            var bestIndex = 0;// 这些点是自身坐标系的点，所以d需要进行旋转回到Proxy A自身坐标系中
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
        /// GetSupport 的扩展版本，它不仅返回索引，还直接返回在某个方向上最远的顶点位置（即该顶点的坐标）
        /// </summary>
        /// <param name="d">指定方向</param>
        /// <returns>顶点的坐标</returns>
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
        /// 这个函数用于获取特定索引处的顶点，返回该顶点的坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ref readonly Vector2 GetVertex(int index)
        {
            Debug.Assert(0 <= index && index < Count);
            return ref Vertices[index];
        }

        /// <summary>
        /// 顶点数组，用于表示形状的顶点。对于圆形，这通常只是一个点，而多边形则由多个顶点构成
        /// </summary>
        public Vector2[] Vertices;
        /// <summary>
        /// 顶点数量
        /// </summary>
        public int Count;
        /// <summary>
        /// 形状的凸边界半径（一般是形状的皮肤厚度或膨胀边界），用于模拟物体接触时的边缘
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