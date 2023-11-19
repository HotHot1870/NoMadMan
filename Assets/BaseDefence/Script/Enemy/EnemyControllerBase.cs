using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EnemyControllerInitConfig{
    public EnemyScriptable scriptable;
    public Vector3 destination;
    public Vector3 cameraPos;
    public Vector3 spawnPos;
    public Camera camera;
}

public abstract class EnemyControllerBase : MonoBehaviour
{
    public Canvas HpCanvas;
    public Image HpBar;
    public GameObject HpParent;
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action OnDeadAction = null;
    protected bool IsThisDead = false;
    protected Vector3 CameraPos;
    protected Vector3 Destination;
    protected Camera MainCamera;
    [SerializeField] protected ParticleSystem m_ExplodeHitParticle;
 
    
    
    public virtual void Init(EnemyControllerInitConfig config){
        Scriptable = config.scriptable;
        Destination = config.destination;
        CurHp = Scriptable.MaxHp;
        CameraPos = config.cameraPos;
        MainCamera =config.camera;
        this.transform.position = config.spawnPos;
        HpCanvas.worldCamera = config.camera;
        BaseDefenceManager.GetInstance().AddEnemyToList(this.transform);
        HpParent.SetActive(false);
    }

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        if( IsThisDead )
            return;

        CurHp += changes;
        
        HpParent.SetActive(true);
        HpBar.fillAmount = CurHp / Scriptable.MaxHp;
        StartCoroutine(HideHp());
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        if( CurHp<=0 ){
            // dead
            IsThisDead = true;
            HpParent.SetActive(false);
            OnDead();
        }
    }

    private IEnumerator HideHp(){
        yield return new WaitForSeconds(3);
        HpParent.SetActive(false);
    }

    public void ExplodeHitParticle(){
        var explodeHitParticle = Instantiate(m_ExplodeHitParticle);
        explodeHitParticle.transform.position = this.transform.position;
        explodeHitParticle.Play();
    }

    public bool IsDead(){
        return IsThisDead;
    }

    public EnemyScriptable GetScriptable(){
        return Scriptable;
    }


    protected virtual void OnDead(){
        BaseDefenceManager.GetInstance().RemoveDeadEnemyFromList(this.transform);
        MainGameManager.GetInstance().ChangeGooAmount(Scriptable.GooOnKill);
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetGooText();
        OnDeadAction?.Invoke();
    }
}
