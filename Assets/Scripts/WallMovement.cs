using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    [SerializeField] float DistanceFromPlayerToRespawn = 100.0f;

    private float speed = 0.0f;

    internal System.Action<WallMovement> OnReplaceWall;

    private GameManager gameManager = null;

    private void Start()
    {
        gameManager = GameManager.Instance;

        speed = DataManager.Instance.Data.WallSpeed;
    }

    void Update()
    {
        transform.position -= Vector3.forward * speed * Time.deltaTime;

        Vector3 playerPosition = gameManager.Player.transform.position;
        if (transform.position.z < playerPosition.z - DistanceFromPlayerToRespawn)
            OnReplaceWall?.Invoke(this);
    }

    internal void ReplaceWall(GameObject lastWall)
    {
         transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z + transform.localScale.z * 4.0f);
    }
}
