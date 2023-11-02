using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    public GameObject QuadPrefab;

    public int LongNum = 30;

    private int m_quadCount = 12;
    private float m_radius = 5.0f;
    public float Width => QuadPrefab.transform.localScale.x;

    private void Awake()
    {
        CalculateRadius();

        for(int i = 0; i < LongNum; i++)
        {
            GenerateRing(new Vector3(transform.position.x,
                                    transform.position.y,
                                    transform.position.z + i * Width));
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
        float angleStep = 360.0f / m_quadCount;

        for (int i = 0; i < m_quadCount; i++)
        {
            float angle = i * angleStep;
            Vector3 position = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * m_radius,
                                            Mathf.Cos(Mathf.Deg2Rad * angle) * m_radius,
                                            centerPos.z);
            Quaternion rotation = Quaternion.LookRotation(position - centerPos, Vector3.up);
            GameObject quad = Instantiate(QuadPrefab, position, rotation);
            quad.transform.parent = transform;
        }
    }

}