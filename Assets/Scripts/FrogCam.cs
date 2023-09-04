using UnityEngine;

public class FrogCam : MonoBehaviour
{
    [SerializeField] Camera cam;
    // Right, Left, Up, Down 순서
    [SerializeField] Transform[] camSides;

    public Camera Cam => cam;

    // 카메라 시야 밖 어딘가를 하나 뽑는다.
    public Vector2 NewSpawnPosition()
    {
        Vector2 moveTarget;
        if (Random.Range(0, 2) == 1)
        {
            moveTarget = new Vector2(camSides[Random.Range(0, 2)].position.x + Random.Range(-5f, 5f),
                Random.Range(-20f, 20f));
        }
        else
        {
            moveTarget = new Vector2(Random.Range(-25f, 25f),
                camSides[Random.Range(2, 4)].position.y + Random.Range(-5f, 5f));
        }

        return moveTarget;
    }
}