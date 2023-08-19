using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject monster;
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
            moveTarget = new Vector2(camPos[Random.Range(0,2)].position.x + Random.Range(-5f, 5f), Random.Range(-20f, 20f));
        }
        else
        {
            moveTarget = new Vector2(Random.Range(-25f, 25f), camPos[Random.Range(2,4)].position.y + Random.Range(-5f, 5f));
        }
        if (!Physics2D.OverlapCircle(moveTarget , 2, moveLayer)) NewTarget();
    }
    private IEnumerator SpawnCoroutine()
    {
        Instantiate(monster,moveTarget, Quaternion.identity);
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        NewTarget();
        StartCoroutine(SpawnCoroutine());
    }

}
