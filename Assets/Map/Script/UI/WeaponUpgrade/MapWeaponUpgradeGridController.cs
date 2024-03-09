
using System.Collections;
using ExtendedButtons;
using UnityEngine;
using UnityEngine.UI;
public class MapWeaponUpgradeGridController : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Image m_WeaponImage;
    [SerializeField] private Button2D m_Btn;
    [SerializeField] private GameObject m_Lock;
    private UnityEngine.Events.UnityAction<GunScriptable> m_OnClickAction;
    private GunScriptable m_GunOwnership;

    public void Init(WeaponUpgradeGridConfig config){
        m_GunOwnership = config.gunScriptsble;
        m_WeaponImage.sprite = config.gunScriptsble.DisplayImage;
        m_OnClickAction = config.onClickAction;

        MainGameManager.GetInstance().AddOnClickBaseAction(m_Btn, m_Btn.GetComponent<RectTransform>());
        m_Btn.onClick.AddListener(()=>{
            m_OnClickAction(m_GunOwnership);
            StartCoroutine(ClickSound());
            });
        m_Lock.SetActive(config.isLock);
    }

    private IEnumerator ClickSound(){
        // TODO :? MainGameManager.GetInstance().OnClickStartSound();
        yield return new WaitForSeconds(0.07f);
        // TODO :? MainGameManager.GetInstance().OnClickEndSound();
    }

}



public class WeaponUpgradeGridConfig
{
    public bool isLock;
    public GunScriptable gunScriptsble;
    public UnityEngine.Events.UnityAction<GunScriptable> onClickAction;
}
