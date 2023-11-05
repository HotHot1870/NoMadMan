using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private float m_AnimationWalkSpeed=9f;
    [SerializeField]

    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        m_Animator.Play("MoveForward");
        m_Animator.speed = Mathf.Lerp(0f,m_AnimationWalkSpeed, Mathf.InverseLerp(0,25f, Scriptable.MoveSpeed) );

        yield return null;
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }

    private void Update() {
        if( IsThisDead )
            return;
        

        if(Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f){
            // close enough for attack 

            StartCoroutine(Attack());
        }else{
            // move
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
        }
    }

    public IEnumerator Attack(){
        if(IsThisDead)
            yield break;

        BaseDefenceManager.GetInstance().OnWallHit(Scriptable.Damage);
        OnDead();
    }

    protected override void OnDead(){
        //base.OnDead();

        // TODO : play explode effect
        Destroy(m_Self);
    }

}

