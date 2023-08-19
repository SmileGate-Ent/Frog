using System;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public static Frog Instance;
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

    [SerializeField] int hp;
    [SerializeField] int score;

    [SerializeField] AnimationCurve jumpHeightCurve;
    [SerializeField] float jumpTotalDuration = 0.5f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] Transform frogSprite;
    [SerializeField] float jumpMoveRatioInCurve = 0.9f;
    [SerializeField] Transform shadowPivot;
    
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip tongueClip;
    [SerializeField] AudioClip scoreClip;

    float jumpCurrentDuration;
    bool isJump;
    Vector2 moveDeltaDuringJump;

    float JumpNormalizedDuration => jumpCurrentDuration / jumpTotalDuration;

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            FrogCanvas.Instance.HpText = $"HP: {Hp}";
        }
    }
    public int Score
    {
        get => score;
        set
        {
            score = value;
            FrogCanvas.Instance.ScoreText = $"Score: {Score}";
        }
    }
 
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");
        if ((dx != 0 || dy != 0) && isJump == false)
        {
            moveDeltaDuringJump = new Vector2(dx, dy).normalized;

            isJump = true;
            jumpCurrentDuration = 0;
            sfxAudioSource.PlayOneShot(jumpClip);
        }

        if (isJump)
        {
            if (JumpNormalizedDuration < jumpMoveRatioInCurve)
            {
                transform.Translate(moveDeltaDuringJump * (moveSpeed * Time.deltaTime), Space.Self);
            }

            jumpCurrentDuration += Time.deltaTime;
            if (jumpCurrentDuration >= jumpTotalDuration)
            {
                isJump = false;
                jumpCurrentDuration = 0;
            }
        }

        // 점프에 의한 개구리 스프라이트 위치 조절
        var frogSpriteLocalPos = frogSprite.transform.localPosition;
        var jumpHeightCurveVal = jumpHeightCurve.Evaluate(JumpNormalizedDuration);
        frogSpriteLocalPos.y = jumpHeightCurveVal * jumpHeight;
        frogSprite.transform.localPosition = frogSpriteLocalPos;
        
        // 점프에 의한 그림자 크기 조절
        shadowPivot.localScale = Vector3.one * (1.0f - jumpHeightCurveVal * 0.3f);
        
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
            
            sfxAudioSource.PlayOneShot(tongueClip);

            // 씬에 프리팹 게임 오브젝트를 클릭한 위치에 생성합니다.
            //Instantiate(tongueTargetPrefab, worldPoint, Quaternion.identity);
        }
        
        var tongueLocalScale = tongue.size; //.transform.localScale;
        tongueLocalScale.x = Mathf.SmoothDamp(tongueLocalScale.x, tongueTargetLength, ref tongueVelocity, tongueSmoothTime);
        //tongue.transform.localScale = tongueLocalScale;
        tongue.size = tongueLocalScale;

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

    public void PlayScoreClip()
    {
        sfxAudioSource.PlayOneShot(scoreClip);
    }

    public void AttachItemToTongue(Item item)
    {
        item.transform.SetParent(tongueTip);
    }
}
