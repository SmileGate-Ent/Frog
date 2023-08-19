using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Transform itemParent;
    [SerializeField] GameObject itemPrefab;

    IEnumerator Start()
    {
        while (Application.isPlaying)
        {
            Instantiate(itemPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), Quaternion.identity,
                itemParent);
            yield return new WaitForSeconds(1);
        }
    }
}