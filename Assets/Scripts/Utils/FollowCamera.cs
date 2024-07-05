using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform TargetToFollow = null;
    [SerializeField] Vector3 Offset = Vector3.zero;
    [SerializeField] Vector3 TransitionOffset = Vector3.zero;

    Vector3 target = Vector3.zero;

    private GameManager gameManager = null;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (gameManager.Transition)
        {
            target = new Vector3(transform.position.x, TransitionOffset.y, TargetToFollow.position.z + TransitionOffset.z);
            transform.position = Vector3.Lerp(transform.position, target, 10 * Time.deltaTime);
            return;
        }

        target = new Vector3(transform.position.x, Offset.y, TargetToFollow.position.z + Offset.z);
        transform.position = target;
    }
}
