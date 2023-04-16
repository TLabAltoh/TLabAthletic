using UnityEngine;
using StarterAssets;

public class TLabAthleticSeesaw : MonoBehaviour
{
    [Header("Seesaw tilt speed multiplier")]
    public float angularVelocityMultiplier;

    [Header("Player Grounded")]
    public bool m_playerGrounded;
    private Quaternion m_baseRotation;
    private Rigidbody m_rb;
    private float m_currentPitch;

    public Vector3 Velocity
    {
        get
        {
            return m_rb.GetPointVelocity(ThirdPersonController.Instance.transform.position);
        }
    }

    void Start()
    {
        m_baseRotation = this.transform.rotation;

        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
            m_rb = gameObject.AddComponent<Rigidbody>();

        m_rb.isKinematic = true;
        m_rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (m_playerGrounded)
        {
            Vector3 axis = Vector3.Cross(this.transform.right, (ThirdPersonController.Instance.transform.position - this.transform.position).normalized);
            float power = Vector3.Dot(this.transform.up, axis);

            m_currentPitch -= angularVelocityMultiplier * power * Time.deltaTime;

            m_rb.MoveRotation(m_baseRotation * Quaternion.Euler(m_currentPitch, 0.0f, 0.0f));
        }
        else
        {
            // m_currentPitch = Mathf.Lerp(m_currentPitch, 0.0f, Time.deltaTime);
        }

        m_playerGrounded = false;
    }
}
