using System;
using UnityEngine;

public class Frog : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    
    //[SerializeField] GameObject tongueTargetPrefab; // 생성할 게임 오브젝트 프리팹입니다.

    [SerializeField] Camera cam;

    [SerializeField] SpriteRenderer tongue;
    [SerializeField] Transform tonguePivot;
    [SerializeField] Transform tonguePos;

    float tongueTargetLength;
    float tongueVelocity;
    [SerializeField] float tongueSmoothTime = 0.1f;
    Vector2? tongueTargetPos;
    float tongueTargetFirstLength;
    [SerializeField] Transform tongueTip;

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

            tongueTargetPos = worldPoint;
            if (tongueTargetPos != null)
            {
                tongueTargetFirstLength = Vector3.Distance(transform.position, tongueTargetPos.Value);
            }

            // 씬에 프리팹 게임 오브젝트를 클릭한 위치에 생성합니다.
            //Instantiate(tongueTargetPrefab, worldPoint, Quaternion.identity);
        }
        
        var tongueLocalScale = tongue.transform.localScale;
        tongueLocalScale.x = Mathf.SmoothDamp(tongueLocalScale.x, tongueTargetLength, ref tongueVelocity, tongueSmoothTime);
        tongue.transform.localScale = tongueLocalScale;

        var tongueLocalPos = tonguePos.localPosition;
        tongueLocalPos.z = tongueLocalScale.x;
        tonguePos.localPosition = tongueLocalPos;
        
        if (tongueTargetPos != null)
        {
            tongueTargetLength = Vector3.Distance(transform.position, tongueTargetPos.Value);

            tonguePivot.transform.LookAt(tongueTargetPos.Value, Vector3.forward);

            // 목표하는 혀 길이보다 0.1f보다 조금 짧은 순간이 오면 다시 혀를 말아 들인다.
            if (Vector3.Distance(tongueTip.position, tongueTargetPos.Value) < 0.1f
                || tongueLocalScale.x > tongueTargetFirstLength)
            {
                tongueTargetLength = 0;
                tongueTargetPos = null;
            }
        }
    }
}
