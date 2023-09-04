using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject monster;
    [SerializeField] private Vector2 moveTarget;
    [SerializeField] private LayerMask moveLayer;
    [SerializeField] private Transform[] camPos;

    const int MaxSampleCount = 100;

    private void Start()
    {
        NewTarget(MaxSampleCount);
        StartCoroutine(SpawnCoroutine());
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

    private IEnumerator SpawnCoroutine()
    {
        Instantiate(monster, moveTarget, Quaternion.identity);
        yield return new WaitForBalanceSeconds(BalancePlanner.Instance.CurrentPlan.RandomSpawnEnemyInterval);
        NewTarget(MaxSampleCount);
        StartCoroutine(SpawnCoroutine());
    }
}