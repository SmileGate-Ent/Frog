using UnityEngine;

public class Frog : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    
    [SerializeField] GameObject tongueTargetPrefab; // 생성할 게임 오브젝트 프리팹입니다.

    [SerializeField] Camera cam;

    [SerializeField] SpriteRenderer tongue;
    [SerializeField] Transform tonguePivot;

    void Update()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");
        if (dx != 0 || dy != 0)
        {
            var d = new Vector2(dx, dy).normalized;

            transform.Translate(d * (moveSpeed * Time.deltaTime), Space.Self);
        }
        
        // 왼쪽 마우스 버튼이 클릭되었을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스의 스크린 좌표를 가져옵니다.
            var mousePos = Input.mousePosition;
            
            // 스크린 좌표에서 이러한 월드 좌표를 얻습니다.
            var worldPoint = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            worldPoint.z = 0; // 생성하려는 게임 오브젝트의 z축 위치를 조정합니다.

            // 씬에 프리팹 게임 오브젝트를 클릭한 위치에 생성합니다.
            Instantiate(tongueTargetPrefab, worldPoint, Quaternion.identity);

            var tongueSize = tongue.size;
            tongueSize.x = Vector3.Distance(transform.position, worldPoint);
            tongue.size = tongueSize;
            tonguePivot.transform.LookAt(worldPoint, Vector3.forward);
        }
    }
}
