
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtendedButtons;
using System.Linq;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private TMP_Text m_ResultTitle;
    [SerializeField] private Transform m_EnemyBlockParent;
    [SerializeField] private GameObject m_EnemyBlockPrefab;
    [SerializeField] private Button2D m_CloseBtn;
    private bool m_IsWin = false;
    private List<int> m_AllKilledEnemy = new List<int>();
    private Dictionary<int,EnemyBlockController> m_AllEnemyBlock = new Dictionary<int,EnemyBlockController>();
    
    private void Start()
    {
        m_Self.SetActive(false);
        m_CloseBtn.onClick.AddListener(OnClickBackFromResult);
    }

    // TODO : record dead enemy
    public void DeadEnemyId(int id){
        m_AllKilledEnemy.Add(id);
    }

    public void Init(bool isWin){
        m_Self.SetActive(true);
        m_IsWin = isWin;
        m_ResultTitle.text = isWin?"Coast Clear":"Defeated";
        var allenemy = MainGameManager.GetInstance().GetAllEnemy();

        for (int i = 0; i < m_EnemyBlockParent.childCount; i++)
        {
            Destroy(m_EnemyBlockParent.GetChild(i).gameObject);
        }
        foreach (var item in m_AllKilledEnemy.Distinct())
        {
            // spawn enemy block
            var newEnemyBlock = Instantiate(m_EnemyBlockPrefab,m_EnemyBlockParent );
            var enemyScriptable = allenemy.Find(x=>x.Id==item);
            var enemyBlockController = newEnemyBlock.GetComponent<EnemyBlockController>();
            m_AllEnemyBlock.Add(item,enemyBlockController);
            enemyBlockController.Init(enemyScriptable);
            enemyBlockController.SetText( m_AllKilledEnemy.Where(x => x == item).Count().ToString() );
        }
    }

    private void ShowDialogOnEndGame(){
        if(m_IsWin)
            MapManager.GetInstance().ShowEndDefenceDialog();
    }
    
    public void OnClickBackFromResult(){
        MainGameManager.GetInstance().LoadSceneWithTransition("Map",ShowDialogOnEndGame);
        // TODO : Hard Coded , try change it later
        if(m_IsWin && MainGameManager.GetInstance().GetSelectedLocation().Id == 17){
            MainGameManager.GetInstance().SaveData<int>("RocketLauncher"+8.ToString(),0);
            MainGameManager.GetInstance().SaveData<int>("Minigun"+9.ToString(),0);
        }
    }
}
