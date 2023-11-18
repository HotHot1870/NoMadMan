using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElwoodController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private float m_AttackStartUp = 0f;
    [SerializeField] private GameObject m_Ghost;
    [SerializeField] private EnemyScriptable m_GhostScriptable;
    [SerializeField] private Transform m_SpawnGhostPos;
    [SerializeField] private float m_AttackDelayPerSpawn = 1f;
    protected float m_AttackDelay = 0;
    private int m_SpawnCount = 0 ;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
    }
    
    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
        m_Self.transform.position += m_Self.transform.forward * 15f ;

        yield return null;

        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }
    
    private void Update() {
        if( IsThisDead )
            return;

        // attack wall handler
        if(m_AttackDelay <=0){
            // attack
            StartCoroutine(Attack());
            
        }else{
            // wait
            m_AttackDelay -= Time.deltaTime;
            
        }
            
        
    }
    

    
    public IEnumerator Attack(){
        m_AttackDelay = Scriptable.AttackDelay + m_AttackStartUp + m_AttackDelayPerSpawn * m_SpawnCount;
        yield return new WaitForSeconds(m_AttackStartUp);
        if(IsThisDead)
            yield break;
        // spawn ghost
        var ghost = Instantiate(m_Ghost,this.transform.parent);   

        m_SpawnCount ++;
        
        var enemyConfig = new EnemyControllerInitConfig{
            scriptable = m_GhostScriptable,
            destination = CameraPos + Vector3.forward + Vector3.down * 0.5f ,
            cameraPos = CameraPos,
            spawnPos = m_SpawnGhostPos.position
        };

        ghost.GetComponent<GhostController>().Init(enemyConfig);
    }

    public void SetGhostScriptable(EnemyScriptable enemyScriptable){
        m_GhostScriptable = enemyScriptable;
    }
    
    protected override void OnDead(){
        base.OnDead();
        Destroy(m_Self,1);
    }
}
