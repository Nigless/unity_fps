using ExtensionMethods;
using UnityEngine;

class JumpPad : MonoBehaviour
{
    public float Force = 5;

    private void OnHit(GameObject obj)
    {
        var body = obj.GetComponent<Rigidbody>();

        if (body != null)
        {
            if (transform.up.Dot(body.velocity.normalized) >= 0)
                body.velocity = Vector3.ProjectOnPlane(body.velocity, transform.up);

            body.velocity += transform.up * Force;
            return;
        }


        var velocity = obj.GetComponent<Velocity>();

        if (velocity != null)
        {
            if (transform.up.Dot(velocity.Momentum.normalized) >= 0)
                velocity.Momentum = Vector3.ProjectOnPlane(velocity.Momentum, transform.up);

            velocity.Momentum += transform.up * Force;
            return;
        }
    }
}