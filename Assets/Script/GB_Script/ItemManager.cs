using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Transform itemParent;
    [SerializeField] Item[] itemPrefabList;

    IEnumerator Start()
    {
        while (Application.isPlaying)
        {
            Instantiate(itemPrefabList[Random.Range(0, itemPrefabList.Length)], new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), Quaternion.identity,
                itemParent);
            yield return new WaitForSeconds(1);
        }
    }
}