using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseDefenceNameSpace;
using Unity.Collections;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private Transform m_EnemyParent;
    [SerializeField] private WaveScriptable m_WavesData;
    [SerializeField] private Transform m_GroundSpawnerCenter;
    [SerializeField] private float m_GroundSpawnerWidth = 40;
    [SerializeField] private Transform m_WallCenter;
    [SerializeField] private float m_WallWidth = 10;


    private int m_WaveCount = 0;
    private float m_TimePassed = 0;
    private bool m_IsFinalWaveStarted = false;
    private float m_MaxSpawnDelay = 10f;

    private void Start()
    {
        BaseDefenceManager.GetInstance().m_ShootUpdateAction += EnemySpawnUpdate;
    }

    public void StartWave(WaveScriptable waesData){
        m_WavesData = waesData;
        StartNextNormalWave();
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
        while (Strength > 0)
        {
            var targetEnemyId = taregtEnemyTypes[Random.Range(0, taregtEnemyTypes.Count)];

            // last enemy spawn time 
            float spawnTime = Random.Range(0f, m_MaxSpawnDelay);
            m_MaxSpawnDelay = Mathf.Max(m_MaxSpawnDelay,spawnTime);
            StartCoroutine(SpawnEnemy(spawnTime, allPossibleEnemy[targetEnemyId]));
            Strength -=  allPossibleEnemy[targetEnemyId].DangerValue;
        }
        m_WaveCount++;

    }


    private IEnumerator SpawnEnemy(float delay, EnemyScriptable enemyData)
    {
        float passedTime = 0;
        while (passedTime < delay)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }

        var newEnemy = Instantiate(enemyData.Prefab, m_EnemyParent);

        var targetPos = new Vector3( m_WallCenter.position.x, m_GroundSpawnerCenter.position.y ,m_WallCenter.position.z) 
            + Vector3.right * Random.Range(m_WallWidth * -1f, m_WallWidth) +
            Vector3.forward * Random.Range(0f, 0.4f);
        newEnemy.GetComponent<EnemyController>().Init(enemyData, targetPos);

        newEnemy.transform.position = m_GroundSpawnerCenter.position +
            Random.Range(m_GroundSpawnerWidth * -1f, m_GroundSpawnerWidth) * Vector3.right;


    }



}
