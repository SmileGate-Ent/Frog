using UnityEngine;

[CreateAssetMenu(menuName = "Frog/Balance Plan")]
public class BalancePlan : ScriptableObject
{
    // 밸런스 적용될 시간 (초)
    [SerializeField] float applyTime;
    
    // 생성할 아이템 목록
    [SerializeField] Item[] spawnItemList;

    // 아이템 생성 간격 (초)
    [SerializeField] float spawnItemInterval;

    // 자연 감소 HP
    [SerializeField] float damageOverTime;
    
    // 자연 감소 간격 (초)
    [SerializeField] float damageOverTimeInterval;

    // 적 생성 간격 (초)
    [SerializeField] float enemySpawnInterval;

    public float ApplyTime => applyTime;
    public Item[] SpawnItemList => spawnItemList;
    public float SpawnItemInterval => spawnItemInterval;
    public float DamageOverTime => damageOverTime;
    public float DamageOverTimeInterval => damageOverTimeInterval;
    public float EnemySpawnInterval => enemySpawnInterval;
}
