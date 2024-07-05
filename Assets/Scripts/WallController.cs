using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] List<WallMovement> Walls = new List<WallMovement>();

    private GameObject lastCurrentWallRight = null;
    private GameObject lastCurrentWallLeft = null;
    private bool rightOrLeft = false;

    void Start()
    {
        for (int i = 0; i < Walls.Count; ++i)
            Walls[i].OnReplaceWall += ReplaceWall;

        lastCurrentWallRight = Walls[Walls.Count - 2].gameObject;
        lastCurrentWallLeft = Walls[Walls.Count - 1].gameObject;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < Walls.Count; ++i)
            Walls[i].OnReplaceWall -= ReplaceWall;
    }

    private void ReplaceWall(WallMovement wall)
    {
        int index = Walls.IndexOf(wall);
        if (rightOrLeft)
        {
            Walls[index].ReplaceWall(lastCurrentWallRight);
            lastCurrentWallRight = Walls[index].gameObject;
            rightOrLeft = !rightOrLeft;
        }
        else
        {
            Walls[index].ReplaceWall(lastCurrentWallLeft);
            //Walls[index].transform.localPosition = new Vector3(Walls[index].transform.localPosition.x, 0.0f, lastCurrentWallLeft.transform.position.z + Walls[index].transform.localScale.z - 0.3f);
            lastCurrentWallLeft = Walls[index].gameObject;
            rightOrLeft = !rightOrLeft;
        }
    }
}
