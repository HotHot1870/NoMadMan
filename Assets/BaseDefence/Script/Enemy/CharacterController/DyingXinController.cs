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

        if(Vector3.Distance(m_Self.transform.position, Destination)<1f){
            OnDead();
        }
    }

    private void SetResult(){
        StartCoroutine(DeadDelay());
    }
    private IEnumerator DeadDelay(){
        yield return new WaitForSeconds(0.75f);
        BaseDefenceManager.GetInstance().ChangeGameStage( BaseDefenceNameSpace.BaseDefenceStage.Result );
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetResultPanel(true);

    }

}
