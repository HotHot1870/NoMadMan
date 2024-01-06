using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_BossHPBarPrefab;
    [SerializeField] private GameObject m_ServantPrefab;
    [SerializeField] private EnemyScriptable m_ServantScriptable;
    [SerializeField] private List<Transform> m_ServantSpawnPoint = new List<Transform>();
    [SerializeField] private List<Transform> m_ServantDestination = new List<Transform>();
    private List<Transform> m_AllServants = new List<Transform>();

    void Start(){
        if(m_ServantScriptable == null){
            // TODO : auto load serveant scriptable
            Debug.Log("missing serveant scriptable");
        }
        SpawnNewWave();
    }

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
        m_Self.transform.localPosition = new Vector3(0,0,17);
    }

    void Update(){

    } 

    public void SpawnNewWave(){
            BaseDefenceManager.GetInstance().LookAtField();
        //int randomInt = Random.Range(0,3); // 0,1,2
        int randomInt = 0; // 0,1,2
        switch (randomInt)
        {
            case 0: 
                StartCoroutine( SpawnRecoverServant() );
            break;
            case 1:
                //StartCoroutine( SpawnWeakServant() );
            break;
            case 2:
                //StartCoroutine( SpawnLinkServant() );
            break;
            default:
            break;
        }
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


        // look at xin
        BaseDefenceManager.GetInstance().LookAtXin();
        return true;
    }

    private IEnumerator SpawnRecoverServant(){
        // TODO : spawn 3 , if 0 hp and other still alive , full recover in 3 sec
        List<int> m_UnusedInt = new List<int>();
        for (int i = 0; i < m_ServantSpawnPoint.Count; i++)
        {   
            m_UnusedInt.Add(i);
        }
        for (int i = 0; i < 3; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
            newServant.position = m_ServantSpawnPoint[0].position;
            int randomInt = m_UnusedInt[Random.Range(0,m_UnusedInt.Count)];
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
            newServant.GetComponent<ServantController>().InitRecover(enemyConfig);
            m_AllServants.Add(newServant);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnWeakServant(){
        // TODO : spawn 3 , weak sport only , heal on non-weak sport attack
        for (int i = 0; i < 3; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
            newServant.position = m_ServantSpawnPoint[i].position;
            m_AllServants.Add(newServant);
        }
    }


    private void SpawnLinkServant(){
        // TODO : spawn 3 , link shield same as puppet , show shield and link on hit
        for (int i = 0; i < 3; i++)
        {
            Transform newServant = Instantiate(m_ServantPrefab,this.transform.parent).transform;
            newServant.position = m_ServantSpawnPoint[i].position;
            m_AllServants.Add(newServant);
        }
    }
}
