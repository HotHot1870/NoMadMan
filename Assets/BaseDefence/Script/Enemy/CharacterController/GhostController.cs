using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_DeadEffect;
    [SerializeField] private Animator m_Animator;

    private IEnumerator Start() {
        var deadEffect = Instantiate(m_DeadEffect,this.transform.parent);
        deadEffect.transform.position = m_Self.transform.position;
        deadEffect.GetComponent<ParticleSystem>().Play();
        Destroy(deadEffect,5);
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));

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
        

        if(Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f ){
            // close enough for attack 

            Attack();
        }else if(!m_IsNeted) {
            // move
            float locationSpeedMod = BaseDefenceManager.GetInstance().GetLocationScriptable().SpeedMutation/100f +1f;
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime * locationSpeedMod;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
        }
    }

    public void Attack(){
        if(IsThisDead || BaseDefenceManager.GetInstance().GetCurHp()<=0)
            return;

        IsThisDead = true;
        PlayHitPlayerSound();
        m_HitPlayerSoundPlayer.PlayOneShot(m_HitPlayerSound);
        var hitScreenPos = Camera.main.WorldToScreenPoint(m_Self.transform.position+Vector3.up*1.5f);
        BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage,hitScreenPos);
        OnDead();
    }

    protected override void OnDead(){
        //base.OnDead();
        var deadEffect = Instantiate(m_DeadEffect,this.transform.parent);
        deadEffect.transform.position = m_Self.transform.position;
        deadEffect.GetComponent<ParticleSystem>().Play();
        Destroy(deadEffect,5);
        m_Self.transform.localScale = Vector3.zero;
        Destroy(m_Self,1.5f);
    }

}

