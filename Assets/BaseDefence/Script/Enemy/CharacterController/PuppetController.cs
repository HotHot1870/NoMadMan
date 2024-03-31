using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetController  : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_Shield;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private float m_AnimationWalkSpeed=9f;
    private bool m_CanAttack = false;
    
    protected float m_AttackDelay = 0;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
    }

    public void AddOnDeadAction(Action action){
        OnDeadAction+=action;
    }

    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        m_Animator.Play("MoveForward");
        m_Animator.speed = Mathf.Lerp(0f,m_AnimationWalkSpeed, Mathf.InverseLerp(0,25f, Scriptable.MoveSpeed) );

        yield return null;
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }

    

    public override void OnNet()
    {
        base.OnNet();
        m_Animator.speed = 0;
    }

    protected override void OnNetEnd()
    {
        base.OnNetEnd();
        m_Animator.speed = 1;
    }

    private void Update() {
        if(BaseDefenceManager.GetInstance().GetCurHp()<=0)
            this.enabled = false;
            
        if( IsThisDead )
            return;
        

        if(!m_CanAttack&&Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f && !m_IsNeted){
            // close enough for attack 

            m_Animator.speed = 1;
            m_Animator.SetBool("IsStoping",true);

            m_CanAttack = true;
            m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        }else if(!m_IsNeted) {
            // move
            float locationSpeedMod = BaseDefenceManager.GetInstance().GetLocationScriptable().SpeedMutation/100f +1f;
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime * locationSpeedMod;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
        }

        // attack wall handler
        if(m_CanAttack && !m_IsNeted && BaseDefenceManager.GetInstance().GetCurHp()>0){
            if(m_AttackDelay <=0){
                // attack
                StartCoroutine(Attack());
                
            }else{
                // wait
                m_AttackDelay -= Time.deltaTime;
                
            }
        }
    }

    public IEnumerator Attack(){
        m_Animator.speed = 1;
        m_Animator.Play("RightAttack");
        m_AttackDelay = Scriptable.AttackDelay + m_AttackStartUp;
        yield return new WaitForSeconds(m_AttackStartUp);
        if(IsThisDead)
            yield break;

        PlayHitPlayerSound();
        var hitScreenPos = Camera.main.WorldToScreenPoint(m_Self.transform.position+Vector3.up*1.5f);
        BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage,hitScreenPos);
    }

    public void OnPuppeteerDead(){
        // Destroy shield
        StartCoroutine(ShieldFadeOut());
    }

    private IEnumerator ShieldFadeOut(){
        
        float fadeTimeNeeded = 0.9f;
        // to make it look more responsive
        float passedTime = fadeTimeNeeded*0.15f;
        var shieldRenderer = m_Shield.GetComponent<MeshRenderer>();
        
        if(shieldRenderer == null)
            yield break;

        while (passedTime < fadeTimeNeeded)
        {
            passedTime += Time.deltaTime;
            yield return null;
            if(this.gameObject == null || shieldRenderer == null){
                yield break;
            }
            shieldRenderer.material.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);

        }
        
        if(shieldRenderer != null)
            shieldRenderer.material.SetFloat("_Normalized",  0);

        
        Destroy(m_Shield);
    }

    protected override void OnDead(){
        base.OnDead();

        m_Animator.speed = 1;
        m_Animator.Play("Die");
        Destroy(m_Self,1);
    }

}

