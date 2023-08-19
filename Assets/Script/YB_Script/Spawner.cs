using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] monsterPattern;
    [SerializeField] private Transform player;

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        Instantiate(monsterPattern[Random.Range(0,monsterPattern.Length)],  player.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        StartCoroutine(SpawnCoroutine());
    }

}
