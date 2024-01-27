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

[Serializable]
public class UniqueText
{
    public string Key;
    public string Replacement;
}

public class ReadCsv : MonoBehaviour
{
    [SerializeField] private string m_ResourcesPath = "";
    [SerializeField] private string m_ScriptablePath = "";
    [SerializeField] private MainGameManager m_MainGameManager;
    private EnemyScriptable m_GhostScriptable = null;
    private EnemyScriptable m_PuppetScriptable = null;
    private EnemyScriptable m_ServantScriptable = null;
    private EnemyScriptable m_DyingXintScriptable = null;
    public List<UniqueText> m_UniqueText = new List<UniqueText>();



    [SerializeField] private List<ShootSoundAndName> m_AllShootSound = new List<ShootSoundAndName>();
#if UNITY_EDITOR
    [EButton("SaveCsv")]
    private void SaveCsvFile(){
        StartCoroutine(GetCsvFromGoogle());
    }
    
    [EButton("ReadCsvDialog")]
    private void ReadCsvFileDialog(){
        StartCoroutine(ReadDialogCSV());

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [EButton("ReadCsvGun")]
    private void ReadCsvFileGun(){
        // TODO : get upgrade data auto
        StartCoroutine(ReadGunCSV());

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [EButton("ReadWeaponUpgrade")]
    private void ReadCsvFileWeaponUpgrade(){
        StartCoroutine(ReacCSVWeaponUpgrade());

        EditorUtility.SetDirty(m_MainGameManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [EButton("ReadCsvLocation")]
    private void ReadCsvFileLocation(){
        StartCoroutine(ReadCSVLocation());

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

        // xin auto read csv
        var xin = Resources.Load<GameObject>("Enemy/Prefab/Xin");
        var xinController = xin.GetComponent<XinController>();
        EditorUtility.SetDirty(xinController);
        xinController.SetServantScriptable(m_ServantScriptable);

        // dying xin auto read csv
        var dyingXin = Resources.Load<GameObject>("Enemy/Prefab/DyingXin");
        var dyingXinController = dyingXin.GetComponent<DyingXinController>();
        EditorUtility.SetDirty(dyingXinController);
        dyingXinController.SetDyingXinScriptable(m_DyingXintScriptable);
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
            Enemy.DisplayImage = Resources.Load<Sprite>("Enemy/DisplayImage/"+displayName.Replace(" ", ""));
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

            //record Servant scriptable
            if(Enemy.Id == 7){
                m_ServantScriptable = AssetDatabase.LoadAssetAtPath(m_ScriptablePath+"/Enemy/"+displayName.Replace(" ", "")+".asset", typeof(EnemyScriptable)) as EnemyScriptable;
            }

            //record Dying Xin scriptable
            if(Enemy.Id == 8){
               m_DyingXintScriptable = AssetDatabase.LoadAssetAtPath(m_ScriptablePath+"/Enemy/"+displayName.Replace(" ", "")+".asset", typeof(EnemyScriptable)) as EnemyScriptable;
            }

            allEnemy.Add(Enemy);
            EditorUtility.SetDirty(Enemy);
            m_MainGameManager.SetAllEnemy(allEnemy);
        }   

    }

    private IEnumerator ReacCSVWeaponUpgrade() {
        // WeaponUpgrade
        List<WeaponUpgradeScriptable> allWeaponUpgrade = new List<WeaponUpgradeScriptable>(); 
        if(Resources.Load<TextAsset>("CSV/WeaponUpgrade") == null){
            Debug.LogError(m_ResourcesPath+"/WeaponUpgrade.csv is null" );
            yield break;
        }
        
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/WeaponUpgrade");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/WeaponUpgrade").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 6;
        // last row is for easier to read on excel
        var allWeapon = m_MainGameManager.GetAllWeapon();
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            int colume = index;
            string displayName = allWeapon.Find(x=>x.Id==int.Parse(contents[colume+1]) ).DisplayName;
            WeaponUpgradeScriptable WeaponUpgrade = null;
            List<WeaponUpgradeDetail> upgradeDetails = new List<WeaponUpgradeDetail>();

            bool isFileAlreadyExist = false;
            if (System.IO.File.Exists(m_ScriptablePath+"/WeaponUpgrade/"+displayName+"_Upgrade.asset"))
            {
                // file already exist 
                isFileAlreadyExist =true;
                WeaponUpgrade = AssetDatabase.LoadAssetAtPath<WeaponUpgradeScriptable>(m_ScriptablePath+"/WeaponUpgrade/"+displayName+"_Upgrade.asset");
                upgradeDetails = WeaponUpgrade.UpgradeDetails;
            }else{
            
                WeaponUpgrade = ScriptableObject.CreateInstance<WeaponUpgradeScriptable>();

                AssetDatabase.CreateAsset(WeaponUpgrade, m_ScriptablePath+"/WeaponUpgrade/"+displayName+"_Upgrade.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

                WeaponUpgrade.Id = int.Parse(contents[colume]);
                colume++;
                WeaponUpgrade.WeaponId = int.Parse(contents[colume]);
                colume++;
                string statName = contents[colume].Trim();
                colume++;

                List<string> allUpgradeValue = contents[colume].Split('|').ToList();
                List<string> allUpgradeCost = contents[colume+1].Split('|').ToList();
                var weaponUpgradeDetail = new WeaponUpgradeDetail{
                    UpgradeStat = (GunScriptableStatEnum)System.Enum.Parse( typeof(GunScriptableStatEnum), statName )
                    
                };
                for (int j = 0; j < allUpgradeValue.Count; j++)
                {

                    var upgradeCostAndValue = new WeaponUpgradeCostAndValue{
                        UpgradeValue = allUpgradeValue[j].Trim(),
                        Cost = float.Parse(allUpgradeCost[j].Trim())
                    };
                    weaponUpgradeDetail.CostAndValue.Add(upgradeCostAndValue);
                }
                upgradeDetails.Add(weaponUpgradeDetail);
                WeaponUpgrade.UpgradeDetails = upgradeDetails;
                colume++;
                // no need to add it again
                if(!isFileAlreadyExist)
                    allWeaponUpgrade.Add(WeaponUpgrade);
                
                var gunScriptable = AssetDatabase.LoadAssetAtPath<GunScriptable>(m_ScriptablePath+"/Gun/"+displayName.Replace(" ", "")+".asset");
                gunScriptable.UpgradeScriptable = AssetDatabase.LoadAssetAtPath<WeaponUpgradeScriptable>(m_ScriptablePath+"/WeaponUpgrade/"+displayName+"_Upgrade.asset");
                EditorUtility.SetDirty(gunScriptable);
                EditorUtility.SetDirty(WeaponUpgrade);
        }    

        m_MainGameManager.SetAllWeaponUpgrade(allWeaponUpgrade);
    }

    private IEnumerator ReadCSVLocation(){
        // Location
        if(Resources.Load<TextAsset>("CSV/Location") == null){
            Debug.LogError(m_ResourcesPath+"/Location.csv is null" );
            yield break;
        }
        List<MapLocationScriptable> allLocation = new List<MapLocationScriptable>();
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Location");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string json = Resources.Load<TextAsset>("CSV/Location").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 14;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            int colume = index;
            // create scriptable 
            MapLocationScriptable location = ScriptableObject.CreateInstance<MapLocationScriptable>();
            AssetDatabase.CreateAsset(location, m_ScriptablePath+"/Location/"+contents[index+1].Trim().Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            location.Id = int.Parse(contents[colume]);
            colume++;
            location.DisplayName = contents[colume].Trim();
            location.Prefab = Resources.Load<GameObject>("Location/"+contents[colume].Trim().Replace(" ", ""));
            
            
            colume++;
            float posX = float.Parse(contents[colume].Split('|')[0]);
            float posZ = float.Parse(contents[colume].Split('|')[1]);
            location.Pos = new Vector3(posX,0,posZ);
            
            colume++;
            location.NormalWavesCount = int.Parse(contents[colume]);
            colume++;
            location.NormalWavesStrength = float.Parse(contents[colume]);
            colume++;
            List<int> normalEnemyList = new List<int>();
            foreach (var item in contents[colume].Split('|'))
            {
                normalEnemyList.Add(int.Parse(item));
            }
            colume++;
            location.NormalWaveEnemy = normalEnemyList;

            location.FinalWaveStrength = float.Parse(contents[colume]);
            colume++;
            
            List<int> finalEnemyList = new List<int>();
            foreach (var item in contents[colume].Split('|'))
            {
                finalEnemyList.Add(int.Parse(item));
            }
            location.FinalWaveEnemy = finalEnemyList;
            colume++;
            List<int> lockBy = new List<int>();
            foreach (var item in contents[colume].Split('|'))
            {
                lockBy.Add(int.Parse(item));
            }
            location.LockBy = lockBy;
            colume++;
            location.Level = int.Parse(contents[colume]);
            colume++;
            location.StartDialogId =  int.Parse(contents[colume]);
            colume++;
            location.EndDialogId =  int.Parse(contents[colume]);
            colume++;
            
            if(contents[colume].Trim() != string.Empty)
            foreach (var item in contents[colume].Split('|'))
            {
                if(item.Trim() == string.Empty)
                    continue;

                float mutationTypeId = float.Parse(item.Split('@')[0]);
                float mutationValue = float.Parse(item.Split('@')[1]);
                switch (mutationTypeId)
                {
                    case 0:
                        location.HealthMutation = mutationValue;
                    break;
                    case 1:
                        location.DamageMutation = mutationValue;
                    break;
                    case 2:
                        location.SpeedMutation = mutationValue;
                    break;
                    default:
                    break;
                }
            }
            colume++;
            // extra reward
            location.ExtraReward = float.Parse(contents[colume]);

            allLocation.Add(location);

            EditorUtility.SetDirty(location);
            yield return null;
        }
            m_MainGameManager.SetAllLocation(allLocation);
    }

    private IEnumerator ReadGunCSV(){
        // Gun
        Dictionary<int,GunScriptable> allGuns = new Dictionary<int, GunScriptable>(); 
        List<GunScriptable> allGunScriptable = new List<GunScriptable>();

        if(Resources.Load<TextAsset>("CSV/Gun") == null){
            Debug.LogError(m_ResourcesPath+"/CSV//Gun.csv is null" );
            yield break;
        }
        
        
        // remove all scriptable in gun folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Gun");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/Gun").ToString();

        var contents = json.Split('\n',',');
        // last line for unlock by level complete
        int collumeCount = 15;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            int colume = index;
            // create scriptable 
            GunScriptable gunScriptable = ScriptableObject.CreateInstance<GunScriptable>();

            colume++;
            gunScriptable.UnlockCost = float.Parse(contents[colume]);
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
            stats.IsSemiAuto = contents[colume].Trim() == "Yes";
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

            // Auto load upgrade scriptable
            WeaponUpgradeScriptable weaponUpgradeScriptable = AssetDatabase.LoadAssetAtPath<WeaponUpgradeScriptable>(m_ScriptablePath+"/WeaponUpgrade/"+displayName+"_Upgrade.asset");
            if(weaponUpgradeScriptable != null){
                gunScriptable.UpgradeScriptable = weaponUpgradeScriptable;
            }else{
                Debug.Log("Missing upgrade scriptable :"+displayName);
            }

            gunScriptable.GunStats = stats;
            allGuns.Add(gunScriptable.Id,gunScriptable);

            allGunScriptable.Add(gunScriptable);
            EditorUtility.SetDirty(gunScriptable);
            yield return null;
        }
        m_MainGameManager.SetAllWeapon(allGunScriptable);
    }


    

    public string ReplaceText(string source){
        string ans = source;
        foreach (var item in m_UniqueText)
        {
            if(!ans.Contains("_@@")){
                return ans;
            }
            ans = ans.Replace(item.Key,item.Replacement);
        }
        return ans;

    }

    private IEnumerator ReadDialogCSV(){
        // Dialog
        Dictionary<int,DialogScriptable> allDialogs = new Dictionary<int, DialogScriptable>(); 
        List<DialogScriptable> allDialogScriptable = new List<DialogScriptable>();

        if(Resources.Load<TextAsset>("CSV/Dialog") == null){
            Debug.LogError(m_ResourcesPath+"/CSV//Dialog.csv is null" );
            yield break;
        }
        
        
        // remove all scriptable in Dialog folder
        FileUtil.DeleteFileOrDirectory(m_ScriptablePath+"/Dialog");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string json = Resources.Load<TextAsset>("CSV/Dialog").ToString();

        var contents = json.Split('\n',',');
        int collumeCount = 6;
        for (int i = collumeCount; i < contents.Length; i+=collumeCount)
        {
            int index = i;
            int colume = index;
            // create scriptable 
            DialogScriptable DialogScriptable = ScriptableObject.CreateInstance<DialogScriptable>();

            DialogScriptable.Id = int.Parse(contents[colume]);
            colume++;
            string displayName = DialogScriptable.Id.ToString()+contents[colume].Trim();
            DialogScriptable.SpeakerName = contents[colume].Trim();
            colume++;
            AssetDatabase.CreateAsset(DialogScriptable, m_ScriptablePath+"/Dialog/"+displayName.Replace(" ", "")+".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // tra chinese
            colume++;
            // sim chinese
            colume++;
            DialogScriptable.EngDialog = contents[colume].Trim();
            DialogScriptable.EngDialog = ReplaceText(DialogScriptable.EngDialog);
            colume++;
            foreach (var item in contents[colume].Split('|'))
            {
                int tragetInt = -2;
                if(int.TryParse(item, out tragetInt))
                    DialogScriptable.NextId.Add(tragetInt);
            }
            allDialogs.Add(DialogScriptable.Id,DialogScriptable);

            allDialogScriptable.Add(DialogScriptable);
            EditorUtility.SetDirty(DialogScriptable);
        }
        m_MainGameManager.SetAllDialog(allDialogScriptable);
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

        // WeaponUpgrade
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=2049343216&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/WeaponUpgrade.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/WeaponUpgrade.csv",json);
 
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

        
        // Dialog
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vQy-u5Mkn62XtESQPB1QMFcG6udxGm9uIIegghRND3_fufm6GlGznw_4NOqTTIeVGzdIWtex3QWZnh7/pub?gid=287695030&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/CSV/Dialog.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/CSV/Dialog.csv",json);
 
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

}
