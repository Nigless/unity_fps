using UnityEngine;

public class LookingPart : PartMachine<Player>.Part
{


    public LookingPart(Player Context) : base(Context)
    {
    }

    public override void Update()
    {

        var rotation = _Context.Head.transform.localRotation * Quaternion.Euler(-_Context.Looking.y * _Context.MouseSensitivity, 0f, 0f);


        if ((rotation * Vector3.up).y < 0)
        {
            if ((rotation * Vector3.forward).y >= 0)
                rotation = Quaternion.Euler(-90f, 0f, 0f);
            else rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        _Context.Head.transform.localRotation = rotation;

        _Context.transform.Rotate(Vector3.up * _Context.Looking.x * _Context.MouseSensitivity);
    }

}