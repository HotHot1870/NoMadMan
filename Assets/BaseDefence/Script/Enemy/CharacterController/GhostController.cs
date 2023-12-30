using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private float m_AnimationWalkSpeed=9f;
    [SerializeField] private GameObject m_DeadEffect;

    private IEnumerator Start() {
        var deadEffect = Instantiate(m_DeadEffect,this.transform.parent);
        deadEffect.transform.position = m_Self.transform.position;
        deadEffect.GetComponent<ParticleSystem>().Play();
        Destroy(deadEffect,5);
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));

        yield return null;
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }

    private void Update() {
        if( IsThisDead )
            return;
        

        if(Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f){
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
        if(IsThisDead)
            return;

        BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage);
        OnDead();
    }

    protected override void OnDead(){
        //base.OnDead();
        var deadEffect = Instantiate(m_DeadEffect,this.transform.parent);
        deadEffect.transform.position = m_Self.transform.position;
        deadEffect.GetComponent<ParticleSystem>().Play();
        Destroy(deadEffect,5);

        Destroy(m_Self);
    }

}

