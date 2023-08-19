using UnityEngine;
using UnityEngine.AI;

public class Find : MonoBehaviour
{
    [SerializeField] private Vector2 moveTarget;
    [SerializeField] private Vector2 movePosition;
    [SerializeField] private LayerMask moveLayer;
    [SerializeField] SpriteRenderer spriteRenderer;

    private NavMeshAgent _agent;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        NewTarget();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;    
    }

    private void NewTarget()
    {
        moveTarget = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        if (!Physics2D.OverlapCircle( new Vector2(transform.position.x + moveTarget.x, transform.position.y + moveTarget.y), 2, moveLayer)) NewTarget();
        movePosition = new Vector2(transform.position.x + moveTarget.x, transform.position.y + moveTarget.y) ;
    }
    
    void Update()
    {
        if (Vector3.Distance(movePosition, transform.position) > 3f)
        {
            _agent.SetDestination(movePosition);
        }
        else
        {
            NewTarget();
        }

        spriteRenderer.flipX = _agent.desiredVelocity.x > 0;
    }
}
