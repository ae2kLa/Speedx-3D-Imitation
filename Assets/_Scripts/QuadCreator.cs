using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    private enum TubeState
    {
        Ring = 0,
        Plane,
        RingToPlane,
        PlaneToRing
    }
    [SerializeField]
    private TubeState m_tubeState;

    public GameObject QuadPrefab;

    public GameObject RingPrefab;
    private List<Vector3> RingPositions = new List<Vector3>();
    private List<Quaternion> RingRotations = new List<Quaternion>();

    public GameObject PlanePrefab;
    private List<Vector3> PlanePositions = new List<Vector3>();
    private List<Quaternion> PlaneRotations = new List<Quaternion>();

    private List<Vector3> TargetPositions = new List<Vector3>();
    private List<Quaternion> TargetRotations = new List<Quaternion>();

    public float LerpPerAdd = 0.01f;

    public int SumLength = 30;
    private int m_quadCount = 12;
    private float m_radius = 5.0f;
    public float Width => QuadPrefab.transform.localScale.x;

    private List<GameObject> m_rows = new List<GameObject>();

    private void Awake()
    {
        CalculateRadius();
        CalculateTargetStateInfo(RingPrefab, ref RingPositions, ref RingRotations);
        CalculateTargetStateInfo(PlanePrefab, ref PlanePositions, ref PlaneRotations);

        for (int i = 0; i < SumLength; i++)
        {
            GenerateRow(new Vector3(transform.position.x, transform.position.y, transform.position.z + i * Width));
        }
    }

    #region 计算前置数据
    private void CalculateRadius()
    {
        // 根据边长计算半径
        float angle = 360.0f / m_quadCount; // 每个角的度数
        float halfAngle = angle * 0.5f; // 每半个角的度数
        float halfWidth = Width * 0.5f; // Quad的半个边长
        // 正切函数计算半径
        float radius = halfWidth / Mathf.Tan(Mathf.Deg2Rad * halfAngle);
        m_radius = radius;
    }

    private void CalculateTargetStateInfo(GameObject parent, ref List<Vector3> positions, ref List<Quaternion> rotations)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            positions.Add(parent.transform.GetChild(i).position);
            rotations.Add(parent.transform.GetChild(i).rotation);
        }
    }
    #endregion

    private void GenerateRow(Vector3 centerPos)
    {
        #region 直接生成父物体

        GameObject row = null;
        if (m_tubeState == TubeState.Ring)
        {
            row = Instantiate(RingPrefab, centerPos, Quaternion.identity);
        }
        else if (m_tubeState == TubeState.Plane)
        {
            row = Instantiate(PlanePrefab, centerPos, Quaternion.identity);
        }
        else if(m_tubeState == TubeState.RingToPlane)
        {
            row = Instantiate(RingPrefab, centerPos, Quaternion.identity);
        }
        else if (m_tubeState == TubeState.PlaneToRing)
        {
            row = Instantiate(PlanePrefab, centerPos, Quaternion.identity);
        }

        if(row != null)
        {
            row.transform.SetParent(this.transform);
            m_rows.Add(row);
        }
        else
        {
            print("枚举值不存在");
        }
        #endregion

        #region 12个面片成环
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
        #endregion

        #region 12个面片并排
        //float startX = centerPos.x - m_quadCount * Width / 2f;
        //for (int i = 0; i < m_quadCount; i++)
        //{
        //    Vector3 position = new Vector3(startX + i * Width, centerPos.y - 2.5f, centerPos.z);
        //    Quaternion rotation = Quaternion.LookRotation(Vector3.down);
        //    GameObject quad = Instantiate(QuadPrefab, position, rotation);
        //}
        #endregion
    }

    //根据传参设置位置和旋转
    private void SetQuad(int index ,Vector3 position, Quaternion rotation)
    {
        GameObject quad = Instantiate(QuadPrefab, position, rotation);
        quad.transform.parent = transform;
        m_rows.Add(quad);
    }

    public float MoveSpeed = 5f;
    private Queue<GameObject> m_waitDelete = new Queue<GameObject>();
    private Queue<float> m_waitGeneratePos = new Queue<float>();

    private void Update()
    {
        m_rows.ForEach((ring) =>
        {
            //全体位移
            ring.transform.position -= new Vector3(0, 0, MoveSpeed * Time.deltaTime);

            //末尾面片待删除
            if(ring.transform.position.z < -1)
            {
                m_waitDelete.Enqueue(ring);
                m_waitGeneratePos.Enqueue(ring.transform.position.z + (SumLength - 1) * Width);
            }
        });

        //删除待删除面片
        while (m_waitDelete.Count > 0)
        {
            GameObject quad = m_waitDelete.Dequeue();
            m_rows.Remove(quad);
            Destroy(quad);
        }

        //生成对应面片
        while (m_waitGeneratePos.Count > 0)
        {
            float z = m_waitGeneratePos.Dequeue();
            GenerateRow(new Vector3(transform.position.x, transform.position.y, z));
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(m_tubeState == TubeState.Ring)
            {
                print("Ring变换Plane");
                SetTubeStateWithDuration(TubeState.Plane);
            }
            else if (m_tubeState == TubeState.Plane)
            {
                print("Plane变换Ring");
                SetTubeStateWithDuration(TubeState.Ring);
            }
            else//TODO:环形分正反两面
            {

            }
        }
    }

    private void SetTubeStateWithDuration(TubeState targetState)
    {
        if (targetState == TubeState.Ring)
        {
            m_tubeState = TubeState.PlaneToRing;
            StartCoroutine(DoTubeTransform(targetState, RingPrefab));
        }
        else if (targetState == TubeState.Plane)
        {
            m_tubeState = TubeState.RingToPlane;
            StartCoroutine(DoTubeTransform(targetState, PlanePrefab));
        }
        else//TODO:环形分正反两面
        {

        }
    }

    /// <summary>
    /// 这种方案需要Prefab中子物体的索引一一对应
    /// </summary>
    /// <param name="targetState"></param>
    /// <param name="targetObj"></param>
    /// <returns></returns>
    private IEnumerator DoTubeTransform(TubeState targetState, GameObject targetObj)
    {
        float m_lerpNum = 0;
        while (m_lerpNum < 1f)
        {
            yield return null;

            //应用在全部面片上，看起来就是隧道整体在变换
            for (int i = 0; i < m_rows.Count; i++)
            {
                for (int j = 0; j < m_rows[i].transform.childCount; j++)
                {
                    var targetQuad = m_rows[i].transform.GetChild(j);
                    targetQuad.localPosition =
                        Vector3.Lerp(targetQuad.localPosition, targetObj.transform.GetChild(j).localPosition, m_lerpNum);
                    targetQuad.localRotation =
                        Quaternion.Lerp(targetQuad.localRotation, targetObj.transform.GetChild(j).localRotation, m_lerpNum);
                }
            }
            m_lerpNum += LerpPerAdd;
        }

        m_tubeState = targetState;
    }

    private void OnGUI()
    {
        
    }

}