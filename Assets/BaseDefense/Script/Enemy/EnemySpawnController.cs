using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseDefenseNameSpace;
using Unity.Collections;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private List<EnemyScriptable> m_AllEnemyTypes = new List<EnemyScriptable>();
    [SerializeField] private Transform m_EnemyParent;

    private float m_WaveHeat = 0;
    private int m_WaveCount = 0;
    private float m_TimePassed = 0;
    private const int TotalWave = 3;
    private List<EnemyScriptable> m_VaildEnemies = new List<EnemyScriptable>();
    private List<GameObject> m_AllSpawnedEnemy = new List<GameObject>();
    private Coroutine m_LastSpawnEnemyCoroutine = null;
    private float m_LastSpawnEnemyDelayTime = 0;

    private void Start() {
        m_WaveHeat = MainGameManager.GetInstance().GetHeat() / TotalWave ;
        // filter out enemy type that need too much heat
        m_VaildEnemies = new List<EnemyScriptable>(m_AllEnemyTypes.Where(x=>x.TargetTotalHeatForSpawn<= MainGameManager.GetInstance().GetHeat()).ToList());
        BaseDefenseManager.GetInstance().m_UpdateAction += EnemySpawnUpdate;
    }

    public void EnemySpawnUpdate() {
        
        if(BaseDefenseManager.GetInstance().GameStage == BaseDefenseStage.Result){
            // game over already
            return;
        }

        if(m_WaveCount>=TotalWave && m_LastSpawnEnemyCoroutine==null){
            for (int i = 0; i < m_AllSpawnedEnemy.Count; i++)
            {
                if(m_AllSpawnedEnemy[i] == null){
                    m_AllSpawnedEnemy.RemoveAt(i);
                    i--;
                }
            }
            // on wave 3 and all enemy are killed and no enemy will be spawn
            if(m_AllSpawnedEnemy.Count<=0){
                // player win
                BaseDefenseManager.GetInstance().GameOver(false);
            }
            return;
        }

        if(m_VaildEnemies.Count<=0){
            // no vaild enemy to be spawn
            return;
        }

        m_TimePassed -= Time.deltaTime;
        if( m_TimePassed <=0 ){
            Debug.Log($"Wave {m_WaveCount}");
            WaveHandler();
        }
    }
    private void WaveHandler(){
        float totalHeatOfThisWave = 0;
        m_LastSpawnEnemyCoroutine = null;
        m_LastSpawnEnemyDelayTime = 0;
        while (totalHeatOfThisWave<m_WaveHeat )
        {
            // select random enemy
            int randomIndex = Random.Range(0,m_VaildEnemies.Count);
            var targetEnemyScriptable = m_VaildEnemies[randomIndex];  

            // select random spawn time 
            float timeDelay = Random.Range(2f,20f);
            if(timeDelay>m_LastSpawnEnemyDelayTime){
                m_LastSpawnEnemyDelayTime = timeDelay;
            }
            StartCoroutine( SpawnEnemy(timeDelay , targetEnemyScriptable) );

            // gain heat 
            totalHeatOfThisWave += targetEnemyScriptable.HeatGainForSpawn;

        }
        m_LastSpawnEnemyCoroutine = StartCoroutine(LastSpawnEnemyDelay(m_LastSpawnEnemyDelayTime));

        m_TimePassed = 30;
        m_WaveCount ++;
    }

    private IEnumerator LastSpawnEnemyDelay(float waitTime){
        yield return new WaitForSeconds(waitTime);
        m_LastSpawnEnemyCoroutine = null;
    }

    private IEnumerator SpawnEnemy(float timeDelay, EnemyScriptable enemyScriptable){
        yield return new WaitForSeconds(timeDelay);
        var newEnemy = Instantiate(enemyScriptable.Prefab);
        m_AllSpawnedEnemy.Add(newEnemy);
        newEnemy.transform.SetParent(m_EnemyParent);
        newEnemy.transform.localPosition = new Vector3(Random.Range(-8,5),20,0);
        newEnemy.GetComponent<EnemyController>().SetScriptable(enemyScriptable);
    }

}
