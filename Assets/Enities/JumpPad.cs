using ExtensionMethods;
using UnityEngine;

class JumpPad : MonoBehaviour
{
    public float Force = 5;

    private void OnCollisionEnter(Collision collision)
    {
        var body = collision.gameObject.GetComponent<Rigidbody>();

        if (body != null)
        {
            if (transform.up.Dot(body.velocity.normalized) >= 0)
                body.velocity = Vector3.ProjectOnPlane(body.velocity, transform.up);

            body.velocity += transform.up * Force;
            return;
        }
    }
}