using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppeteerController : EnemyControllerBase
{ 
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private GameObject m_Ghost;
    [SerializeField] private EnemyScriptable m_GhostScriptable;
    private EnemyControllerBase m_Puppet=null;
    protected float m_AttackDelay = 0;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
    }
    
    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
        m_Self.transform.position += Vector3.up*6f ;
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
        m_AttackDelay = Scriptable.AttackDelay + m_AttackStartUp;
        yield return new WaitForSeconds(m_AttackStartUp);
        if(IsThisDead)
            yield break;
        // spawn ghost
        var ghost = Instantiate(m_Ghost,this.transform.parent);   
        ghost.transform.position = m_Self.transform.position;     
        
        var enemyConfig = new EnemyControllerInitConfig{
            scriptable = m_GhostScriptable,
            destination = CameraPos + Vector3.forward + Vector3.down * 0.5f ,
            cameraPos = CameraPos
        };

        ghost.GetComponent<GhostController>().Init(enemyConfig);
        //BaseDefenceManager.GetInstance().OnWallHit(Scriptable.Damage);
    }

    public void SetGhostScriptable(EnemyScriptable enemyScriptable){
        m_GhostScriptable = enemyScriptable;
    }
    
    protected override void OnDead(){
        base.OnDead();

        m_Animator.speed = 1;
        Destroy(m_Self,1);
    }
}
