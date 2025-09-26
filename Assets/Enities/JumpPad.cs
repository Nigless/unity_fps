using ExtensionMethods;
using UnityEngine;

class JumpPad : MonoBehaviour
{
    public float Force = 5;

    private void OnCollisionEnter(Collision collision)
    {
        var rigidBody = collision.gameObject.GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            if (transform.up.Dot(rigidBody.velocity.normalized) >= 0)
                rigidBody.velocity = Vector3.ProjectOnPlane(rigidBody.velocity, transform.up);

            rigidBody.velocity += transform.up * Force;
            return;
        }
    }
}