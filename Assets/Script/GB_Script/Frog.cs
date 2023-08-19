using System.Collections;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Frog : MonoBehaviour
{
    public static Frog Instance;
    [SerializeField] float moveSpeed = 10;

    [SerializeField] GameObject tongueTargetPrefab; // 생성할 게임 오브젝트 프리팹입니다.

    [SerializeField] Camera cam;

    [SerializeField] float tongueScale = 1.0f / 3;

    [SerializeField] SpriteRenderer tongue;
    [SerializeField] Transform tonguePivot;
    [SerializeField] Transform tonguePos;

    float tongueTargetLength;
    float tongueVelocity;
    [SerializeField] float tongueSmoothTime = 0.1f;
    Vector2? tongueTargetPos;
    float tongueTargetFirstLength;
    [SerializeField] Transform tongueTip;

    [SerializeField] float hp;
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
    [SerializeField] AudioClip damageClip;
    
    [SerializeField] LayerMask layers;
    [SerializeField] GameObject dieWater;
    [SerializeField] GameObject[] frogpivot;

    [SerializeField] SpriteRenderer frogSprite;

    [SerializeField] GameObject frogMouth;

    [SerializeField] private AnimationCurve hpCurve;
    [SerializeField] private int hpTime;
    [SerializeField] private Slider hpSlider;
    
    
    [SerializeField] private Slider debuffSlider;

    [SerializeField] CharacterPreset preset;

    [SerializeField] SpriteRenderer idleSpriteRenderer;
    [SerializeField] SpriteRenderer attackSpriteRenderer;
    [SerializeField] SpriteRenderer jumpSpriteRenderer;
    
    [SerializeField] GameObject gameOverPopup;

    float jumpCurrentDuration;
    bool isJump;
    bool isDie;
    Vector2 moveDeltaDuringJump;

    enum SpriteState
    {
        Idle,
        Attack,
        Jump,
    }

    SpriteState spriteState;

    public CharacterPreset Preset
    {
        set => preset = value;
    }

    float JumpNormalizedDuration => jumpCurrentDuration / jumpTotalDuration;

    public bool CanCatch => tongueTargetLength > 0;

    public float Hp
    {
        get => hp;
        set => hp = value;
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
        StartCoroutine(HpCalculation());
        tonguePivot.localScale = Vector3.one * tongueScale;

        spriteState = SpriteState.Idle;
    }

    void OnDrawGizmos()
    {
        if (tongueTargetPos != null)
        {
            Gizmos.DrawSphere(tongueTargetPos.Value, 0.5f);
        }
    }

    IEnumerator HpCalculation()
    {
        Hp -= hpCurve.Evaluate(hpTime);
        if (hp <= 0 && isDie == false)
        {
            Die();
        }
        yield return new WaitForSeconds(1f);
        hpTime++;
        StartCoroutine(HpCalculation());
    }
    
    void Update()
    {
        debuffSlider.transform.position = cam.WorldToScreenPoint(transform.position + new Vector3(0, -1.5f, 0));
        
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");
        if ((dx != 0 || dy != 0) && isJump == false)
        {
            moveDeltaDuringJump = new Vector2(dx, dy).normalized;

            isJump = true;
            jumpCurrentDuration = 0;
            sfxAudioSource.PlayOneShot(jumpClip);

            //frogSprite.sprite = frogJumpSprite;
            spriteState = SpriteState.Jump;
        }
        
        //Debug.Log($"isDie = {isDie}");

        if (isJump && !isDie)
        {
            if (JumpNormalizedDuration < jumpMoveRatioInCurve)
            {
                transform.Translate(moveDeltaDuringJump * (moveSpeed * Time.deltaTime), Space.Self);
            }
            else if (tongueTargetPos.HasValue == false) // 혀를 꺼내고 있는 도중에는 Idle로 돌아가지 않아야 한다.
            {
                //frogSprite.sprite = frogIdleSprite;
                //frogMouth.SetActive(false);
                spriteState = SpriteState.Idle;
            }

            jumpCurrentDuration += Time.deltaTime;
            if (jumpCurrentDuration >= jumpTotalDuration)
            {
                if (!Physics2D.OverlapCircle(transform.position, 2, layers))
                {
                    Debug.Log("Instantiate dieWater");
                    var a = Instantiate(dieWater, transform.position, quaternion.identity);
                    Destroy(a, 1f);
                    Die();
                }

                isJump = false;
                jumpCurrentDuration = 0;
            }
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
        
        var mousePos = Input.mousePosition;
        
        // 스크린 좌표에서 이러한 월드 좌표를 얻습니다.
        var worldPoint = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        worldPoint.z = 0; // 생성하려는 게임 오브젝트의 z축 위치를 조정합니다.

        // 개구리 이미지 좌우 뒤집기
        var frogSpritePivotScale = frogSpritePivot.localScale;

        // [이동 방향에 따른 것]
        if (moveDeltaDuringJump.x != 0)
        {
            frogSpritePivotScale.x = moveDeltaDuringJump.x > 0 ? 1 : -1;
            frogSpritePivotScale.x = transform.position.x < worldPoint.x ? 1 : -1;
            frogSpritePivot.localScale = frogSpritePivotScale;
        }
        
        // [마우스 위치에 따른 것]
        frogSpritePivotScale.x = transform.position.x < worldPoint.x ? 1 : -1;
        frogSpritePivot.localScale = frogSpritePivotScale;

        // 왼쪽 마우스 버튼이 클릭되었을 때
        // 점프 중일 때는 공격 시작 못한다. 
        if (Input.GetMouseButtonDown(0) && JumpNormalizedDuration <= 0 && debuffSlider.value <= 0)
        {
            tongueTargetPos = worldPoint;
            if (tongueTargetPos != null)
            {
                tongueTargetFirstLength = Vector3.Distance(transform.position, tongueTargetPos.Value);
            }

            sfxAudioSource.PlayOneShot(tongueClip);

            // 씬에 프리팹 게임 오브젝트를 클릭한 위치에 생성합니다.
            Instantiate(tongueTargetPrefab, worldPoint, Quaternion.identity);

            //frogSprite.sprite = frogAttackSprite;
            //frogMouth.SetActive(true);
            spriteState = SpriteState.Attack;
        }

        var tongueLocalScale = tongue.size; //.transform.localScale;
        tongueLocalScale.x =
            Mathf.SmoothDamp(tongueLocalScale.x, tongueTargetLength, ref tongueVelocity, tongueSmoothTime);
        //tongue.transform.localScale = tongueLocalScale;
        tongue.size = tongueLocalScale;

        if (tongueLocalScale.x < 0.1f && tongueTargetPos.HasValue == false && isJump == false)
        {
            //frogSprite.sprite = frogIdleSprite;
            //frogMouth.SetActive(false);
            spriteState = SpriteState.Idle;
        }

        var tongueLocalPos = tonguePos.localPosition;
        tongueLocalPos.z = tongueLocalScale.x;
        tonguePos.localPosition = tongueLocalPos;

        if (tongueTargetPos != null)
        {
            tongueTargetLength = Vector3.Distance(tonguePivot.position, tongueTargetPos.Value) / tongueScale;

            tonguePivot.transform.LookAt(tongueTargetPos.Value, Vector3.forward);

            // 목표하는 혀 길이보다 0.1f보다 조금 짧은 순간이 오면 다시 혀를 말아 들인다.
            if (Vector3.Distance(tongueTip.position, tongueTargetPos.Value) < 0.1f
                || tongueLocalScale.x > tongueTargetFirstLength / tongueScale
               )
            {
                tongueTargetLength = 0;
                tongueTargetPos = null;
            }
        }

        UpdateFrogSprite();

        // 간혹 혀 끝에 붙은 오브젝트가 사라지지 않는다.
        // 안전장치 추가!
        if (spriteState == SpriteState.Idle)
        {
            foreach (var c in tongueTip.Cast<Transform>())
            {
                Destroy(c.gameObject);
            }
        }
        
        hpSlider.value = Hp;

        debuffSlider.value = Mathf.Max(0, debuffSlider.value - Time.deltaTime);
        debuffSlider.gameObject.SetActive(debuffSlider.value > 0);
    }

    public void StartDebuff()
    {
        debuffSlider.maxValue = 5;
        debuffSlider.value = debuffSlider.maxValue;
        debuffSlider.gameObject.SetActive(true);
    }

    void UpdateFrogSprite()
    {
        idleSpriteRenderer.sprite = preset.IdleSprite;
        jumpSpriteRenderer.sprite = preset.JumpSprite;
        attackSpriteRenderer.sprite = preset.AttackSprite;
        
        idleSpriteRenderer.gameObject.SetActive(spriteState == SpriteState.Idle);
        jumpSpriteRenderer.gameObject.SetActive(spriteState == SpriteState.Jump);
        attackSpriteRenderer.gameObject.SetActive(spriteState == SpriteState.Attack);
        
        frogMouth.SetActive(spriteState == SpriteState.Attack);
    }

    private void Die()
    {
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }

        frogpivot[0].SetActive(false);
        frogpivot[1].SetActive(false);
        Debug.Log("isDie to true");
        isDie = true;
        var m = hpTime / 60;
        GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>().text = score.ToString();
        GameObject.FindWithTag("Time").GetComponent<TextMeshProUGUI>().text = $"{m}:{hpTime%60}";
        Destroy(GetComponent<Frog>());
    }

    public void PlayScoreClip()
    {
        sfxAudioSource.PlayOneShot(scoreClip);
    }

    public void AttachItemToTongue(Item item)
    {
        item.transform.SetParent(tongueTip);
    }
    
    public void AttachItemToTongue(Enemy enemy)
    {
        enemy.transform.SetParent(tongueTip);
    }

    public bool IsAttachedToTongue(Item item)
    {
        return item.transform.parent == tongueTip;
    }
    
    public bool IsAttachedToTongue(Enemy enemy)
    {
        return enemy.transform.parent == tongueTip;
    }

    public void PlayDamageClip()
    {
        sfxAudioSource.PlayOneShot(damageClip);
    }
}