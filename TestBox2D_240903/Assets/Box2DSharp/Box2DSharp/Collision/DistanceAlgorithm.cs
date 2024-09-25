using System;
using System.Diagnostics;
using System.Numerics;
using Box2DSharp.Common;

namespace Box2DSharp.Collision
{
    public static class DistanceAlgorithm
    {
        /// <summary>
        /// GJK碰撞检测
        /// </summary>
        /// <param name="output"></param>
        /// <param name="cache"></param>
        /// <param name="input"></param>
        /// <param name="gJkProfile"></param>
        public static void Distance(
            out DistanceOutput output,
            ref SimplexCache cache,
            in DistanceInput input,
            in GJkProfile gJkProfile = null)
        {
            if (gJkProfile != null)
            {
                ++gJkProfile.GjkCalls;
            }

            output = new DistanceOutput();
            ref readonly var proxyA = ref input.ProxyA;
            ref readonly var proxyB = ref input.ProxyB;

            var transformA = input.TransformA;
            var transformB = input.TransformB;

            // 初始化单纯形
            var simplex = new Simplex();
            simplex.ReadCache(
                ref cache,
                proxyA,
                transformA,
                proxyB,
                transformB);

            // Get simplex vertices as an array.
            ref var vertices = ref simplex.Vertices;
            const int maxIters = 20;

            // These store the vertices of the last simplex so that we
            // can check for duplicates and prevent cycling.保存上次单纯形的顶点索引，用于检测重复
            Span<int> saveA = stackalloc int[3];
            Span<int> saveB = stackalloc int[3];

            // 设定最大迭代次数，防止算法陷入无限循环
            var iter = 0;
            while (iter < maxIters)
            {
                // 保存当前单纯形的顶点索引，以便之后检测重复
                var saveCount = simplex.Count;
                for (var i = 0; i < simplex.Count; ++i)
                {
                    saveA[i] = vertices[i].IndexA;
                    saveB[i] = vertices[i].IndexB;
                }

                switch (simplex.Count)
                {
                    case 1:
                        break;

                    case 2:
                        simplex.Solve2(); 
                        break;

                    case 3:
                        simplex.Solve3();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(simplex.Count));
                }

                // 如果当前单纯形已经有 3 个顶点，则说明原点在三角形内，结束循环
                if (simplex.Count == 3)
                {
                    break;
                }

                // 获取下一步的搜索方向,根据线段AB 得到法向量，朝原点靠近
                var d = simplex.GetSearchDirection();

                // Ensure the search direction is numerically fit.
                if (d.LengthSquared() < Settings.Epsilon * Settings.Epsilon)
                {
                    // The origin is probably contained by a line segment
                    // or triangle. Thus the shapes are overlapped.

                    // We can't return zero here even though there may be overlap.
                    // In case the simplex is a point, segment, or triangle it is difficult
                    // to determine if the origin is contained in the CSO or very close to it.
                    break;
                }

                // Compute a tentative new simplex vertex using support points.
                ref var vertex = ref vertices[simplex.Count];
                // 计算形状A 在方向-d上的支撑点 找到A的支撑点，再转成世界坐标
                vertex.IndexA = proxyA.GetSupport(MathUtils.MulT(transformA.Rotation, -d));
                vertex.Wa = MathUtils.Mul(transformA, proxyA.GetVertex(vertex.IndexA));

                // 计算形状 B 在方向 d 上的支持点
                vertex.IndexB = proxyB.GetSupport(MathUtils.MulT(transformB.Rotation, d));
                vertex.Wb = MathUtils.Mul(transformB, proxyB.GetVertex(vertex.IndexB));

                // 计算新支持点的向量
                vertex.W = vertex.Wb - vertex.Wa;

                // Iteration count is equated to the number of support point calls.
                ++iter;
                if (gJkProfile != null)
                {
                    ++gJkProfile.GjkIters;
                }

                // 检查是否有重复的支持点，这是 GJK 算法终止的主要条件
                var duplicate = false;
                for (var i = 0; i < saveCount; ++i)
                {
                    // 如果找到重复的支持点，退出循环避免无限循环
                    if (vertex.IndexA == saveA[i] && vertex.IndexB == saveB[i])
                    {
                        duplicate = true;
                        break;
                    }
                }

                // 如果发现重复点，退出循环
                if (duplicate)
                {
                    break;
                }

                // 增加单纯形的顶点数
                ++simplex.Count;
            }

            if (gJkProfile != null)
            {
                gJkProfile.GjkMaxIters = Math.Max(gJkProfile.GjkMaxIters, iter);
            }

            // 准备输出结果
            simplex.GetWitnessPoints(out output.PointA, out output.PointB);// 获取两个形状的最近点
            output.Distance = Vector2.Distance(output.PointA, output.PointB);// 计算两点之间的距离
            output.Iterations = iter;// 返回迭代次数

            // 将当前单纯形写入缓存，以便下次加速计算
            simplex.WriteCache(ref cache);

