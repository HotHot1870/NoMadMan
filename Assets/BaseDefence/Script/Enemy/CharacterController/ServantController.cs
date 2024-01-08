using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ServantType
{
    Reover = 0,
    Weak

}
public class ServantController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_AttackStartUp = 0.45f;
    [SerializeField] private List<EnemyBodyPart> m_AllWeakSpots = new List<EnemyBodyPart>();
    [SerializeField] private List<EnemyBodyPart> m_AllNormalBodypart = new List<EnemyBodyPart>();

    // for recover
    [SerializeField] private EnemyBodyPart m_MeshShader;
    private bool m_CanAttack = false;
    protected float m_AttackDelay = 0.3f;
    private ServantType m_ServantType = ServantType.Reover;
    private XinController m_Xin;
    private ServantController m_GuardServant = null;
    private bool m_IsRecovering = false;


    private IEnumerator Start() {
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        m_Animator.Play("Move");

        yield return null;
        float locationSpeedMod = BaseDefenceManager.GetInstance().GetLocationScriptable().SpeedMutation/100f +1f;
        m_Animator.SetFloat("Speed",Scriptable.MoveSpeed*locationSpeedMod);
        m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        
    }

    public bool IsRecovering(){
        return m_IsRecovering;
    }

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        m_Xin = config.xin;
        m_AttackDelay = config.scriptable.AttackDelay + m_AttackStartUp;
    }

    public void InitRecover(EnemyControllerInitConfig config){
        Init(config);
        m_ServantType = ServantType.Reover; 
        // remove all weak point
        foreach (var item in m_AllWeakSpots)
        {
            Destroy(item.gameObject);
        }
    }

    public void InitWeak(EnemyControllerInitConfig config){
        Init(config);
        m_ServantType = ServantType.Weak;

        foreach (var item in m_AllNormalBodypart)
        {
            item.SetDamageMod(-0.5f);
            item.ChangeBodyType(EnemyBodyPartEnum.Heal);
        }

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
        if( IsThisDead )
            return;
        

        if(!m_CanAttack&&Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f && !m_IsNeted && !m_IsRecovering){
            // close enough for attack 

            m_Animator.speed = 1;
            m_Animator.Play("Idle");
            //m_Animator.SetBool("IsStoping",true);

            m_CanAttack = true;
            m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));
        }else if(!m_IsNeted && !m_IsRecovering) {
            // move
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
        }

        // attack wall handler
        if(m_CanAttack && !m_IsNeted && !m_IsRecovering){
            if(m_AttackDelay <=0){
                // attack
                StartCoroutine(Attack());
                
            }else{
                // wait
                m_AttackDelay -= Time.deltaTime;
                
            }
        }
    }

    public float GetCurHp(){
        return CurHp;
    }


    public void OnAllRecoverDead(){
        // all recover servant dead 
        IsThisDead = true;
        base.OnDead();
        m_Animator.speed = 1;
        Destroy(m_Self,1);
    }


    public IEnumerator Attack(){
        m_Animator.speed = 1;
        m_Animator.Play("Attack");
        m_AttackDelay = Scriptable.AttackDelay + m_AttackStartUp;
        yield return new WaitForSeconds(m_AttackStartUp);
        if(IsThisDead)
            yield break;

        BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage);
    }

    private IEnumerator RecoverHp(){
        float timePass = 0;
        float totalTimeTakeIfNoInterfere = 6;
        while (CurHp < GetMaxHp())
        {
            // TODO : Mesh blooming
            //float newNormalized = m_MeshShader.GetSpawnMeshNormalized() ;
            //m_MeshShader.SetSpawnMeshNormalized( Mathf.Clamp01( newNormalized ));

            timePass+=Time.deltaTime;
            ChangeHp(Time.deltaTime * GetMaxHp() / totalTimeTakeIfNoInterfere);
            yield return null;
        }
        m_MeshShader.SetSpawnMeshNormalized( 1);
        CurHp = GetMaxHp();
        m_IsRecovering = false;
        m_CanAttack = false;
        m_Animator.Play("Move");
    }

    protected override void OnDead(){
        if(m_ServantType == ServantType.Reover){
            IsThisDead = false;
            m_Animator.Play("Dead");
            // TODO : check if other is dead , if not , heal in 3 second , if so , dead
            if(m_Xin.IsAllServantRecovering()){
                // all dead
                IsThisDead = true;
                base.OnDead();
                m_Animator.speed = 1;
                Destroy(m_Self,1);
            }else if(!m_IsRecovering){
                // Not all dead, recover
                StartCoroutine(RecoverHp());
            }
            m_IsRecovering = true;
            
            return;
        }
        
        m_Xin.WeakServantDeadHandler();

        base.OnDead();
        m_Animator.speed = 1;
        m_Animator.Play("Dead");
        Destroy(m_Self,1);
    }



}
