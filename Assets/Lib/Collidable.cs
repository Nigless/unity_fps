using UnityEngine;

class Collidable : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hit.gameObject.SendMessage("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);

    }

    private void OnCollisionStay(Collision collision)
    {
        collision.gameObject.SendMessage("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);

    }
}