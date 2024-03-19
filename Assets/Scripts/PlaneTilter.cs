using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneTilter : MonoBehaviour
{
    public float maxTiltAngle = 15f; // Maximum tilt angle in degrees
    public LayerMask objectsAffectingTiltLayer; // Layer containing the objects that will affect the tilt

    private Rigidbody planeRigidbody;

    void Start()
    {
        planeRigidbody = GetComponent<Rigidbody>();
        // Ensure the plane's Rigidbody doesn't go to sleep (which would stop the tilting effect)
        planeRigidbody.sleepThreshold = 0.0f;
    }

    void FixedUpdate()
    {
        // Calculate the center of mass of all objects
        Vector3 com = CalculateCenterOfMass();

        // Apply the torque to the plane to tilt it
        ApplyTorqueBasedOnCOM(com);
    }

    Vector3 CalculateCenterOfMass()
    {
        // Get all colliders on the specified layer within the plane's bounds
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, objectsAffectingTiltLayer);
        Vector3 com = Vector3.zero;
        float totalMass = 0f;

        foreach (Collider col in colliders)
        {
            if (col.attachedRigidbody != null)
            {
                com += col.attachedRigidbody.worldCenterOfMass * col.attachedRigidbody.mass;
                totalMass += col.attachedRigidbody.mass;
            }
        }

        if (totalMass > 0)
        {
            com /= totalMass;
            // Transform the center of mass to local space relative to the plane
            com = transform.InverseTransformPoint(com);
        }

        return com;
    }

    void ApplyTorqueBasedOnCOM(Vector3 com)
    {
        // Calculate the desired tilt angle based on the center of mass along the x-axis
        float tiltAroundZ = -maxTiltAngle * (com.x / transform.localScale.x);

        // Desired rotation in local space around the z-axis for left/right tilt
        Quaternion targetLocalRotation = Quaternion.Euler(0, 0, tiltAroundZ);
        // Desired rotation in world space
        Quaternion targetWorldRotation = transform.rotation * targetLocalRotation;

        // Calculate torque needed to achieve the target rotation
        Quaternion torqueQuaternion = targetWorldRotation * Quaternion.Inverse(transform.rotation);
        torqueQuaternion.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180) angle -= 360; // Ensure the smallest rotation is taken (angle is always < 180)
        angle *= Mathf.Deg2Rad; // Convert angle to radians for torque calculation
        axis.Normalize();
        Vector3 torque = axis * angle;

        // Since we want to tilt left/right only, we only apply torque around the plane's local Z-axis
        Vector3 localTorque = transform.InverseTransformDirection(torque);
        localTorque.x = 0; // No tilting forward/backward
        localTorque.y = 0; // No tilting sideways (yaw)
        torque = transform.TransformDirection(localTorque); // Convert back to world space

        // Apply the torque
        planeRigidbody.AddTorque(torque, ForceMode.VelocityChange);
    }
}
