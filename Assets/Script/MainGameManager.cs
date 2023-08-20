using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainGameNameSpace;



namespace MainGameNameSpace
{
    [System.Serializable]
    public class WeaponOwnership
    {
        public GunScriptable Gun;
        public bool IsOwned = false;
    }
    [System.Serializable]
    public class ResourcesRecord
    {
        public float Raw = 0;
        public float Scrap = 0;
        public float Chem = 0;
        public float Electronic = 0;
        public int Bot = 0;
        public float Heat = 0;

        public ResourcesRecord GetReverse()
        {
            var ans = new ResourcesRecord
            {
                Raw = Raw * -1,
                Scrap = Scrap * -1,
                Chem = Chem * -1,
                Electronic = Electronic * -1,
                Bot = Bot * -1,
                Heat = Heat * -1,
            };
            return ans;
        }

        public bool IsEnough(ResourcesRecord record)
        {
            return Raw >= record.Raw && Scrap >= record.Scrap && Chem >= record.Chem &&
                 Electronic >= record.Electronic && Bot >= record.Bot && Heat >= record.Heat;
        }

        public ResourcesRecord Multiply(float multiplyBy){
            return new ResourcesRecord{
                Raw = Raw * multiplyBy,
                Scrap = Scrap * multiplyBy,
                Chem = Chem * multiplyBy,
                Electronic = Electronic * multiplyBy,
                Bot = (int) (Bot * multiplyBy),
                Heat = Heat * multiplyBy,
            };

        }

        public void Change(ResourcesRecord record)
        {
            Raw += record.Raw;
            Scrap += record.Scrap;
            Chem += record.Chem;
            Electronic += record.Electronic;
            Bot += record.Bot;
            Heat += record.Heat;
        }
    }
}

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    [SerializeField] private float m_Volume = 0.75f;
    [SerializeField] private float m_AimSensitivity = 0.5f;

    [SerializeField] private List<GunScriptable> m_AllSelectedWeapon = new List<GunScriptable>();

    private const string TotalHeatHeader = " - Strong enemy will not spawn if Total heat is lower than the requirement\n" +
        " - High cap at 1000 \n" +
        " - Low cap at 15 \n" +
        " - 30 seconds per wave \n" + // EnemySpawnController >> m_TimePassed 
        " - Random select vaild enemy(s) until it reach Wave heat \n" +
        " - All selected enemy will spawn within the first 20 seconds \n" +
        " - Wave heat = TotalHeat / 3 \n" + // EnemySpawnController >> TotalWave
        " - To next day it player clear all 3 waves";

    //[Header(TotalHeatHeader)]
    //[SerializeField][Range(15f,1000f)] private float m_TotalHeat = 35;

    [SerializeField] private ResourcesRecord m_OwnedResource = new ResourcesRecord();
    [SerializeField] private List<WeaponOwnership> m_AllWeaponOwnership = new List<WeaponOwnership>();
    [SerializeField][Range(2,4)] private int m_WeaponSlotOwned = 2;
    
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
        m_OwnedResource = new ResourcesRecord
        {
            Raw = 5000,
            Scrap = 5000,
            Chem = 5000,
            Electronic = 5000,
            Bot = 10,
            Heat = 35
        };
    }

    private void Start()
    {

    }

    public float GetWallCurHp(){
        return m_WallCurrentHp;
    }

    public float GetWallMaxHp(){
        return m_WallMaxHp;
    }

    public void ChangeWallHp(float changes){
        m_WallCurrentHp += changes;
        if(m_WallCurrentHp<0){
            m_WallCurrentHp = 0;
        }else if(m_WallCurrentHp>m_WallMaxHp){
            m_WallCurrentHp = m_WallMaxHp;
        }
    }


    public int GetWeaponSlotOwned(){
        return m_WeaponSlotOwned;
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


    public float GetHeat()
    {
        if (m_OwnedResource.Heat > 1000)
        {
            m_OwnedResource.Heat = 1000;
        }
        if (m_OwnedResource.Heat < 15)
        {
            m_OwnedResource.Heat = 15;
        }
        return m_OwnedResource.Heat;
    }

    public void AddNewAudioSource(AudioSource audioSource)
    {
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;

        UpdateVolume();
    }

    public int GetOwnedBotCount()
    {
        return m_OwnedResource.Bot;
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

    public void GainResources(ResourcesRecord gain)
    {
        m_OwnedResource.Change(gain);
    }

    public ResourcesRecord GetOwnedResources()
    {
        return m_OwnedResource;
    }

}
