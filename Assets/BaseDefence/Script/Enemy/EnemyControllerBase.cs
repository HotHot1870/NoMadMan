using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyControllerInitConfig{
    public EnemyScriptable scriptable;
    public Vector3 destination;
    public Vector3 cameraPos;
    public Vector3 spawnPos;
}

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action OnDeadAction = null;
    protected bool IsThisDead = false;
    protected Vector3 CameraPos;
    protected Vector3 Destination;
    [SerializeField] protected ParticleSystem m_ExplodeHitParticle;
 
    
    
    public virtual void Init(EnemyControllerInitConfig config){
        Scriptable = config.scriptable;
        Destination = config.destination;
        CurHp = Scriptable.MaxHp;
        CameraPos = config.cameraPos;
        this.transform.position = config.spawnPos;
        BaseDefenceManager.GetInstance().AddEnemyToList(this.transform);
    }

    /// <summary>
    /// use negative for damage
    /// </summary>
    public void ChangeHp(float changes){
        if( IsThisDead )
            return;

        CurHp += changes;
        CurHp = Mathf.Clamp(CurHp,0f,Scriptable.MaxHp);
        if( CurHp<=0 ){
            // dead
            IsThisDead = true;
            OnDead();
        }
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
