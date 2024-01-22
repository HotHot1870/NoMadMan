
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtendedButtons;
using System.Linq;
using System.Collections;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private TMP_Text m_ResultTitle;
    [SerializeField] private TMP_Text m_HpLeft;
    [SerializeField] private TMP_Text m_DifficultyBouns;
    [SerializeField] private TMP_Text m_TotalGainText;
    [SerializeField] private Transform m_EnemyBlockParent;
    [SerializeField] private GameObject m_EnemyBlockPrefab;
    [SerializeField] private Button2D m_CloseBtn;
    private bool m_IsWin = false;
    private List<EnemyScriptable> m_AllKilledEnemy = new List<EnemyScriptable>();
    private Dictionary<int,EnemyBlockController> m_AllEnemyBlock = new Dictionary<int,EnemyBlockController>();
    
    private void Start()
    {
        m_Self.SetActive(false);
        m_CloseBtn.onClick.AddListener(OnClickBackFromResult);
    }

    // record dead enemy
    public void RecordDeadEnemy(EnemyScriptable enemyScriptable){
        m_AllKilledEnemy.Add( enemyScriptable);
    }

    public void Init(bool isWin){
        m_Self.SetActive(true);
        m_IsWin = isWin;
        m_ResultTitle.text = isWin?"Coast Clear":"Defeated";
        m_DifficultyBouns.text = "";
        m_HpLeft.text = "";

        for (int i = 0; i < m_EnemyBlockParent.childCount; i++)
        {
            Destroy(m_EnemyBlockParent.GetChild(i).gameObject);
        }
        m_TotalGainText.text = "";
        StartCoroutine(ShowEnemyBlockOneByOne());
    }
     
    private IEnumerator ShowEnemyBlockOneByOne(){
        yield return new WaitForSeconds(2f);
        var allenemy = MainGameManager.GetInstance().GetAllEnemy();
        float totalGain = 0;
        foreach (var item in m_AllKilledEnemy.Distinct())
        {
            // TODO : sound spawn enemy killed block 
            // spawn enemy block
            var newEnemyBlock = Instantiate(m_EnemyBlockPrefab,m_EnemyBlockParent );
            var enemyScriptable = allenemy.Find(x=>x.Id==item.Id);
            var enemyBlockController = newEnemyBlock.GetComponent<EnemyBlockController>();
            m_AllEnemyBlock.Add(item.Id,enemyBlockController);
            enemyBlockController.Init(enemyScriptable);
            yield return new WaitForSeconds(0.2f);
            int currentCount = 1;
            enemyBlockController.SetText( currentCount.ToString() );

            int totalKillCount = m_AllKilledEnemy.Where(x => x == item).Count();
            while (currentCount<totalKillCount)
            {
                // TODO : play grow animation
                currentCount++;
                enemyBlockController.SetText( currentCount.ToString() );
                yield return new WaitForSeconds(0.2f);
            }
        }
        float hpPresentage = BaseDefenceManager.GetInstance().GetCurHp()/BaseDefenceManager.GetInstance().GetMaxHp()*100f;
        m_HpLeft.text = "HP : "+hpPresentage+"%";
        float extra = BaseDefenceManager.GetInstance().GetLocationScriptable().ExtraReward;
        m_DifficultyBouns.text = "Difficulty Bouns : "+ extra*100f+"%";
        
        foreach (var item in m_AllKilledEnemy)
        {
            totalGain += item.GooOnKill;
        }
        totalGain = totalGain * hpPresentage/100f * (1f+extra);
        m_TotalGainText.text = "Total : "+totalGain.ToString("0.#");
    }

    private void ShowDialogOnEndGame(){
        if(m_IsWin)
            MapManager.GetInstance().ShowEndDefenceDialog();
    }
    
    public void OnClickBackFromResult(){
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",ShowDialogOnEndGame);
        // TODO : Hard Coded , try change it later
        // unlock gun by win 
        if(m_IsWin && MainGameManager.GetInstance().GetSelectedLocation().Id == 17){
            MainGameManager.GetInstance().SaveData<int>("RocketLauncher"+8.ToString(),0);
            MainGameManager.GetInstance().SaveData<int>("Minigun"+9.ToString(),0);
        }
    }
}
