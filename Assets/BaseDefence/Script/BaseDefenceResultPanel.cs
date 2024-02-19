
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
        
    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioPlayer;
    [SerializeField] private AudioClip m_SpawnSound;
    [SerializeField] private AudioClip m_ShotSound;
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

    // TODO : pp20 reload clip image missing

    public void Init(bool isWin){
        if(m_Self.activeSelf)
            return;

        // End game Music
        if (!isWin)
        {
            // lose
            MainGameManager.GetInstance().ChangeBGM(BGM.Defeated,0f);
        }else{
            // win BGM
            MainGameManager.GetInstance().ChangeBGM(BGM.None,1f);

        }
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
        if(isWin)
            StartCoroutine(ShowEnemyBlockOneByOne());
    }
     
    private IEnumerator ShowEnemyBlockOneByOne(){
        yield return new WaitForSeconds(1.25f);
        var allenemy = MainGameManager.GetInstance().GetAllEnemy();
        float totalGain = 0;
        foreach (var item in m_AllKilledEnemy.Distinct())
        {
            // sound spawn enemy killed block 
            m_AudioPlayer.PlayOneShot(m_SpawnSound);
            // spawn enemy block
            var newEnemyBlock = Instantiate(m_EnemyBlockPrefab,m_EnemyBlockParent );
            var enemyScriptable = allenemy.Find(x=>x.Id==item.Id);
            var enemyBlockController = newEnemyBlock.GetComponent<EnemyBlockController>();
            m_AllEnemyBlock.Add(item.Id,enemyBlockController);
            enemyBlockController.Init(enemyScriptable);
            yield return new WaitForSeconds(0.15f);
            int currentCount = 1;
            enemyBlockController.SetText( currentCount.ToString() );

            int totalKillCount = m_AllKilledEnemy.Where(x => x == item).Count();
            while (currentCount<totalKillCount)
            {
                m_AudioPlayer.PlayOneShot(m_SpawnSound);
                currentCount++;
                enemyBlockController.SetText( currentCount.ToString() );
                enemyBlockController.PlayGrowAnimation();
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        foreach (var item in m_AllKilledEnemy)
        {
            totalGain += item.GooOnKill;
        }


        float extra = BaseDefenceManager.GetInstance().GetLocationScriptable().ExtraReward;
        m_DifficultyBouns.text = "Difficulty Bouns : "+ extra*100f+"% (+" + (extra*totalGain).ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        yield return new WaitForSeconds(0.45f);

        float hpPresentage = BaseDefenceManager.GetInstance().GetCurHp()/BaseDefenceManager.GetInstance().GetMaxHp();
        float gooReduceByHp = totalGain * (1f-hpPresentage);
        m_HpLeft.text = "HP : "+hpPresentage*100f+"% (-" + gooReduceByHp.ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        yield return new WaitForSeconds(0.45f);

        
        totalGain = Mathf.Clamp(totalGain + extra*totalGain - gooReduceByHp,0f,99999999f) ;
        float total = MainGameManager.GetInstance().GetGooAmount()+totalGain;
        m_TotalGainText.text = "Total : "+ total.ToString("0.#") +"(+"+totalGain.ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        
        
        MainGameManager.GetInstance().ChangeGooAmount(totalGain);
    }

    private void ShowDialogOnEndGame(){
        if(m_IsWin)
            MapManager.GetInstance().ShowEndDefenceDialog();
    }
    
    public void OnClickBackFromResult(){
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",ShowDialogOnEndGame);

        // TODO : hard coded , dont get audio clip from other , store in main game controller
        MainGameManager.GetInstance().ChangeBGM(BGM.Map);
        // TODO : Hard Coded , try change it later
        // unlock gun by win 
        if(m_IsWin && MainGameManager.GetInstance().GetSelectedLocation().Id == 17){
            MainGameManager.GetInstance().SaveData<int>("WeaponUnlock"+8.ToString(),0);
            MainGameManager.GetInstance().SaveData<int>("WeaponUnlock"+9.ToString(),0);
        }
    }
}
