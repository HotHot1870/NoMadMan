using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RemnantModleAndAnimation
{
    public string m_Name;
    public GameObject m_Self;
    public Animator m_Animtor;
    public SkinnedMeshRenderer m_Renderer;
}

public class RemnantController : EnemyControllerBase
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private float m_AttackAnimationStartUp = 0.45f;
    [SerializeField] private List<RemnantModleAndAnimation> m_AllModleAndAnimation = new List<RemnantModleAndAnimation>();
    private SkinnedMeshRenderer m_TargetRenderer = null;
    private Animator m_TargetAnimator = null;
    private float m_RandomSpeedMod = 1;
    private bool m_CanAttack = false;
    private float m_FallingBackTime =0;

    public override void Init(EnemyControllerInitConfig config)
    {
        base.Init(config);
    }

    private IEnumerator Start() {
        // set random modle
        int randomIndex = UnityEngine.Random.Range(0,m_AllModleAndAnimation.Count);
        for (int i = 0; i < m_AllModleAndAnimation.Count; i++)
        {
            if(i==randomIndex){
                m_AllModleAndAnimation[i].m_Self.SetActive(true);
                m_TargetRenderer = m_AllModleAndAnimation[i].m_Renderer;
                m_TargetAnimator = m_AllModleAndAnimation[i].m_Animtor;
            }else{
                Destroy(m_AllModleAndAnimation[i].m_Self);
                m_AllModleAndAnimation.RemoveAt(i);
                i--;
                randomIndex--;
            }
        }

        m_RandomSpeedMod = UnityEngine.Random.Range(0.6f,1.6f);

        //m_Self.transform.LookAt(new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z));

        m_TargetAnimator.Play("Move");
        yield return null;
        m_Self.transform.LookAt(new Vector3(Destination.x,m_Self.transform.position.y,Destination.x));
        
    }

    private void Update() {
        if( IsThisDead )
            return;

         if(!m_CanAttack&&Vector3.Distance(m_Self.transform.position , Destination)<Scriptable.MoveSpeed * Time.deltaTime*2f){
            // close enough for attack 

            m_TargetAnimator.speed = 1;
            m_TargetAnimator.SetBool("IsStopped",true);
            // first attack has no non-animation delay
            m_CanAttack = true;
            StartCoroutine(TrunAndLookAndCamera());

        }else if(m_FallingBackTime <=0){
            // move forward
            float locationSpeedMod = BaseDefenceManager.GetInstance().GetLocationScriptable().SpeedMutation/100f +1f;
            m_TargetAnimator.SetFloat("Speed",Scriptable.MoveSpeed*m_RandomSpeedMod*locationSpeedMod);
            float moveDistance = Scriptable.MoveSpeed * Time.deltaTime *m_RandomSpeedMod*locationSpeedMod;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);

        }else{
            // move backward
            m_TargetAnimator.SetFloat("Speed",-0.5f);
            float moveDistance = -0.5f * Time.deltaTime *m_RandomSpeedMod;
            m_Self.transform.position = Vector3.MoveTowards(
                m_Self.transform.position, Destination, moveDistance);
        }
    }
    private IEnumerator TrunAndLookAndCamera(){
        float passTime = 0;
        float duration = 0.25f;
        while (passTime<duration)
        {
            yield return null;
            passTime += Time.deltaTime;
            var lookPos = Vector3.Lerp(
                new Vector3(Destination.x,m_Self.transform.position.y,Destination.x),
                new Vector3(CameraPos.x,m_Self.transform.position.y,CameraPos.z),
                passTime / duration
            );
            m_Self.transform.LookAt(lookPos);
        }
        StartCoroutine(FirstAttack());

    }

    private IEnumerator FallingBack(){
        float duration = 1;
        while (m_FallingBackTime < duration)
        {
            m_FallingBackTime += Time.deltaTime;
            yield return null;
        }
        
    }

    private IEnumerator FirstAttack(){
        if(IsThisDead)
            yield break;
        m_TargetAnimator.Play("Attack");
            // attack animation delay
        yield return new WaitForSeconds(m_AttackAnimationStartUp);
        if(IsThisDead)
            yield break;
        BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage);
        StartCoroutine(Attack());
    }

    public IEnumerator Attack(){
        while (!IsThisDead)
        {
            m_TargetAnimator.speed = 1;
            // wait delay
            //m_TargetAnimator.Play("Idle");
            yield return new WaitForSeconds(Scriptable.AttackDelay);
            if(IsThisDead)
                yield break;
            
            m_TargetAnimator.Play("Attack");
            // attack animation delay
            yield return new WaitForSeconds(m_AttackAnimationStartUp);
            if(IsThisDead)
                yield break;
            BaseDefenceManager.GetInstance().OnPlayerHit(Scriptable.Damage);
        }

    }

    protected override void OnDead(){
        base.OnDead();

        m_TargetAnimator.Play("Dead");
        Destroy(m_Self,1);
    }

}
