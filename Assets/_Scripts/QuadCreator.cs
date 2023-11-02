using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    public GameObject RingPrefab;

    public int SumLength = 30;

    private int m_quadCount = 12;
    private float m_radius = 5.0f;
    public float Width => RingPrefab.transform.localScale.x;

    private List<GameObject> m_rings = new List<GameObject>();

    private void Awake()
    {
        CalculateRadius();

        for(int i = 0; i < SumLength; i++)
        {
            GenerateRing(new Vector3(transform.position.x, transform.position.y, transform.position.z + i * Width));
        }
        
    }

    void CalculateRadius()
    {
        // 根据边长计算半径
        float angle = 360.0f / m_quadCount; // 每个角的度数
        float halfAngle = angle * 0.5f; // 每半个角的度数
        float halfWidth = Width * 0.5f; // Quad的半个边长
        // 正切函数计算半径
        float radius = halfWidth / Mathf.Tan(Mathf.Deg2Rad * halfAngle);
        m_radius = radius;
    }

    void GenerateRing(Vector3 centerPos)
    {
        GameObject ring = Instantiate(RingPrefab, centerPos, Quaternion.identity);
        m_rings.Add(ring);

        //float angleStep = 360.0f / m_quadCount;

        //for (int i = 0; i < m_quadCount; i++)
        //{
        //    float angle = i * angleStep;
        //    Vector3 position = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * m_radius,
        //                                    Mathf.Cos(Mathf.Deg2Rad * angle) * m_radius,
        //                                    centerPos.z);
        //    Quaternion rotation = Quaternion.LookRotation(position - centerPos, Vector3.up);
        //    GameObject quad = Instantiate(RingPrefab, position, rotation);
        //    quad.transform.parent = transform;

        //    m_quads.Add(quad);
        //}
    }

    public float MoveSpeed = 5f;
    private Queue<GameObject> m_waitDelete = new Queue<GameObject>();
    private Queue<float> m_waitGeneratePos = new Queue<float>();
    private void Update()
    {
        m_rings.ForEach((ring) =>
        {
            //全体位移
            ring.transform.position -= new Vector3(0, 0, MoveSpeed * Time.deltaTime);

            //末尾面片待删除
            if(ring.transform.position.z < -1)
            {
                m_waitDelete.Enqueue(ring);
                m_waitGeneratePos.Enqueue(ring.transform.position.z + 29 * Width);
            }
        });

        //删除待删除面片
        while (m_waitDelete.Count > 0)
        {
            GameObject ring = m_waitDelete.Dequeue();
            m_rings.Remove(ring);
            Destroy(ring);
        }

        //生成对应面片
        while (m_waitGeneratePos.Count > 0)
        {
            float z = m_waitGeneratePos.Dequeue();
            GenerateRing(new Vector3(transform.position.x, transform.position.y, z));
        }
        
    }
}