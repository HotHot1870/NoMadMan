using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class XinController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_BossHPBarPrefab;
    [SerializeField] private GameObject m_ServantPrefab;
    [SerializeField] private GameObject m_SpawnBallPrefab;
    [SerializeField] private Transform m_SpawnBallStartPos;
    [SerializeField] private SkinnedMeshRenderer m_XinSkinMesh;
    [SerializeField] private EnemyScriptable m_ServantScriptable;
    [SerializeField] private List<XinBodyPart> m_AllXinBodyPart = new List<XinBodyPart>();
    [SerializeField] private List<Transform> m_ServantSpawnPoint = new List<Transform>();
    [SerializeField] private List<Transform> m_ServantDestination = new List<Transform>();
    [SerializeField] private GameObject m_DyingXinPrefab = null;    
    [SerializeField] private GameObject m_Star;
    private List<Transform> m_AllServants = new List<Transform>();
    private XinHpController m_HpController = null;
    private GameObject m_SpawnBall = null;
    public bool m_IsDyingEffect = false;
    

    void Start(){
        if(m_ServantScriptable == null){
            Debug.Log("missing serveant scriptable");
        }

        m_Star.SetActive(BaseDefenceManager.GetInstance().GetLocationScriptable().Id==20);
        
        BaseDefenceManager.GetInstance().LookAtXin();
        StartCoroutine(WaveEnd());
        m_HpController = BaseDefenceManager.GetInstance().GetXinHpController();
        m_HpController.Init(Scriptable,this);
    }

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        m_Self.transform.localPosition = new Vector3(0,0,17);
    }

    void Update(){

    } 

    public void SetServantScriptable(EnemyScriptable enemyScriptable){
        m_ServantScriptable = enemyScriptable;
    }

    public IEnumerator WaveEnd(){
        if(m_HpController==null)
            m_HpController = BaseDefenceManager.GetInstance().GetXinHpController();

        m_HpController.ResetRound();
        ResetAllWeakPoint();
        yield return new WaitForSeconds(0.5f);
        BaseDefenceManager.GetInstance().LookAtXin();
        yield return new WaitForSeconds(4f);
        if(m_IsDyingEffect)
            yield break;
        // spawn ball
        m_SpawnBall = Instantiate(m_SpawnBallPrefab, m_SpawnBallStartPos.position, Quaternion.identity,this.transform);
        m_SpawnBall.GetComponent<XinSpawnBallController>().Init(this);
      
    }

    public void ResetAllWeakPoint(){
        int randomInt = UnityEngine.Random.Range(0,m_AllXinBodyPart.Count);
        for (int i = 0; i < m_AllXinBodyPart.Count; i++)
        {
            int index = i;
            m_AllXinBodyPart[i].SetXinBodyPart(index!=randomInt);
        }
    }
    public void StartWave(){
        BaseDefenceManager.GetInstance().LookAtField();
        int randomInt = UnityEngine.Random.Range(0,2);
        StartCoroutine( SpawnServant( (ServantType) randomInt ) );
    }

    public void WeakServantDeadHandler(){
        // call by servant on dead
        foreach (var item in m_AllServants)
        {
            if(item != null){
                if(item.TryGetComponent<ServantController>(out var servantController)){
                    if(servantController.GetCurHp()>0){
                        // one still alive 
                        return;
                    }
                }
            }
        }

        StartCoroutine(WaveEnd());
    }

    public override void ChangeHp(float changes)
    {
        //base.ChangeHp(changes);
        m_HpController.ChangeHp(changes);
    }

    public bool IsAllServantRecovering(){
        foreach (var item in m_AllServants)
        {
            if(item != null){
                if(item.TryGetComponent<ServantController>(out var servantController)){
                    if(!servantController.IsRecovering()){
                        // one still alive 
                        return false;
                    }
                }
            }
        }


        // tell all other servant to die because ondead only call once on hp reach 0
        foreach (var item in m_AllServants)
        {
            if(item != null){
                if(item.TryGetComponent<ServantController>(out var servantController)){
                    servantController.OnAllRecoverDead();
                }
            }
        }
        m_AllServants.Clear();

        StartCoroutine(WaveEnd());
        return true;
    }

    private IEnumerator SpawnServant(ServantType type){
        List<int> m_UnusedInt = new List<int>();
        for (int i = 0; i < m_ServantSpawnPoint.Count; i++)
        {   
            m_UnusedInt.Add(i);
        }

        int servantCount = 3;
        if(MainGameManager.GetInstance().GetSelectedLocation().Id == 20){
            // finial
            servantCount = 4;

        }
        for (int i = 0; i < servantCount; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
            BaseDefenceManager.GetInstance().AddEnemyToList(newServant);
            newServant.position = m_ServantSpawnPoint[0].position;
            int randomInt = m_UnusedInt[UnityEngine.Random.Range(0,m_UnusedInt.Count)];
            m_UnusedInt.Remove(randomInt);
            newServant.position = m_ServantSpawnPoint[randomInt].position;

            var enemyConfig = new EnemyControllerInitConfig{
                scriptable = m_ServantScriptable,
                destination = m_ServantDestination[randomInt].position,
                cameraPos = CameraPos,
                spawnPos = m_ServantSpawnPoint[randomInt].position,
                camera = MainCamera,
                spawnId = SpawnId,
                xin = this
            };

            var newServantController = newServant.GetComponent<ServantController>();
            switch (type)
            {
                case ServantType.Reover:
                    newServantController.InitRecover(enemyConfig);
                break;
                case ServantType.Weak:
                    newServantController.InitWeak(enemyConfig);
                break;
                default:
                break;
            }
            m_AllServants.Add(newServant);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator SetResult(){
        m_IsDyingEffect = true;
        if(m_SpawnBall != null)
            Destroy(m_SpawnBall);

        float passtime = 0f;
        float duration = 1f;
        while (passtime<duration)
        {
            passtime += Time.deltaTime;
            foreach (var item in m_XinSkinMesh.materials)
            {
                item.SetFloat("_Normalized",(duration-passtime)/duration);
            }
            foreach (var item in m_AllXinBodyPart)
            {
                item.SetNormalized((duration-passtime)/duration);
            }
            yield return null;
        }
        m_AllXinBodyPart.RemoveAll(s => s == null);
        foreach (var item in m_AllXinBodyPart)
        {
            Destroy(item.gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        BaseDefenceManager.GetInstance().LookAtField();
        // spawn dying xin
        var dyingXin  = Instantiate(m_DyingXinPrefab);
        dyingXin.transform.position = m_Self.transform.position;
        BaseDefenceManager.GetInstance().AddEnemyToList(dyingXin.transform);
        dyingXin.GetComponent<DyingXinController>().Init(m_ServantDestination[0].position);
        yield return null;
        OnDead();
    }

    // xin die , win
    protected override void OnDead(){
        base.OnDead();
        Destroy(this.gameObject,0.1f);
        
    }


}
