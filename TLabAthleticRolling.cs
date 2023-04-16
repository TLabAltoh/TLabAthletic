using UnityEngine;

public class TLabAthleticRolling : MonoBehaviour
{
    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (meshCollider == null) return;

        meshCollider.convex = true;

        m_rb = gameObject.GetComponent<Rigidbody>();
        if (m_rb == null)
            m_rb = gameObject.AddComponent<Rigidbody>();

        m_rb.useGravity = false;

        Vector3 randomAxis = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;

        m_rb.angularDrag = 0.0f;
        m_rb.angularVelocity = randomAxis * 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
