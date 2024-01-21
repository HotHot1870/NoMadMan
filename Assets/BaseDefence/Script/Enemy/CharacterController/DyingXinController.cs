using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingXinController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private EnemyScriptable m_DyingXinScriptable;

    public void Init(Vector3 destination){
        BaseDefenceManager.GetInstance().AddEnemyToList(m_Self.transform);
        Destination = destination;
        Scriptable = m_DyingXinScriptable;
    }

    public void SetDyingXinScriptable(EnemyScriptable dyingXin){
        m_DyingXinScriptable = dyingXin;

    }
    void Start(){
        OnDeadAction+=SetResult;
        m_Animator.Play("Crawl");
    }

    void Update(){
        // move forward
        float moveDistance = m_DyingXinScriptable.MoveSpeed * Time.deltaTime ;
        m_Self.transform.position = Vector3.MoveTowards(
            m_Self.transform.position, Destination, moveDistance);

        if(Vector3.Distance(m_Self.transform.position, Destination)<0.25f){
            OnDead();
        }
    }

    private void SetResult(){
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetResultPanel(true);
    }
}
