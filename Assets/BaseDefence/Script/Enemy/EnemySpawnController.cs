using System.Collections;
using System.Collections.Generic;
using BaseDefenceNameSpace;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private Transform m_EnemyParent;
    [SerializeField] private Transform m_GroundSpawnerCenter;
    [SerializeField] private float m_GroundSpawnerWidth = 40;
    [SerializeField] private Transform m_Destination;
    [SerializeField] private float m_WallWidth = 10;
    [SerializeField] private Transform m_ShootCamera;
    [SerializeField] private Camera m_MainCamera;




    private MapLocationScriptable m_LocationData;
    private int m_WaveCount = 0;
    private int m_EnemySpawnId =0;
    private float m_TimePassed = 0;
    private bool m_IsFinalWaveStarted = false;
    private float m_MaxSpawnDelay = 10f;
    private float m_StrengthBase = 0;

    private List<Transform> m_AllEnemyTrans = new List<Transform>();


    private void Start()
    {
        //BaseDefenceManager.GetInstance().m_ShootUpdateAction += EnemySpawnUpdate;
        //GetAttackerSpotInOrder( new Vector3(1,2,3));
    }

    public MapLocationScriptable GetLocationScriptable(){
        return m_LocationData;
    }

    public void StartWave(MapLocationScriptable locationData){
        m_LocationData = locationData;
        StartNextNormalWave();
    }

    public Vector3 GetOneRandomEnemyPos(){
        Vector3 ans = Vector3.zero;
        int tryCount = 0;
        while (tryCount< m_AllEnemyTrans.Count * 3f && m_AllEnemyTrans.Count>=1 )
        {
            int randomInt = UnityEngine.Random.Range(0,m_AllEnemyTrans.Count);
            if(m_AllEnemyTrans[randomInt] != null){
                ans = m_AllEnemyTrans[randomInt].position;
                return ans;
            }
        }
        return ans;
    }

    void Update(){

        // check time pass to prevent all wave start at begin
        if(m_TimePassed>=m_MaxSpawnDelay){
            // is all enemy dead
            if(m_EnemyParent.childCount<=0){
                // all dead
                
                if(m_IsFinalWaveStarted){
                        // all dead
                        BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceStage.Result);
                        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetResultPanel(true);
                    return;
                }

                m_TimePassed = 0;
                StartNextNormalWave();
            }
        }else{
            m_TimePassed += Time.deltaTime;
        }
    }

    private void StartNextNormalWave()
    {
        if(m_LocationData.Id ==21){
            // get stronger overtime
            m_StrengthBase++;
        }
        // last level infinite 
        if(m_WaveCount>=m_LocationData.NormalWavesCount && m_LocationData.Id !=21 ){
            m_IsFinalWaveStarted = true;
        }
        float Strength = m_IsFinalWaveStarted?m_LocationData.FinalWaveStrength:m_LocationData.NormalWavesStrength + m_StrengthBase;
        List<int> taregtEnemyTypes = m_IsFinalWaveStarted?m_LocationData.FinalWaveEnemy:m_LocationData.NormalWaveEnemy;
        Dictionary<int,EnemyScriptable> allPossibleEnemy = new Dictionary<int,EnemyScriptable>();
        var allenemy = MainGameManager.GetInstance().GetAllEnemy();
        foreach (var item in taregtEnemyTypes)
        {
            allPossibleEnemy.Add(item, allenemy.Find(x=>x.Id == item));
        }
        int index = 0;
        while (Strength > 0)
        {
            var targetEnemyId = taregtEnemyTypes[Random.Range(0, taregtEnemyTypes.Count)];

            // last enemy spawn time 
            float spawnTime = Random.Range(0f, m_MaxSpawnDelay);
            m_MaxSpawnDelay = Mathf.Max(m_MaxSpawnDelay,spawnTime);
            StartCoroutine(SpawnEnemy(spawnTime, allPossibleEnemy[targetEnemyId],index));
            Strength -=  allPossibleEnemy[targetEnemyId].DangerValue;
            index++;
        }
        m_WaveCount++;

    }


    private IEnumerator SpawnEnemy(float delay, EnemyScriptable enemyData, float index)
    {
        float passedTime = 0;
        if(enemyData.Id == 6){
            passedTime =1+ delay;
            //newEnemy.transform.position = spawnPos;
        }
        while (passedTime < delay)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }

        var newEnemy = Instantiate(enemyData.Prefab, m_EnemyParent);

        var targetPos = new Vector3( m_Destination.position.x, m_GroundSpawnerCenter.position.y ,m_Destination.position.z) 
            + Vector3.right * Random.Range(m_WallWidth * -1f, m_WallWidth) +
            Vector3.forward * Mathf.Lerp(-0.5f,0.5f, Mathf.InverseLerp(0f,4f,index%5f) );

        var spawnPos = m_GroundSpawnerCenter.position +
            Random.Range(m_GroundSpawnerWidth * -1f, m_GroundSpawnerWidth) * Vector3.right;
            
        var enemyConfig = new EnemyControllerInitConfig{
            scriptable = enemyData,
            destination = targetPos,
            cameraPos = m_ShootCamera.position,
            spawnPos = spawnPos,
            camera = m_MainCamera,
            spawnId = m_EnemySpawnId
        };

        m_EnemySpawnId++;

        newEnemy.GetComponent<EnemyControllerBase>().Init(enemyConfig);



    }


    public int GetEnemySpawnId(){
        m_EnemySpawnId++;
        return m_EnemySpawnId-1;
    }

    public List<Transform> GetAllEnemyTrans(){
        return m_AllEnemyTrans;
    }

    public void AddEnemyToList(Transform trans){
        m_AllEnemyTrans.Add(trans);
    }

    public void RemoveDeadEnemyFromList(Transform trans){
        m_AllEnemyTrans.Remove(trans);
    }

}
