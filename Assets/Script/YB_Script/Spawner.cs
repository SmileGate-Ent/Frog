using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject monster;
    [SerializeField] private Vector2 moveTarget;
    [SerializeField] private LayerMask moveLayer;
    
    private void Start()
    {
        NewTarget();
        StartCoroutine(SpawnCoroutine());
    }
    private void NewTarget()
    {
        moveTarget = new Vector2(Random.Range(-33f, 33f), Random.Range(-23f, 23f));
        if (!Physics2D.OverlapCircle(moveTarget , 2, moveLayer)) NewTarget();
    }
    private IEnumerator SpawnCoroutine()
    {
        Instantiate(monster,moveTarget, Quaternion.identity);
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        StartCoroutine(SpawnCoroutine());
    }

}
