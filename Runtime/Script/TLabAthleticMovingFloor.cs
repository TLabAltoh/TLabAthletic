using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TLabAthleticMovingFloor : MonoBehaviour
{
    [Header("Speed to reach the next station [m/s]")]
    public float m_speed;

    [Header("Show Gizmo")]
    public bool m_showGizmo;

    [Header("Floor Transition Infos")]
    public TransitionInfo[] m_transitionInfos;

    private Transform[] m_anchorPoints;
    private Transform m_currentTarget;
    private Transform m_prevTarget;
    private Rigidbody m_rb;
    private TransitionInfo m_currentTranstionInfo;
    private int m_anchorIndex;
    private int m_transtionInfoIndex;
    private float m_remain;
    private float m_distance;

    public Vector3 FloorVelocity
    {
        get
        {
            return m_rb.GetPointVelocity(TLabThirdPersonController.Instance.transform.position);
        }
    }

    private void SetNextStation()
    {
        m_prevTarget = m_currentTarget;
        m_anchorIndex = (m_anchorIndex + 1) % m_anchorPoints.Length;
        m_currentTarget = m_anchorPoints[m_anchorIndex];
        m_distance = (m_prevTarget.position - m_currentTarget.position).magnitude;

        if(m_currentTranstionInfo.index == m_anchorIndex)
        {
            m_remain = m_currentTranstionInfo.remain;
            m_transtionInfoIndex = (m_transtionInfoIndex + 1) % m_transitionInfos.Length;
            m_currentTranstionInfo = m_transitionInfos[m_transtionInfoIndex];
        }
        else
        {
            m_remain = m_distance;
        }
    }

    private void ApproachToTarget()
    {
        // https://docs.unity3d.com/jp/2018.4/ScriptReference/Rigidbody.MovePosition.html
        float ratioA = m_remain / m_distance;
        float ratioB = 1 - ratioA;
        m_rb.MovePosition(m_prevTarget.position * ratioA + m_currentTarget.position * ratioB);
        m_rb.MoveRotation(Quaternion.Euler(m_prevTarget.rotation.eulerAngles * ratioA + m_currentTarget.rotation.eulerAngles * ratioB));
    }

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            m_rb = this.gameObject.AddComponent<Rigidbody>();
        m_rb.isKinematic = true;
        m_rb.interpolation = RigidbodyInterpolation.Interpolate;

        Transform[] inChildren = GetComponentsInChildren<Transform>();
        m_anchorPoints = new Transform[inChildren.Length - 1];

        GameObject m_anchors = new GameObject(this.gameObject.name);
        m_anchors.transform.parent = this.transform.parent;

        int index = 0;
        foreach (Transform anchor in inChildren)
        {
            if(anchor != this.transform)
            {
                anchor.parent = m_anchors.transform;
                m_anchorPoints[index++] = anchor;
            }
        }

        m_anchorIndex = m_anchorPoints.Length - 1;
        m_currentTarget = m_anchorPoints[m_anchorIndex];

        m_transtionInfoIndex = 0;
        if(m_transitionInfos.Length > 0)
        {
            m_currentTranstionInfo = m_transitionInfos[m_transtionInfoIndex];
        }
        else
        {
            m_currentTranstionInfo = new TransitionInfo();
        }

        SetNextStation();
    }

    void FixedUpdate()
    {
        if (m_remain > 0.0f)
        {
            m_remain -= Time.deltaTime * m_speed;
        }

        if (m_remain <= 0.0f)
            SetNextStation();
        else
            ApproachToTarget();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying || m_showGizmo == false) return;

        Transform[] inChildren = GetComponentsInChildren<Transform>();

        // https://gametukurikata.com/customize/sceneview/handles
        Handles.color = new Color(1f, 1f, 0f, 1f);
        Vector3[] points = new Vector3[inChildren.Length - 1];

        int index = 0;
        foreach (Transform anchor in inChildren)
        {
            if (anchor != this.transform)
            {
                points[index++] = anchor.position;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(anchor.position, 0.1f);
                Gizmos.DrawWireSphere(anchor.position, 0.5f);

                Gizmos.DrawLine(anchor.position, anchor.position + anchor.up);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(anchor.position, anchor.position + anchor.forward);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(anchor.position, anchor.position + anchor.right);
            }
        }

        Handles.DrawPolyLine(points);
    }
#endif
}

[System.Serializable]
public class TransitionInfo
{
    public TransitionInfo() { }

    public int index = -1;
    public float remain = -1f;
}