            // 如果启用了半径修正，进一步调整最近点结果
            // 如果这两个形状具有物理半径（如圆形或其他形状的边界），则需要对最近点的计算结果进行调整，以确保它们反映出实际的物理接触情况。
            if (input.UseRadii)
            {
                if (output.Distance < Settings.Epsilon)
                {
                    // Shapes are too close to safely compute normal
                    var p = 0.5f * (output.PointA + output.PointB);
                    output.PointA = p;
                    output.PointB = p;
                    output.Distance = 0.0f;
                }
                else
                {
                    // Keep closest points on perimeter even if overlapped, this way
                    // the points move smoothly.
                    var rA = proxyA.Radius;
                    var rB = proxyB.Radius;
                    var normal = output.PointB - output.PointA;
                    normal.Normalize();
                    output.Distance = Math.Max(0.0f, output.Distance - rA - rB);
                    output.PointA += rA * normal;
                    output.PointB -= rB * normal;
                }
            }
        }

        /// <summary>
        /// Perform a linear shape cast of shape B moving and shape A fixed. Determines the hit point, normal, and translation fraction.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns>true if hit, false if there is no hit or an initial overlap</returns>
        public static bool ShapeCast(out ShapeCastOutput output, in ShapeCastInput input)
        {
            output = new ShapeCastOutput
            {
                Iterations = 0,
                Lambda = 1.0f,
                Normal = Vector2.Zero,
                Point = Vector2.Zero
            };

            ref readonly var proxyA = ref input.ProxyA;
            ref readonly var proxyB = ref input.ProxyB;

            var radiusA = Math.Max(proxyA.Radius, Settings.PolygonRadius);
            var radiusB = Math.Max(proxyB.Radius, Settings.PolygonRadius);
            var radius = radiusA + radiusB;

            var xfA = input.TransformA;
            var xfB = input.TransformB;

            var r = input.TranslationB;
            var n = new Vector2(0.0f, 0.0f);
            var lambda = 0.0f;

            // Initial simplex
            var simplex = new Simplex();

            // Get simplex vertices as an array.
            // ref var vertices = ref simplex.Vertices;

            // Get support point in -r direction
            var indexA = proxyA.GetSupport(MathUtils.MulT(xfA.Rotation, -r));
            var wA = MathUtils.Mul(xfA, proxyA.GetVertex(indexA));
            var indexB = proxyB.GetSupport(MathUtils.MulT(xfB.Rotation, r));
            var wB = MathUtils.Mul(xfB, proxyB.GetVertex(indexB));
            var v = wA - wB;

            // Sigma is the target distance between polygons
            var sigma = Math.Max(Settings.PolygonRadius, radius - Settings.PolygonRadius);
            const float tolerance = 0.5f * Settings.LinearSlop;

            // Main iteration loop.
            // 迭代次数上限
            const int maxIters = 20;
            var iter = 0;
            while (iter < maxIters && v.Length() - sigma > tolerance)
            {
                Debug.Assert(simplex.Count < 3);

                output.Iterations += 1;

                // Support in direction -v (A - B)
                indexA = proxyA.GetSupport(MathUtils.MulT(xfA.Rotation, -v));
                wA = MathUtils.Mul(xfA, proxyA.GetVertex(indexA));
                indexB = proxyB.GetSupport(MathUtils.MulT(xfB.Rotation, v));
                wB = MathUtils.Mul(xfB, proxyB.GetVertex(indexB));
                var p = wA - wB;

                // -v is a normal at p
                v.Normalize();

                // Intersect ray with plane
                var vp = Vector2.Dot(v, p);
                var vr = Vector2.Dot(v, r);
                if (vp - sigma > lambda * vr)
                {
                    if (vr <= 0.0f)
                    {
                        return false;
                    }

                    lambda = (vp - sigma) / vr;
                    if (lambda > 1.0f)
                    {
                        return false;
                    }

                    n = -v;
                    simplex.Count = 0;
                }

                // Reverse simplex since it works with B - A.
                // Shift by lambda * r because we want the closest point to the current clip point.
                // Note that the support point p is not shifted because we want the plane equation
                // to be formed in unshifted space.
                ref var vertex = ref simplex.Vertices[simplex.Count];
                vertex.IndexA = indexB;
                vertex.Wa = wB + lambda * r;
                vertex.IndexB = indexA;
                vertex.Wb = wA;
                vertex.W = vertex.Wb - vertex.Wa;
                vertex.A = 1.0f;

                simplex.Count += 1;

                switch (simplex.Count)
                {
                    case 1:
                        break;

                    case 2:
                        simplex.Solve2();
                        break;

                    case 3:
                        simplex.Solve3();
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }

                // If we have 3 points, then the origin is in the corresponding triangle.
                if (simplex.Count == 3)
                {
                    // Overlap
                    return false;
                }

                // Get search direction.
                v = simplex.GetClosestPoint();

                // Iteration count is equated to the number of support point calls.
                ++iter;
            }

            if (iter == 0)
            {
                // Initial overlap
                return false;
            }

            // Prepare output.
            simplex.GetWitnessPoints(out var pointB, out var pointA);

            if (v.LengthSquared() > 0.0f)
            {
                n = -v;
                n.Normalize();
            }

            output.Point = pointA + radiusA * n;
            output.Normal = n;
            output.Lambda = lambda;
            output.Iterations = iter;
            return true;
        }
    }
}