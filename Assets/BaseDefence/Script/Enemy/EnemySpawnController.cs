using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseDefenceNameSpace;
using Unity.Collections;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private Transform m_EnemyParent;
    [SerializeField] private Transform m_GroundSpawnerCenter;
    [SerializeField] private float m_GroundSpawnerWidth = 40;
    [SerializeField] private Transform m_Destination;
    [SerializeField] private float m_WallWidth = 10;
    [SerializeField] private Transform m_Camera;

    [Header("Attacker Spot")]
    [SerializeField] private Transform m_LeftAttackerSpot;
    private bool m_IsLeftAttackerSpotOccupied=false;
    [SerializeField] private Transform m_MidAttackerSpot;
    private bool m_IsMidAttackerSpotOccupied=false;
    [SerializeField] private Transform m_RightAttackerSpot;
    private bool m_IsRightAttackerSpotOccupied=false;




    private WaveScriptable m_WavesData;
    private int m_WaveCount = 0;
    private float m_TimePassed = 0;
    private bool m_IsFinalWaveStarted = false;
    private float m_MaxSpawnDelay = 10f;

    private void Start()
    {
        BaseDefenceManager.GetInstance().m_ShootUpdateAction += EnemySpawnUpdate;
        GetAttackerSpotInOrder( new Vector3(1,2,3));
    }

    public void StartWave(WaveScriptable waesData){
        m_WavesData = waesData;
        StartNextNormalWave();
    }

    public List<Vector3> GetAttackerSpotInOrder(Vector3 pos){
        // return from closest to fardest
        List<Vector3> ans = new List<Vector3>();
        //
        ans.Add(m_LeftAttackerSpot.position);
        ans.Add(m_MidAttackerSpot.position);
        ans.Add(m_RightAttackerSpot.position);
        ans.Sort((p1,p2)=>Vector3.Distance(p1,pos).CompareTo(Vector3.Distance(p2,pos)));

        return ans;
    }

    private void EnemySpawnUpdate(){

        // check time pass to prevent all wave start at begin
        if(m_TimePassed>=m_MaxSpawnDelay){
            // is all enemy dead
            if(m_EnemyParent.childCount<=0){
                // all dead
                
                if(m_IsFinalWaveStarted){
                        // all dead
                        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetResultPanel(true);
                        BaseDefenceManager.GetInstance().ChangeGameStage(BaseDefenceStage.Result);
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
        if(m_WaveCount>=m_WavesData.NormalWavesCount){
            m_IsFinalWaveStarted = true;
        }
        float Strength = m_IsFinalWaveStarted?m_WavesData.FinalWaveStrength:m_WavesData.NormalWavesStrength;
        List<int> taregtEnemyTypes = m_IsFinalWaveStarted?m_WavesData.FinalWaveEnemy:m_WavesData.NormalWaveEnemy;
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
        while (passedTime < delay)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }

        var newEnemy = Instantiate(enemyData.Prefab, m_EnemyParent);

        var targetPos = new Vector3( m_Destination.position.x, m_GroundSpawnerCenter.position.y ,m_Destination.position.z) 
            + Vector3.right * Random.Range(m_WallWidth * -1f, m_WallWidth) +
            Vector3.forward * Mathf.Lerp(-0.5f,0.5f, Mathf.InverseLerp(0f,4f,index%5f) );

        var enemyConfig = new EnemyControllerInitConfig{
            scriptable = enemyData,
            destination = targetPos,
            cameraPos = m_Camera.position
        };
        newEnemy.GetComponent<EnemyControllerBase>().Init(enemyConfig);

        newEnemy.transform.position = m_GroundSpawnerCenter.position +
            Random.Range(m_GroundSpawnerWidth * -1f, m_GroundSpawnerWidth) * Vector3.right;


    }



}
