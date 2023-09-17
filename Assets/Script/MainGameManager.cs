using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainGameNameSpace;
using UnityEngine.SceneManagement;
using System;



namespace MainGameNameSpace
{
    [System.Serializable]
    public class WeaponOwnership
    {
        public GunScriptable Gun;
        public bool IsOwned = false;
    }
}

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    [SerializeField] private float m_Volume = 0.75f;
    [SerializeField] private float m_AimSensitivity = 0.5f;

    [SerializeField] private List<GunScriptable> m_AllSelectedWeapon = new List<GunScriptable>();

    [SerializeField] private List<WeaponOwnership> m_AllWeaponOwnership = new List<WeaponOwnership>();
    
    [SerializeField]private float m_WallCurrentHp = 1000;
    [SerializeField]private float m_WallMaxHp = 1000;


    public static MainGameManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameObject().AddComponent<MainGameManager>();
        }
        return m_Instance;
    }

    public List<WeaponOwnership> GetAllWeaponOwnership()
    {
        return m_AllWeaponOwnership;
    }

    public List<GunScriptable> GetAllSelectedWeapon()
    {
        return m_AllSelectedWeapon;
    }


    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {

    }

    public void SetBaseDefenceScene(MapLocationScriptable location){
        SceneManager.LoadScene("baseDefence");
        StartCoroutine(StartWave(location));
    }

    private IEnumerator StartWave(MapLocationScriptable location){
        yield return null;
        BaseDefenceManager.GetInstance().StartWave(location);

    }

    public float GetWallCurHp(){
        return m_WallCurrentHp;
    }

    public float GetWallMaxHp(){
        return m_WallMaxHp;
    }

    public void SetMapScene(){
        SceneManager.LoadScene("Map");
    }

    public void ChangeWallHp(float changes){
        m_WallCurrentHp += changes;
        if(m_WallCurrentHp<0){
            m_WallCurrentHp = 0;
        }else if(m_WallCurrentHp>m_WallMaxHp){
            m_WallCurrentHp = m_WallMaxHp;
        }
    }



    public void ChangeSelectedWeapon(int slotIndex, GunScriptable newWeapon)
    {
        if (m_AllSelectedWeapon.Count > slotIndex && slotIndex >= 0)
            m_AllSelectedWeapon[slotIndex] = newWeapon;
    }


    public void SetAimSensitivity(float sensitivity)
    {
        m_AimSensitivity = sensitivity;
    }

    public void SetVolume(float volume)
    {
        m_Volume = volume;
    }
    public float GetVolume()
    {
        return m_Volume;
    }
    public float GetAimSensitivity()
    {
        return m_AimSensitivity;
    }

    public void AddNewAudioSource(AudioSource audioSource)
    {
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;

        UpdateVolume();
    }

    public void UpdateVolume()
    {
        List<int> toBeRemove = new List<int>();
        for (int i = 0; i < m_AllAudioSource.Count; i++)
        {
            if (m_AllAudioSource[i] != null)
            {
                m_AllAudioSource[i].volume = m_Volume;
            }
            else
            {
                toBeRemove.Add(i);
            }
        }
        for (int i = 0; i < toBeRemove.Count; i++)
        {
            m_AllAudioSource.RemoveAt(toBeRemove[i] - i);
        }
    }

}
