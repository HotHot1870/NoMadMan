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
    [SerializeField] private float m_AttackStartUp = 0.35f;
    [SerializeField] private List<EnemyBodyPart> m_AllWeakSpots = new List<EnemyBodyPart>();
    [SerializeField] private List<EnemyBodyPart> m_AllNormalBodypart = new List<EnemyBodyPart>();

    // for recover
    [SerializeField] private EnemyBodyPart m_MeshShader;
    private bool m_CanAttack = false;
    protected float m_AttackDelay = 0.3f;
    private ServantType m_ServantType = ServantType.Reover;
    private XinController m_Xin;
    private bool m_IsRecovering = false;
    private Coroutine m_Recovering = null;



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
        m_MeshShader.SetBackColor(Color.green);
    }

    public void InitWeak(EnemyControllerInitConfig config){
        Init(config);
        m_ServantType = ServantType.Weak;

        foreach (var item in m_AllNormalBodypart)
        {
            item.SetDamageMod(-0.5f * (1+BaseDefenceManager.GetInstance().GetXinHpController().GetSkullCount()*0.05f));
            item.ChangeBodyType(EnemyBodyPartEnum.Heal);
        }

        int mustShowInt = Random.Range(0,m_AllWeakSpots.Count);
        for (int i = 0; i < m_AllWeakSpots.Count; i++)
        {
            if(i==mustShowInt)
                continue;

            int randomInt = Random.Range(0,2);
            if(randomInt==1){
                Destroy(m_AllWeakSpots[i].gameObject);
            }
        }

    }



    private IEnumerator FirstAttack(){
        if (!m_IsRecovering)
        {
            if(IsThisDead)
                yield break;

            m_Animator.Play("Attack");
            
            // attack animation delay
            yield return new WaitForSeconds(m_AttackStartUp);
            if(IsThisDead)
                yield break;
              
            PlayHitPlayerSound();
            var hitScreenPos = Camera.main.WorldToScreenPoint(m_Self.transform.position+Vector3.up*1.5f);  
            BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage, hitScreenPos);
        }
        StartCoroutine(Attack());
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
            StartCoroutine(FirstAttack());
        }else if(!m_IsNeted && !m_IsRecovering) {
            // move
            float moveDistance =  Scriptable.MoveSpeed * Time.deltaTime;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
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

        while (!IsThisDead)
        {
            while (m_IsRecovering)
            {
                yield return null;
            }
                if(m_IsNeted){
                    yield return null;
                    continue;
                }
                m_Animator.speed = 1;
                // wait delay
                yield return new WaitForSeconds(Scriptable.AttackDelay);            
                while (m_IsRecovering)
                {
                    yield return null;
                }
                if(IsThisDead)
                    yield break;
                    m_Animator.Play("Attack");
                    // attack animation delay
                    yield return new WaitForSeconds(m_AttackStartUp);            
                    while (m_IsRecovering)
                    {
                        yield return null;
                    }
                    if(IsThisDead)
                        yield break;

                    PlayHitPlayerSound();
                    var hitScreenPos = Camera.main.WorldToScreenPoint(m_Self.transform.position+Vector3.up*1.5f);  
                    BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage,hitScreenPos); 
                    yield return null;
        }
    }

    private IEnumerator RecoverHp(){
        float timePass = 0;
        m_MeshShader.SetSpawnMeshNormalized( 0.7f);
        float totalTimeTakeIfNoInterfere = 6 - BaseDefenceManager.GetInstance().GetXinHpController().GetSkullCount()*0.5f;
        while (CurHp < GetMaxHp())
        {
            // Mesh blooming
            m_MeshShader.ChangeMaterialNoiseDensity( Random.Range(-0.2f,0.3f) * Time.deltaTime);
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
            if(m_Xin.IsAllServantRecovering()){
                // all dead
                IsThisDead = true;
                
                BaseDefenceManager.GetInstance().RemoveDeadEnemyFromList(this.transform);
                base.OnDead();
                m_Animator.speed = 1;
                Destroy(m_Self,1);
            }else if(!m_IsRecovering){
                // Not all dead, recover
                if(m_Recovering != null){
                    StopCoroutine(m_Recovering);
                }
                m_Recovering = StartCoroutine(RecoverHp());
            }
            m_IsRecovering = true;
            
            return;
        }
        
        BaseDefenceManager.GetInstance().RemoveDeadEnemyFromList(this.transform);
        m_Xin.WeakServantDeadHandler();

        base.OnDead();
        m_Animator.speed = 1;
        m_Animator.Play("Dead");
        Destroy(m_Self,1);
    }



}
