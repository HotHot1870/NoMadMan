using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using Unity.VisualScripting;

[Serializable]
public class ShootSoundAndName
{
    public string clipName;
    public AudioClip audioClip;
}


public class ReadCsv : MonoBehaviour
{
    [SerializeField] private string m_ResourcesPath = "";
    [SerializeField] private string m_ScriptablePath = "";
    [SerializeField] private MainGameManager m_MainGameManager;
    private Dictionary<int,string> m_AllWaveName = new Dictionary<int, string>();
    private Dictionary<int,string> m_AllGun = new Dictionary<int, string>();
    [SerializeField] private List<ShootSoundAndName> m_AllShootSound = new List<ShootSoundAndName>();
#if UNITY_EDITOR
    //[MenuItem("Tool/ReadCSV")]
    [EButton("SaveCsv")]
    private void SaveCsvFile(){
        StartCoroutine(GetCsvFromGoogle());
    }
    [EButton("ReadCsv")]
    private void ReadCsvFile(){

        ReadGunCSV();
        ReadCSVLocation();
        ReacCSVWave();
        ReadCSVEnemy();
        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private void ReadCSVEnemy(){
        // enemy
        List<EnemyScriptable> allEnemy = new List< EnemyScriptable>(); 
        if(Resources.Load<TextAsset>("CSV/Enemy") == null){
            Debug.LogError(m_ResourcesPath+"/Enemy.csv is null" );
            return;
        }
        
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Enemy");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/Enemy").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 8;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            // create scriptable 
            EnemyScriptable Enemy = ScriptableObject.CreateInstance<EnemyScriptable>();
            string displayName = contents[index+1].Trim();
            AssetDatabase.CreateAsset(Enemy, m_ScriptablePath+"/Enemy/"+displayName.Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Enemy.Id = int.Parse(contents[index]);
            Enemy.DisplayName = displayName;
            Enemy.MaxHp = float.Parse(contents[index+2]);
            Enemy.MoveSpeed = float.Parse(contents[index+3]);
            Enemy.Damage = float.Parse(contents[index+4]);
            Enemy.AttackDelay = float.Parse(contents[index+5]);
            Enemy.DangerValue = float.Parse(contents[index+6]);
            Enemy.GooOnKill = float.Parse(contents[index+7]);
            // TODO : Enemy.DisplayImage = Resources.Load<Sprite>("Enemy/DisplayImage/"+displayName.Replace(" ", ""));
            Enemy.Prefab = Resources.Load<GameObject>("Enemy/Prefab/"+displayName.Replace(" ", ""));


            allEnemy.Add(Enemy);
            EditorUtility.SetDirty(Enemy);
            m_MainGameManager.SetAllEnemy(allEnemy);
        }   
    }

    private void ReacCSVWave() {
        //Wave
        List<WaveScriptable> allWave = new List< WaveScriptable>(); 
        if(Resources.Load<TextAsset>("CSV/Wave") == null){
            Debug.LogError(m_ResourcesPath+"/Wave.csv is null" );
            return;
        }
        
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Wave");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/Wave").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 6;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            // create scriptable 
            WaveScriptable Wave = ScriptableObject.CreateInstance<WaveScriptable>();
            string displayName = m_AllWaveName[int.Parse(contents[index])];
            AssetDatabase.CreateAsset(Wave, m_ScriptablePath+"/Wave/"+displayName+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Wave.Id = int.Parse(contents[index]);
            Wave.NormalWavesCount = int.Parse(contents[index+1]);
            Wave.NormalWavesStrength = float.Parse(contents[index+2]);
            List<int> normalEnemyList = new List<int>();
            foreach (var item in contents[index+3].Split('|'))
            {
                normalEnemyList.Add(int.Parse(item));
            }
            Wave.NormalWaveEnemy = normalEnemyList;

            Wave.FinalWaveStrength = float.Parse(contents[index+4]);
            
            List<int> finalEnemyList = new List<int>();
            foreach (var item in contents[index+5].Split('|'))
            {
                finalEnemyList.Add(int.Parse(item));
            }
            Wave.FinalWaveEnemy = finalEnemyList;
            allWave.Add(Wave);
            EditorUtility.SetDirty(Wave);
        }    

