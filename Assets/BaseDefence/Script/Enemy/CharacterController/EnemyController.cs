using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO.Ports;
using UnityEngine;

public class EnemyController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    private Vector3 m_Destination;
    private bool m_CanAttack = false;
    private float m_AttackDelay = 0;

    public void Init(EnemyScriptable scriptable, Vector3 destination){
        Scriptable = scriptable;
        m_Destination = destination;
        CurHp = Scriptable.MaxHp;
        m_AttackDelay = scriptable.AttackDelay;
        
        m_Self.transform.LookAt(new Vector3(m_Destination.x,m_Self.transform.position.y,m_Destination.z));
    }

    private void Start() {
        //m_Animator.Play("Moving");
        
    }

    private void Update() {
        if( m_IsDead )
            return;

        if(Vector3.Distance(m_Self.transform.position , m_Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f){
           // close enough for attack 
           m_CanAttack = true;
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
                Attack();
            }else{
                // wait
                m_AttackDelay -= Time.deltaTime;
                
            }
        }
    }

    public void Attack(){
        //m_Animator.Play("Attack");
        m_AttackDelay = Scriptable.AttackDelay;
        BaseDefenceManager.GetInstance().OnWallHit(Scriptable.Damage);
    }

    protected override void OnDead(){
        m_Animator.enabled = false;
        MainGameManager.GetInstance().ChangeGooAmount(Scriptable.GooOnKill);
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetGooText();
        m_OnDead?.Invoke();
        Destroy(m_Self,1);
    }

}
