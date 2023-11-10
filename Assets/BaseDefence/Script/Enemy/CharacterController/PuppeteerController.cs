using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppeteerController : EnemyControllerBase
{ 
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private GameObject m_Ghost;
    [SerializeField] private GameObject m_PuppetPrefab;
    [SerializeField] private EnemyScriptable m_GhostScriptable;
    [SerializeField] private EnemyScriptable m_PuppetScriptable;
    [SerializeField] private GameObject m_Electic;
    private PuppetController m_PuppetController=null;
    protected float m_AttackDelay = 0;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
        // spawn puppet
        var puppet = Instantiate(m_PuppetPrefab,m_Self.transform.parent);   
        
        var enemyConfig = new EnemyControllerInitConfig{
            scriptable = m_PuppetScriptable,
            destination = config.destination ,
            cameraPos = CameraPos,
            spawnPos = config.spawnPos
        };

        m_PuppetController = puppet.GetComponent<PuppetController>();
        m_PuppetController.Init(enemyConfig);
        m_PuppetController.AddOnDeadAction(DestoryElectic);
    }
    
    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
        m_Self.transform.position += Vector3.up*5f ;
        m_Self.transform.position += m_Self.transform.forward * 20f ;
        m_Animator.Play("Idle");

        yield return null;
        
        var electicController = m_Electic.GetComponent<ElecticController>();
        electicController.m_StartPos = m_Self;
        electicController.m_EndPos.transform.SetParent(m_PuppetController.transform);
        electicController.m_EndPos.transform.localPosition = Vector3.zero + Vector3.up*0.75f;

        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }

    public void DestoryElectic(){
        if(m_Electic != null)
            Destroy(m_Electic);
    }
    
    private void Update() {
        if( IsThisDead )
            return;

        if(m_PuppetController != null){
            if(!m_PuppetController.IsDead()){
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
    public void SetPuppetScriptable(EnemyScriptable enemyScriptable){
        m_PuppetScriptable = enemyScriptable;
    }
    
    protected override void OnDead(){
        base.OnDead();
        
        DestoryElectic();

        // remove puppet shield
        if(m_PuppetController != null){
            m_PuppetController.OnPuppeteerDead();
        }

        m_Animator.speed = 1;
        Destroy(m_Self,1);
    }
}
