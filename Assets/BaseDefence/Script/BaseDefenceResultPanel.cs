
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
    [SerializeField] private TMP_Text m_ScrapGainBouns;
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
    [Header("Bonus")]
    [SerializeField] private AdPanel m_AdPanel;
    private float m_TotalGain = 0;
    
    private void Start()
    {
        m_Self.SetActive(false);
        m_CloseBtn.onClick.AddListener(OnClickBackFromResult);
        MainGameManager.GetInstance().AddOnClickBaseAction(m_CloseBtn,m_CloseBtn.GetComponent<RectTransform>());
        m_CloseBtn.gameObject.SetActive(false);
    }

    // record dead enemy
    public void RecordDeadEnemy(EnemyScriptable enemyScriptable){
        m_AllKilledEnemy.Add( enemyScriptable);
    }

    public void Init(bool isWin){
        if(m_Self.activeSelf)
            return;
        MainGameManager.GetInstance().InitAd();

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
        m_ScrapGainBouns.text = "";

        for (int i = 0; i < m_EnemyBlockParent.childCount; i++)
        {
            Destroy(m_EnemyBlockParent.GetChild(i).gameObject);
        }
        m_TotalGainText.text = "";
        StartCoroutine(ShowEnemyBlockOneByOne());
    }
     
    private IEnumerator ShowEnemyBlockOneByOne(){
        yield return new WaitForSeconds(1.25f);

        var allenemy = MainGameManager.GetInstance().GetAllEnemy();
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
            m_TotalGain+=item.GooOnKill;
            m_ScrapGainBouns.text = "Scrap Gain : "+m_TotalGain;
            enemyBlockController.SetText( currentCount.ToString() );

            int totalKillCount = m_AllKilledEnemy.Where(x => x == item).Count();
            
            while (currentCount<totalKillCount)
            {
                m_AudioPlayer.PlayOneShot(m_SpawnSound);
                m_TotalGain+=item.GooOnKill;
                m_ScrapGainBouns.text = "Scrap Gain : "+m_TotalGain;
                currentCount++;
                enemyBlockController.SetText( currentCount.ToString() );
                enemyBlockController.PlayGrowAnimation();
                yield return new WaitForSeconds(0.15f);
            }
        }


        float extra = BaseDefenceManager.GetInstance().GetLocationScriptable().ExtraReward;
        m_DifficultyBouns.text = "Difficulty Bouns : "+ extra*100f+"% (+" + (extra*m_TotalGain).ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        yield return new WaitForSeconds(0.45f);

        float hpPresentage = BaseDefenceManager.GetInstance().GetCurHp()/BaseDefenceManager.GetInstance().GetMaxHp();
        float gooReduceByHp = m_TotalGain * (1f-hpPresentage);
        m_HpLeft.text = "HP : "+hpPresentage*100f+"% (-" + gooReduceByHp.ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        yield return new WaitForSeconds(0.45f);

        
        m_TotalGain = Mathf.Clamp(m_TotalGain + extra*m_TotalGain - gooReduceByHp,0f,99999999f) ;
        float total = MainGameManager.GetInstance().GetGooAmount()+m_TotalGain;
        m_TotalGainText.text = "Total : "+ total.ToString("0.#") +"(+"+m_TotalGain.ToString("0.#")+")";
        m_AudioPlayer.PlayOneShot(m_ShotSound);
        
        
        m_CloseBtn.gameObject.SetActive(true);
    }

    private void ShowDialogOnEndGame(){
        if(m_IsWin)
            MapManager.GetInstance().ShowEndDefenceDialog();
    }
    
    public void OnClickBackFromResult(){

        if(m_IsWin && MainGameManager.GetInstance().GetSelectedLocation().Id == 20){
            // endgame
            MainGameManager.GetInstance().LoadSceneWithTransition("EndGame");
            MainGameManager.GetInstance().ChangeBGM(BGM.MainMenu);
        }else{
            if(m_IsWin && MainGameManager.GetInstance().IsAdLoaded()){
                // win , show ad panel if 
                m_AdPanel.gameObject.SetActive(true);
                m_AdPanel.Init(m_TotalGain);
            }else{
                // Lose , back to map scene
                MainGameManager.GetInstance().LoadSceneWithTransition("Map",ShowDialogOnEndGame);
                MainGameManager.GetInstance().ChangeBGM(BGM.Map);
            }
        }

        /*
        // unlock gun by win 
        if(m_IsWin && MainGameManager.GetInstance().GetSelectedLocation().Id == 17){
            MainGameManager.GetInstance().SaveData<int>("WeaponUnlock"+8.ToString(),0);
            MainGameManager.GetInstance().SaveData<int>("WeaponUnlock"+9.ToString(),0);
        }*/
    }
}
