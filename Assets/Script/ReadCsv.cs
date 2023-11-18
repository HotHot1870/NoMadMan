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
    //private Dictionary<int,string> m_AllWaveName = new Dictionary<int, string>();
    //private Dictionary<int,string> m_AllGun = new Dictionary<int, string>();
    private EnemyScriptable m_GhostScriptable = null;
    private EnemyScriptable m_PuppetScriptable = null;


    [SerializeField] private List<ShootSoundAndName> m_AllShootSound = new List<ShootSoundAndName>();
#if UNITY_EDITOR
    //[MenuItem("Tool/ReadCSV")]
    [EButton("SaveCsv")]
    private void SaveCsvFile(){
        StartCoroutine(GetCsvFromGoogle());
    }
    [EButton("ReadCsvGun")]
    private void ReadCsvFileGun(){
        ReadGunCSV();

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [EButton("ReadCsvLocation")]
    private void ReadCsvFileLocation(){
        ReadCSVLocation();

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [EButton("ReadCsvWave")]
    private void ReadCsvFileWave(){
        ReacCSVWave();

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [EButton("ReadCsvEnemy")]
    private void ReadCsvFileEenmy(){
        ReadCSVEnemy();

        
        // Puppeteer get scriptable
        var puppeteer = Resources.Load<GameObject>("Enemy/Prefab/Puppeteer");
        var puppeteerController = puppeteer.GetComponent<PuppeteerController>();
        EditorUtility.SetDirty(puppeteerController);
        puppeteerController.SetGhostScriptable(m_GhostScriptable);
        puppeteerController.SetPuppetScriptable(m_PuppetScriptable);

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
        EnemyScriptable ghostScriptable = null; 
        EnemyScriptable puppetScriptable = null; 

        var contents = json.Split('\n',',');
        int collumeCount = 9;
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
            Enemy.ExplodeDamageMod = float.Parse(contents[index+8]);
            Enemy.Prefab = Resources.Load<GameObject>("Enemy/Prefab/"+displayName.Replace(" ", ""));

            //record Ghost scriptable
            if(Enemy.Id == 4){
                m_GhostScriptable = AssetDatabase.LoadAssetAtPath(m_ScriptablePath+"/Enemy/"+displayName.Replace(" ", "")+".asset", typeof(EnemyScriptable)) as EnemyScriptable;
            }

            //record Puppet scriptable
            if(Enemy.Id == 3){
                m_PuppetScriptable = AssetDatabase.LoadAssetAtPath(m_ScriptablePath+"/Enemy/"+displayName.Replace(" ", "")+".asset", typeof(EnemyScriptable)) as EnemyScriptable;
            }

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
        int collumeCount = 7;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            int colume = index;
            // create scriptable 
            WaveScriptable Wave = ScriptableObject.CreateInstance<WaveScriptable>();
            string displayName = contents[colume+1];
            
            AssetDatabase.CreateAsset(Wave, m_ScriptablePath+"/Wave/"+displayName+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Wave.Id = int.Parse(contents[colume]);
            colume++;
            // display name = contents[index +1]
            colume++;
            Wave.NormalWavesCount = int.Parse(contents[colume]);
            colume++;
            Wave.NormalWavesStrength = float.Parse(contents[colume]);
            colume++;
            List<int> normalEnemyList = new List<int>();
            foreach (var item in contents[colume].Split('|'))
            {
                normalEnemyList.Add(int.Parse(item));
            }
            colume++;
            Wave.NormalWaveEnemy = normalEnemyList;

            Wave.FinalWaveStrength = float.Parse(contents[colume]);
            colume++;
            
            List<int> finalEnemyList = new List<int>();
            foreach (var item in contents[colume].Split('|'))
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
            location.Prefab = Resources.Load<GameObject>("Location/"+contents[index+1].Trim().Replace(" ", ""));
            location.WaveId = int.Parse(contents[index+2]);
            float posX = float.Parse(contents[index+3].Split('|')[0]);
            float posZ = float.Parse(contents[index+3].Split('|')[1]);
            location.Pos = new Vector3(posX,0,posZ);
            location.RewardGunId = int.Parse(contents[index+4]);
            location.FortifyCost = int.Parse(contents[index+5]);

            allLocation.Add(location);
            /*
            if(!m_AllWaveName.ContainsKey(int.Parse(contents[index+2])))
                m_AllWaveName.Add(int.Parse(contents[index+2]),contents[index+1].Trim().Replace(" ", ""));*/
            
            

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
            int colume = index;
            // create scriptable 
            GunScriptable gunScriptable = ScriptableObject.CreateInstance<GunScriptable>();

            colume++;
            string displayName = contents[colume].Trim();
            AssetDatabase.CreateAsset(gunScriptable, m_ScriptablePath+"/Gun/"+displayName.Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            gunScriptable.Id = int.Parse(contents[index]);
            gunScriptable.DisplayName = displayName;

            gunScriptable.DisplayImage = Resources.Load<Sprite>("Gun/DisplayImage/"+displayName.Replace(" ", ""));
            gunScriptable.FPSPrefab = Resources.Load<GameObject>("Gun/3DModel/"+displayName.Replace(" ", ""));

            var stats = new GunScriptableStats();
            colume++;
            stats.DamagePerPellet =  float.Parse(contents[colume]);
            colume++;
            stats.PelletPerShot =  int.Parse(contents[colume]);
            colume++;
            stats.ClipSize =  int.Parse(contents[colume]);
            colume++;
            stats.IsSemiAuto = contents[colume].Trim() == "y";
            colume++;
            stats.FireRate =  float.Parse(contents[colume]);
            colume++;
            stats.Accuracy =  float.Parse(contents[colume]);
            colume++;
            stats.Handling =  float.Parse(contents[colume]);
            colume++;
            stats.Recoil =  float.Parse(contents[colume]);
            colume++;
            stats.BulletType = (BulletType)System.Enum.Parse( typeof(BulletType), contents[colume].Trim() );
            colume++;
            gunScriptable.ShootSound = m_AllShootSound.Single(x=>x.clipName == contents[colume].Trim().Replace(" ", "")).audioClip;
            colume++;
            gunScriptable.ExplodeRadius = float.Parse(contents[colume]);
            gunScriptable.ReloadScriptable = AssetDatabase.LoadAssetAtPath<GunReloadScriptable>(m_ScriptablePath+"/Reload/"+displayName.Replace(" ", "")+"_Reload.asset");

            gunScriptable.GunStats = stats;
            allGuns.Add(gunScriptable.Id,gunScriptable);
            var weaponOwnership = new MainGameNameSpace.WeaponOwnership();
            weaponOwnership.Gun = gunScriptable;
            weaponOwnership.IsOwned = index/collumeCount<=4;
            allWeaponAndOwnership.Add(weaponOwnership);
            EditorUtility.SetDirty(gunScriptable);
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
