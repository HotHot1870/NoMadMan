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
    [SerializeField] private EnemyScriptable m_ServantScriptable;
    [SerializeField] private List<Transform> m_ServantSpawnPoint = new List<Transform>();
    [SerializeField] private List<Transform> m_ServantDestination = new List<Transform>();
    private List<Transform> m_AllServants = new List<Transform>();
    private XinHpController m_HpController = null;

    void Start(){
        if(m_ServantScriptable == null){
            // TODO : auto load serveant scriptable
            Debug.Log("missing serveant scriptable");
        }
        BaseDefenceManager.GetInstance().LookAtXin();
        StartCoroutine(WaveEnd());
    }

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        m_Self.transform.localPosition = new Vector3(0,0,17);
        m_HpController = BaseDefenceManager.GetInstance().GetXinHpController();
        m_HpController.Init(Scriptable);
    }

    void Update(){

    } 

    public IEnumerator WaveEnd(){
        yield return new WaitForSeconds(0.5f);
        BaseDefenceManager.GetInstance().LookAtXin();
        yield return new WaitForSeconds(3f);
        // TODO : spawn ball
        var spawnBall = Instantiate(m_SpawnBallPrefab, m_SpawnBallStartPos.position, Quaternion.identity,this.transform);
        spawnBall.GetComponent<XinSpawnBallController>().Init(this);
      
    }
    public void StartWave(){
        BaseDefenceManager.GetInstance().LookAtField();
        int randomInt = UnityEngine.Random.Range(0,2);
        StartCoroutine( SpawnServant( (ServantType) randomInt ) );
    }

    public void WeakServantDeadHandler(){
        // TODO : call by servant on dead
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
        base.ChangeHp(changes);
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

        m_HpController.ResetRound();

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

        for (int i = 0; i < 3; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
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
/*
    private void SpawnWeakServant(){
        for (int i = 0; i < 3; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
            newServant.position = m_ServantSpawnPoint[i].position;
            m_AllServants.Add(newServant);
        }
    }*/

}
