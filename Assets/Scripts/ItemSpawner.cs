using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Transform itemParent;

    [SerializeField] private Vector2 moveTarget;
    [SerializeField] private LayerMask moveLayer;
    [SerializeField] private Transform[] camPos;
    [SerializeField] Transform worldMin;
    [SerializeField] Transform worldMax;
    
    const int MaxSampleCount = 100;
    
    private void Start()
    {
        NewTarget(MaxSampleCount);
        StartCoroutine(SpawnCoroutine());

        for (var i = 0; i < 50; i++)
        {
            NewTargetInWorld(MaxSampleCount);
            Spawn();
        }
    }

    private void NewTarget(int tryCount)
    {
        while (true)
        {
            if (tryCount < 0)
            {
                Debug.LogError("Maximum sampling count exceeded.");
                return;
            }

            moveTarget = Frog.Instance.NewSpawnPosition();

            if (!Physics2D.OverlapCircle(moveTarget, 2, moveLayer))
            {
                tryCount--;
                continue;
            }

            break;
        }
    }

    void NewTargetInWorld(int tryCount)
    {
        while (true)
        {
            if (tryCount < 0)
            {
                Debug.LogError("Maximum sampling count exceeded.");
                return;
            }

            var worldMinPos = worldMin.position;
            var worldMaxPos = worldMax.position;

            moveTarget = new Vector2(Random.Range(worldMinPos.x, worldMaxPos.x), Random.Range(worldMinPos.y, worldMaxPos.y));

            if (!Physics2D.OverlapCircle(moveTarget, 2, moveLayer))
            {
                tryCount--;
                continue;
            }

            break;
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        Spawn();
        yield return new WaitForBalanceSeconds(BalancePlanner.Instance.CurrentPlan.RandomSpawnItemInterval);
        NewTarget(MaxSampleCount);
        StartCoroutine(SpawnCoroutine());
    }

    void Spawn()
    {
        var prefab = BalancePlanner.Instance.CurrentPlan.RandomSpawnItem;
        Instantiate(prefab, moveTarget, prefab.transform.localRotation, itemParent);
    }
}