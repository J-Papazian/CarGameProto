using UnityEngine;

public class CompenseRotation : MonoBehaviour
{
    [SerializeField] Vector3 Rotation = Vector3.zero;

    void Update()
    {
        if (Rotation == Vector3.zero)
            transform.rotation = Quaternion.identity;
        else
            transform.rotation = Quaternion.Euler(Rotation);
    }
}
