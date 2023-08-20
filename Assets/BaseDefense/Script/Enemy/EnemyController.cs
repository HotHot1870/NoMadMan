using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class EnemyController : MonoBehaviour
{
    private EnemyScriptable m_Scriptable;
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_AnimatorForImage;
    [SerializeField] private Animator m_AnimatorForHitBox;

    [Header("Hp")]
    [SerializeField] private GameObject m_HpBarPrefab;
    [SerializeField] private Transform m_HpBarWorldPosition;
    [SerializeField] private float m_HpBarStayTime = 2;
    private float m_CurrentHp = 100;
    private float m_TotalHpBarStayTime = 0;
    private HpBar m_HpBar; // parent of hp bar

    [Header("Damage")]
    private float m_CurrentAttackDelay = 0;
    private float m_AttackDelay = 1.5f;

    [Header("Movement")]
    [SerializeField] private float m_CurrentDistance = 100;
    [SerializeField] private float m_MaxDistance = 100;
    [SerializeField] private float m_YPosClosest = -9.5f;
    [SerializeField] private float m_YPosFarthest = 4f;
    [SerializeField] private float m_ScaleClosest = 5;
    [SerializeField] private float m_ScaleFarthest = 0.5f;
    [SerializeField] private AnimationCurve m_NormalizedMoveToScale;

    // TODO : 
    // left right sway , use animation instead of code in future
    [SerializeField][Range(0.1f, 10)] private float m_LeftRightSwayAmount = 3;
    [SerializeField][Range(1, 50)] private float m_LeftRightSwaySpeed = 5;
    private bool m_IsRotationCloseWise = true;

    private void Start() {
        this.transform.localEulerAngles += Vector3.forward * m_LeftRightSwayAmount * Random.Range(-1f,1f);
        m_CurrentHp = m_Scriptable.MaxHp;
        m_CurrentAttackDelay = m_AttackDelay;
        m_AnimatorForImage.speed = m_Scriptable.MoveSpeed/5;
        m_AnimatorForHitBox.speed = m_Scriptable.MoveSpeed/5;
        
    }

    private void FixedUpdate()
    {
        // hp bar stay visable
        if(m_TotalHpBarStayTime>0){
            m_TotalHpBarStayTime -= Time.deltaTime;
            UpdateHpBar(false);
        }else{
            if(m_HpBar)
                m_HpBar.m_CanvasGroup.alpha = 0;
        }

        if(m_CurrentDistance<=0){
            // attack wall
            m_CurrentAttackDelay -= Time.deltaTime;
            if(m_CurrentAttackDelay <=0){
                m_CurrentAttackDelay = m_AttackDelay;
                BaseDefenseManager.GetInstance().OnWallHit(m_Scriptable.Damage);
            }


            m_CurrentDistance = 0;
            this.transform.localScale = m_ScaleClosest * Vector3.one;
            this.transform.position = new Vector3(
                this.transform.position.x,
                m_YPosClosest,
                this.transform.position.z);
            this.transform.localEulerAngles = Vector3.zero;

        }else{
            // move
            m_CurrentDistance -= m_Scriptable.MoveSpeed * Time.deltaTime;

            this.transform.localScale = Vector3.Lerp( m_ScaleClosest * Vector3.one , m_ScaleFarthest * Vector3.one , 
                m_NormalizedMoveToScale.Evaluate( m_CurrentDistance / m_MaxDistance) );
            this.transform.position = new Vector3(
                this.transform.position.x,
                Mathf.Lerp(m_YPosClosest , m_YPosFarthest , m_NormalizedMoveToScale.Evaluate( m_CurrentDistance / m_MaxDistance ) ),
                this.transform.position.z
            );

            // TODO : 
            // left right sway , use animation instead of code in future
            if (m_IsRotationCloseWise)
            {
                this.transform.localEulerAngles += Vector3.forward * Time.deltaTime * m_LeftRightSwaySpeed;
                if (this.transform.localEulerAngles.z > m_LeftRightSwayAmount && 
                    this.transform.localEulerAngles.z < 180 )
                {
                    m_IsRotationCloseWise = false;
                }
            }
            else
            {
                this.transform.localEulerAngles -= Vector3.forward * Time.deltaTime * m_LeftRightSwaySpeed;
                if (this.transform.localEulerAngles.z < 0 - m_LeftRightSwayAmount || 
                    (this.transform.localEulerAngles.z < 360 - m_LeftRightSwayAmount && this.transform.localEulerAngles.z > m_LeftRightSwayAmount ) )
                {
                    m_IsRotationCloseWise = true;
                }
            }
        }

    }

    public void SetScriptable(EnemyScriptable enemyScriptable){
        m_Scriptable = enemyScriptable;
    }

    private void SpawnHpBar(){
        var hpBar = Instantiate(m_HpBarPrefab);
        hpBar.transform.SetParent( BaseDefenseManager.GetInstance().EnemyHpBarParent );
        m_HpBar = hpBar.GetComponent<HpBar>();
        UpdateHpBar(true);
    }

    private void UpdateHpBar(bool shouldResetHpFadeOutTime){
        if(shouldResetHpFadeOutTime)
            m_TotalHpBarStayTime = m_HpBarStayTime ;
        
        m_HpBar.m_CanvasGroup.alpha = 1;

        m_HpBar.GetComponent<RectTransform>().localScale = Vector3.one * ( 0.25f + Mathf.InverseLerp(m_MaxDistance,0,m_CurrentDistance) * 0.75f );
        // lower the position according to distance
        var canvasPos = Camera.main.WorldToScreenPoint(m_HpBarWorldPosition.position - Vector3.up * Mathf.InverseLerp(m_MaxDistance,0,m_CurrentDistance) * 0.6f);
        m_HpBar.GetComponent<RectTransform>().position = canvasPos;
        m_HpBar.m_HpBarFiller.fillAmount = m_CurrentHp / m_Scriptable.MaxHp;
    }

    public float GetDistance(){
        return m_CurrentDistance;
    }

    public void OnDamage(float changes)
    {
        m_CurrentHp -= changes;
        if (m_CurrentHp <= 0)
        {
            Destroy(m_Self);
            return;
        }

        if(m_HpBar==null)
        {
            SpawnHpBar();   
        }else{
            UpdateHpBar(true);
        }
    }

    private void OnDestroy()
    {
        if(m_HpBar)
            Destroy(m_HpBar.gameObject);
    }
}
