using UnityEngine;

class Velocity : MonoBehaviour
{
    public Vector3 Momentum = new();

    public CharacterController? CharacterController;

    private void Update()
    {
        if (CharacterController)
            CharacterController.Move(Momentum * Time.deltaTime);
    }

}