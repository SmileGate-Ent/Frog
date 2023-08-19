using System;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Frog : MonoBehaviour
{
    public static Frog Instance;
    [SerializeField] float moveSpeed = 10;

    [SerializeField] GameObject tongueTargetPrefab; // 생성할 게임 오브젝트 프리팹입니다.

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

    [SerializeField] Transform frogSpritePivot;


    [SerializeField] float jumpMoveRatioInCurve = 0.9f;
    [SerializeField] Transform shadowPivot;

    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip tongueClip;
    [SerializeField] AudioClip scoreClip;
    
    [SerializeField] LayerMask layers;
    [SerializeField] GameObject dieWater;
    [SerializeField] GameObject[] frogpivot;

    [SerializeField] SpriteRenderer frogSprite;

    [SerializeField] Sprite frogIdleSprite;
    [SerializeField] Sprite frogJumpSprite;

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

    void OnDrawGizmos()
    {
        if (tongueTargetPos != null)
        {
            Gizmos.DrawSphere(tongueTargetPos.Value, 0.5f);
        }
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

            frogSprite.sprite = frogJumpSprite;
        }

        if (isJump)
        {
            if (JumpNormalizedDuration < jumpMoveRatioInCurve)
            {
                transform.Translate(moveDeltaDuringJump * (moveSpeed * Time.deltaTime), Space.Self);
            }
            else
            {
                frogSprite.sprite = frogIdleSprite;
            }

            jumpCurrentDuration += Time.deltaTime;
            if (jumpCurrentDuration >= jumpTotalDuration)
            {
                if (!Physics2D.OverlapCircle(transform.position, 2,layers))
                {
                    var a = Instantiate(dieWater, transform.position,quaternion.identity);
                    Destroy(a, 1f);
                    Invoke("ReSpawn",3f);
                    frogpivot[0].SetActive(false);
                    frogpivot[1].SetActive(false);
                }
                isJump = false;
                jumpCurrentDuration = 0;
                
                
            }
        }

        // 개구리 이미지 좌우 뒤집기
        if (moveDeltaDuringJump.x != 0)
        {
            var frogSpritePivotScale = frogSpritePivot.localScale;
            frogSpritePivotScale.x = moveDeltaDuringJump.x > 0 ? 1 : -1;
            frogSpritePivot.localScale = frogSpritePivotScale;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var popup = FrogCanvas.Instance.InstantiateConfirmPopup();
            popup.Text = "종료하시겠습니까?";
            popup.Btn1Text = "예";
            popup.Btn2Text = "아니요";
            popup.OnBtn1 = () => SceneManager.LoadScene("TitleScene");
            popup.OnBtn2 = () => Destroy(popup.gameObject);
        }

        // 점프에 의한 개구리 스프라이트 위치 조절
        var frogSpriteLocalPos = frogSpritePivot.transform.localPosition;
        var jumpHeightCurveVal = jumpHeightCurve.Evaluate(JumpNormalizedDuration);
        frogSpriteLocalPos.y = jumpHeightCurveVal * jumpHeight;
        frogSpritePivot.transform.localPosition = frogSpriteLocalPos;

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
            Instantiate(tongueTargetPrefab, worldPoint, Quaternion.identity);
        }

        var tongueLocalScale = tongue.size; //.transform.localScale;
        tongueLocalScale.x =
            Mathf.SmoothDamp(tongueLocalScale.x, tongueTargetLength, ref tongueVelocity, tongueSmoothTime);
        //tongue.transform.localScale = tongueLocalScale;
        tongue.size = tongueLocalScale;

        var tongueLocalPos = tonguePos.localPosition;
        tongueLocalPos.z = tongueLocalScale.x;
        tonguePos.localPosition = tongueLocalPos;

        if (tongueTargetPos != null)
        {
            tongueTargetLength = Vector3.Distance(tonguePivot.position, tongueTargetPos.Value) * 2;

            tonguePivot.transform.LookAt(tongueTargetPos.Value, Vector3.forward);

            // 목표하는 혀 길이보다 0.1f보다 조금 짧은 순간이 오면 다시 혀를 말아 들인다.
            if (Vector3.Distance(tongueTip.position, tongueTargetPos.Value) < 0.2f
                )
            {
                tongueTargetLength = 0;
                tongueTargetPos = null;
            }
        }
    }

    private void ReSpawn()
    {
        frogpivot[0].SetActive(true);
        frogpivot[1].SetActive(true);
        transform.position = Vector3.zero;
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