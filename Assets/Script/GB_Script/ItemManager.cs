using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Transform itemParent;

    [SerializeField] private Vector2 moveTarget;
    [SerializeField] private LayerMask moveLayer;
    [SerializeField] private Transform[] camPos;

    private void Start()
    {
        NewTarget();
        StartCoroutine(SpawnCoroutine());
    }

    private void NewTarget()
    {
        if (Random.Range(0, 2) == 1)
        {
            moveTarget = new Vector2(camPos[Random.Range(0, 2)].position.x + Random.Range(-5f, 5f),
                Random.Range(-20f, 20f));
        }
        else
        {
            moveTarget = new Vector2(Random.Range(-25f, 25f),
                camPos[Random.Range(2, 4)].position.y + Random.Range(-5f, 5f));
        }

        if (!Physics2D.OverlapCircle(moveTarget, 2, moveLayer)) NewTarget();
    }

    private IEnumerator SpawnCoroutine()
    {
        var prefab = BalancePlanner.Instance.CurrentPlan.RandomSpawnItem;
        Instantiate(prefab, moveTarget, prefab.transform.localRotation, itemParent);
        yield return new WaitForSeconds(BalancePlanner.Instance.CurrentPlan.RandomSpawnItemInterval);
        NewTarget();
        StartCoroutine(SpawnCoroutine());
    }
}