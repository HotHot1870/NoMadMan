using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainGameNameSpace;
using UnityEngine.SceneManagement;
using System;
using System.Linq;



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

    [SerializeField] private List<WeaponOwnership> m_AllWeapon = new List<WeaponOwnership>();
    [SerializeField] private List<MapLocationScriptable> m_AllLocation = new List<MapLocationScriptable>();
    [SerializeField] private List<WaveScriptable> m_AllWave = new List<WaveScriptable>();
    [SerializeField] private List<EnemyScriptable> m_AllEnemy = new List<EnemyScriptable>();
    
    [SerializeField]private float m_WallCurrentHp = 1000;
    [SerializeField]private float m_WallMaxHp = 1000;
    [SerializeField]private float m_GooAmount=10000;


    public static MainGameManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameObject().AddComponent<MainGameManager>();
        }
        return m_Instance;
    }

    public List<WeaponOwnership> GetAllWeapon()
    {
        return m_AllWeapon;
    }

    public List<MapLocationScriptable> GetAllLocation(){
        return m_AllLocation;
    }

    public List<GunScriptable> GetAllSelectedWeapon()
    {
        if(m_AllSelectedWeapon.Count>4){
            m_AllSelectedWeapon = new List<GunScriptable>(m_AllSelectedWeapon.GetRange(0,4));
        }
        return m_AllSelectedWeapon;
    }

    
    public List<WaveScriptable> GetAllWave() {
        return m_AllWave;
    }

    
    public List<EnemyScriptable> GetAllEnemy(){
        return m_AllEnemy;
    }

    public void SetAllWeapon(List<WeaponOwnership> allWeapon){
        // TODO : check Ownership
        m_AllWeapon = allWeapon;
        m_AllSelectedWeapon.Clear();

        for (int i = 0; i < m_AllWeapon.Count; i++)
        {
            if(m_AllWeapon[i].IsOwned){
                m_AllSelectedWeapon.Add(m_AllWeapon[i].Gun);
            }
            if(m_AllSelectedWeapon.Count>=4)
                break;
        } 
        if(m_AllSelectedWeapon.Count<4){
            Debug.Log("Not enough owned gun");
        }
    }

    public void SetAllLocation(List<MapLocationScriptable> allLocation){
        m_AllLocation = allLocation;
    }

    public void SetAllWave(List<WaveScriptable> allWave) {
        m_AllWave = allWave;
    }

    public void SetAllEnemy(List<EnemyScriptable> allEnemy){
        m_AllEnemy = allEnemy;
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


    public void UnlockGun(int id){
        var tmp = m_AllWeapon.Find(x=>x.Gun.Id==id);
        tmp.IsOwned = true;
    }


    public void ChangeGooAmount(float gooChanges){
        m_GooAmount += gooChanges;
    }

    public float GetGooAmount(){
        return m_GooAmount;
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
