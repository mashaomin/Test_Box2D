﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class World
    {
        public readonly List<Body> m_bodies = new List<Body>();
        public readonly List<Joint> m_joints = new List<Joint>();
        public readonly Dictionary<ArbiterKey, Arbiter> m_arbiters = new Dictionary<ArbiterKey, Arbiter>();
        public Vec2 m_gravity;
        public int m_iterations;
        public static bool accumulateImpulses = true;
        public static bool warmStarting = true;
        public static bool positionCorrection = true;

        public World(Vec2 gravity, int iterations)
        {
            m_gravity = gravity;
            m_iterations = iterations;
        }

        public void Add(Body body)
        {
            m_bodies.Add(body);
        }

        public void Add(Joint joint)
        {
            m_joints.Add(joint);
        }

        public void Clear()
        {
            foreach (var body in m_bodies)
            {
                body.Clear();
            }
            m_bodies.Clear();
            m_joints.Clear();
            m_arbiters.Clear();
        }

        /// <summary>
        /// 相交检测，接触约束更新
        /// </summary>
        public void BroadPhase()
        {
            // O(n^2) broad-phase
            for (int i = 0; i < m_bodies.Count; ++i)
            {
                Body bi = m_bodies[i];
                for (int j = i + 1; j < m_bodies.Count; ++j)
                {
                    Body bj = m_bodies[j];
                    if (bi.m_invMass == 0.0f && bj.m_invMass == 0.0f)
                        continue;
                    Arbiter newArb = new Arbiter(bi, bj);
                    ArbiterKey key = new ArbiterKey(bi, bj);
                    if (newArb.m_numContacts > 0)
                    {
                        Arbiter iter;
                        if (!m_arbiters.TryGetValue(key, out iter))
                        {
                            m_arbiters.Add(key, newArb);
                        }
                        else
                        {
                            iter.Update(newArb.m_contacts, newArb.m_numContacts);
                        }
                    }
                    else
                    {
                        m_arbiters.Remove(key);
                    }
                }
            }
        }

        public void Step(float deltaTime)
        {
            // Determine overlapping bodies and update contact points.
            // 碰撞检测
            BroadPhase();

            // Integrate forces.
            //对应用力积分得到速度的变化，变化的速度会在约束求解阶段合理化
            for (int i = 0; i < m_bodies.Count; ++i)
            {
                Body b = m_bodies[i];
                if (b.m_invMass == 0.0f)
                    continue;
                b.m_velocity += (m_gravity + b.m_force * b.m_invMass) * deltaTime;
                b.m_angularVelocity += deltaTime * b.m_torque* b.m_invI;
            }

            // Perform pre-steps.
            foreach(var arbiter in m_arbiters.Values)
            {
                arbiter.PreStep(deltaTime);
            }
            // 关节处理，先忽略
            for (int i = 0; i < m_joints.Count; ++i)
            {
                m_joints[i].PreStep(deltaTime);
            }

            // Perform iterations
            //约束求解，最后得到接近符合约束的速度值
            for (int i = 0; i < m_iterations; ++i)
            {
                foreach (var arbiter in m_arbiters.Values)
                {
                    arbiter.ApplyImpulse();
                }
                for (int j = 0; j < m_joints.Count; ++j)
                {
                    m_joints[j].ApplyImpulse();
                }
            }

            // Integrate Velocities
            for (int i = 0; i < m_bodies.Count; ++i)
            {
                Body b = m_bodies[i];

                b.m_position += b.m_velocity * deltaTime;
                b.m_rotation += b.m_angularVelocity * deltaTime;

                b.m_force.Set(0.0f, 0.0f);
                b.m_torque = 0.0f;
            }
        }

       
    }
}