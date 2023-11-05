using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyControllerInitConfig{
    public EnemyScriptable scriptable;
    public Vector3 destination;
    public Vector3 cameraPos;
}

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected EnemyScriptable Scriptable;
    protected float CurHp;
    public Action OnDeadAction = null;
    protected bool IsThisDead = false;
    protected Vector3 CameraPos;
    protected Vector3 Destination;

    
    
    public virtual void Init(EnemyControllerInitConfig config){
        Scriptable = config.scriptable;
        Destination = config.destination;
        CurHp = Scriptable.MaxHp;
        CameraPos = config.cameraPos;
        
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

    public bool IsDead(){
        return IsThisDead;
    }


    protected virtual void OnDead(){
        MainGameManager.GetInstance().ChangeGooAmount(Scriptable.GooOnKill);
        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetGooText();
        OnDeadAction?.Invoke();
    }
}