        m_MainGameManager.SetAllWave(allWave);
    }

    private void ReadCSVLocation(){
        // Location
        if(Resources.Load<TextAsset>("CSV/Location") == null){
            Debug.LogError(m_ResourcesPath+"/Location.csv is null" );
            return;
        }
        List<MapLocationScriptable> allLocation = new List<MapLocationScriptable>();
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Location");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string json = Resources.Load<TextAsset>("CSV/Location").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 6;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            // create scriptable 
            MapLocationScriptable location = ScriptableObject.CreateInstance<MapLocationScriptable>();
            AssetDatabase.CreateAsset(location, m_ScriptablePath+"/Location/"+contents[index+1].Trim().Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            location.Id = int.Parse(contents[index]);
            location.DisplayName = contents[index+1].Trim();
            location.WaveId = int.Parse(contents[index+2]);
            float posX = float.Parse(contents[index+3].Split('|')[0]);
            float posZ = float.Parse(contents[index+3].Split('|')[1]);
            location.Pos = new Vector3(posX,2f,posZ);
            location.RewardGunId = int.Parse(contents[index+4]);
            location.Prefab = Resources.Load<GameObject>("Location/"+contents[index+1].Trim().Replace(" ", ""));

            allLocation.Add(location);
            if(!m_AllWaveName.ContainsKey(int.Parse(contents[index+2])))
                m_AllWaveName.Add(int.Parse(contents[index+2]),contents[index+1].Trim().Replace(" ", ""));
            
            

            EditorUtility.SetDirty(location);
        }
            m_MainGameManager.SetAllLocation(allLocation);
    }

    private void ReadGunCSV(){
        // Gun
        Dictionary<int,GunScriptable> allGuns = new Dictionary<int, GunScriptable>(); 
        List<MainGameNameSpace.WeaponOwnership> allWeaponAndOwnership = new List<MainGameNameSpace.WeaponOwnership>();

        if(Resources.Load<TextAsset>("CSV/Gun") == null){
            Debug.LogError(m_ResourcesPath+"/CSV//Gun.csv is null" );
            return;
        }
        
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Gun");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/Gun").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 13;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            // create scriptable 
            GunScriptable gunStat = ScriptableObject.CreateInstance<GunScriptable>();
            string displayName = contents[index+1].Trim();
            AssetDatabase.CreateAsset(gunStat, m_ScriptablePath+"/Gun/"+displayName.Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            gunStat.Id = int.Parse(contents[index]);
            gunStat.DisplayName = displayName;
            gunStat.ShakeAmount = float.Parse(contents[index+2]);

            gunStat.DisplayImage = Resources.Load<Sprite>("Gun/DisplayImage/"+displayName.Replace(" ", ""));
            gunStat.FPSPrefab = Resources.Load<GameObject>("Gun/3DModel/"+displayName.Replace(" ", ""));

            var stats = new GunScriptableStats();
            stats.DamagePerPellet =  float.Parse(contents[index+3]);
            stats.PelletPerShot =  int.Parse(contents[index+4]);
            stats.ClipSize =  int.Parse(contents[index+5]);
            stats.IsSemiAuto = contents[index+6].Trim() == "y";
            stats.FireRate =  float.Parse(contents[index+7]);
            stats.Accuracy =  float.Parse(contents[index+8]);
            stats.Handling =  float.Parse(contents[index+9]);
            stats.Recoil =  float.Parse(contents[index+10]);

            // is AP contents[index+11]
            gunStat.ShootSound = m_AllShootSound.Single(x=>x.clipName == contents[index+12].Trim().Replace(" ", "")).audioClip;

            gunStat.ReloadScriptable = AssetDatabase.LoadAssetAtPath<GunReloadScriptable>(m_ScriptablePath+"/Reload/"+displayName.Replace(" ", "")+"_Reload.asset");

            gunStat.GunStats = stats;
            allGuns.Add(gunStat.Id,gunStat);
            var weaponOwnership = new MainGameNameSpace.WeaponOwnership();
            weaponOwnership.Gun = gunStat;
            weaponOwnership.IsOwned = index/collumeCount<=4;
            allWeaponAndOwnership.Add(weaponOwnership);
            EditorUtility.SetDirty(gunStat);
        }
        m_MainGameManager.SetAllWeapon(allWeaponAndOwnership);
    }



    private IEnumerator GetCsvFromGoogle(){

        // Gun
        UnityWebRequest www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=0&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/Gun.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/Gun.csv",json);
 
        }

        // Enemy
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=1847254529&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/Enemy.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/Enemy.csv",json);
 
        }

        
        // Wave
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=109585683&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/Wave.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/Wave.csv",json);
 
        }        
        
        // Location
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=83501182&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/Location.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/Location.csv",json);
 
        }

        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

}
