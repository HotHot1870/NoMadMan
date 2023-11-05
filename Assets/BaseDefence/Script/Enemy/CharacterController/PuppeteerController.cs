using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppeteerController : EnemyControllerBase
{ 
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    private EnemyControllerBase m_Puppet;
    protected float m_AttackDelay = 0;
    private bool m_CanAttack = false;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
    }
    
    private IEnumerator Start() {
        Destination += Vector3.up * 1.5f;
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
        m_Self.transform.position += Vector3.up*8f ;
        m_Self.transform.position += m_Self.transform.forward * 15f ;
        m_Animator.Play("Idle");

        yield return null;
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }
    
    private void Update() {
        if( IsThisDead )
            return;

        if(m_Puppet != null){
            if(!m_Puppet.IsDead()){
                // puppet is alive , stay in mid air
            }
        }else{
            // move
            /*
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);

            */
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
    }

    
    public IEnumerator Attack(){
        m_Animator.speed = 1;
        // TODO : attack animation
        //m_Animator.Play("RightAttack");
        m_AttackDelay = Scriptable.AttackDelay + m_AttackStartUp;
        yield return new WaitForSeconds(m_AttackStartUp);
        if(IsThisDead)
            yield break;
        // spawn ghost
        
        //BaseDefenceManager.GetInstance().OnWallHit(Scriptable.Damage);
    }

    
    protected override void OnDead(){
        base.OnDead();

        m_Animator.speed = 1;
        // TODO : dead animation
        //m_Animator.Play("Die");
        Destroy(m_Self,1);
    }
}
