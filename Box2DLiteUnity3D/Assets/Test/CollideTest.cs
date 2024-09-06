using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class CollideTest : MonoBehaviour
    {
        DebugDraw debugDraw = DebugDraw.Instance;
        Body body1 = new Body();
        Body body2 = new Body();
        Contact[] contacts = new Contact[2];
        public Vec2 bodyV1;
        public Vec2 bodyV2;
        int contactCount = 0;
        public float dis;
        public float changeValue = 0.01f;
        public float Size = 1;
        private void Awake()
        {
            contacts[0] = new Contact();
            contacts[1] = new Contact();

            body1.m_size = new Vec2(Size, Size);
            body2.m_size = new Vec2(Size, Size);
        }

        void Update()
        {
            debugDraw.Clear();
            ProcessInput();
            DrawXY(new Vec2(-2, 0), 4, new Vec2(0, -2), 4);
            debugDraw.DrawPoint(Vec2.Zero, BlckColor);
            body1.m_position = bodyV1;
            body2.m_position = bodyV2;

            debugDraw.DrawLine(Vec2.Zero, body1.m_position, BlckColor);
            debugDraw.DrawLine(Vec2.Zero, body2.m_position, BlckColor);

            debugDraw.DrawBox(body1.m_position, body1.m_size, body1.m_rotation, new Color(0f, 0f, 1f, 0.5f));
            debugDraw.DrawBox(body2.m_position, body2.m_size, body2.m_rotation, new Color(0f, 0f, 1f, 0.5f));

            contactCount = Collide.CollideTest(contacts, body1, body2);


            for (int i = 0; i < contactCount; i++)
            {
                var contact = contacts[i];
                debugDraw.DrawPoint(contact.m_position, BlckColor);// 黑色
            }
        }
        public Color BlckColor = new Color(0, 0, 0, 0.2f);//Color.black 黑色
        void ProcessInput()
        {

            if (Input.GetKey(KeyCode.A))
            {
                bodyV2.x -= changeValue;
                //body2.m_position.x -= changeValue;
            }
            if (Input.GetKey(KeyCode.D))
            {
                bodyV2.x += changeValue;
                //body2.m_position.x += changeValue;
            }
            if (Input.GetKey(KeyCode.W))
            {
                bodyV2.y += changeValue;
                //body2.m_position.y += changeValue;
            }
            if (Input.GetKey(KeyCode.S))
            {
                bodyV2.y -= changeValue;
                //body2.m_position.y -= changeValue;
            }
            if (Input.GetKey(KeyCode.E))
            {
                body2.m_rotation += changeValue;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                body2.m_rotation -= changeValue;
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                bodyV1.x -= changeValue;
                //body1.m_position.x -= changeValue;
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                bodyV1.x += changeValue;
                //body1.m_position.x += changeValue;
            }
            if (Input.GetKey(KeyCode.Keypad8))
            {
                bodyV1.y += changeValue;
                //body1.m_position.y += changeValue;
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                bodyV1.y -= changeValue;
                //body1.m_position.y -= changeValue;
            }
            if (Input.GetKey(KeyCode.Keypad9))
            {
                body1.m_rotation += changeValue;
            }
            if (Input.GetKey(KeyCode.Keypad7))
            {
                body1.m_rotation -= changeValue;
            }
        }

        void OnPostRender()
        {
            debugDraw.DrawBatch();
        }

        public Color BlckColor01 = new Color(0, 0, 0, 1f);//Color.black 黑色
        void DrawXY(Vec2 startX, int lengthX, Vec2 startY, int lengthY, float interval = 0.1f)
        {
            int tmp01 = 5;
            int tmp02 = 10;
            float offset = 0.015f;
            int length = (int)(lengthX / interval);
            for (int i = 0; i < length; i++)
            {
                Vec2 s1 = startX + Vec2.ZX * interval * (i);
                Vec2 s11 = s1 + Vec2.ZX * offset;

                Vec2 s2 = startX + Vec2.ZX * interval * (i + 1);
                Vec2 s21 = s2 - Vec2.ZX * offset;
                debugDraw.DrawLine(s11, s21, Color.white);
                if ((tmp01 - 1) == i % (tmp01 / 2))
                {
                    debugDraw.DrawLine(s2, s2 + Vec2.ZY * 0.1f, Color.white);
                }
                if ((tmp02 - 1) == i % tmp02)
                {
                    debugDraw.DrawLine(s2, s2 + Vec2.ZY * 0.2f, Color.white);
                }
            }

            length = (int)(lengthY / interval);
            for (int i = 0; i < length; i++)
            {
                Vec2 s1 = startY + Vec2.ZY * interval * (i);
                Vec2 s11 = s1 + Vec2.ZY * offset;

                Vec2 s2 = startY + Vec2.ZY * interval * (i + 1) ;
                Vec2 s21 = s2 - Vec2.ZY * offset;
                debugDraw.DrawLine(s11, s21, Color.white);
                if ((tmp01 - 1) == i % tmp01)
                {
                    debugDraw.DrawLine(s2, s2 + Vec2.ZX * 0.1f, Color.white);
                }
                if ((tmp02 - 1) == i % tmp02)
                {
                    debugDraw.DrawLine(s2, s2 + Vec2.ZX * 0.2f, Color.white);
                }
            }
        }
    }
}
