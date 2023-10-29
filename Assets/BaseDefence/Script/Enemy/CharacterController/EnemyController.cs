using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO.Ports;
using UnityEngine;

public class EnemyController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private float m_AnimationWalkSpeed=9f;
    private Vector3 m_Destination;
    private bool m_CanAttack = false;
    private float m_AttackDelay = 0;
    private Vector3 m_CameraPos;

    public void Init(EnemyScriptable scriptable, Vector3 destination, Vector3 cameraPos){
        Scriptable = scriptable;
        m_Destination = destination;
        CurHp = Scriptable.MaxHp;
        m_AttackDelay = scriptable.AttackDelay + m_AttackStartUp;
        m_CameraPos = cameraPos;
        
    }

    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(m_Destination.x,m_Self.transform.position.y,m_Destination.z));
        m_Animator.Play("MoveForwar");
        m_Animator.speed = Mathf.Lerp(0f,m_AnimationWalkSpeed, Mathf.InverseLerp(0,25f, Scriptable.MoveSpeed) );

        yield return null;
        m_Self.transform.LookAt(new Vector3(m_Destination.x,m_Self.transform.position.y,m_Destination.z));
        
    }

    private void Update() {
        if( m_IsDead ){
            m_Self.transform.position += Vector3.down*Time.deltaTime;
            return;
        }

        if(!m_CanAttack&&Vector3.Distance(m_Self.transform.position , m_Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f){
            // close enough for attack 

            m_CanAttack = true;
            m_Self.transform.LookAt(new Vector3(m_CameraPos.x,m_Self.transform.position.y,m_CameraPos.z));
        }else{
            // move
            
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, m_Destination, moveDistance);
        }

        // attack wall handler
        if(m_CanAttack){
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
        BaseDefenceManager.GetInstance().OnWallHit(Scriptable.Damage);
    }

    protected override void OnDead(){
        MainGameManager.GetInstance().ChangeGooAmount(Scriptable.GooOnKill);
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetGooText();
        m_OnDead?.Invoke();
        
        m_Animator.speed = 1;
        m_Animator.Play("Die");
        Destroy(m_Self,1);
    }

}
