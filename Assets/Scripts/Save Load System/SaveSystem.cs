using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;

using HellTrain.PlayerSystems;

namespace HellTrain
{
    public class SaveSystem : MonoBehaviour
    {
        /**************************************************************************
            </> Summary </>

            There is a serializable struct made, SaveData, which holds other structs of data. 
            (Example: A struct of PlayerSaveData holds Player's position when they last saved)

            If you want something to save, create a stuct within that class that holds what you want saved.
            Create methods inside that class Save() and Load() that when called grabs the info you want saved or loaded
            and puts them into the struct you created over there.
        
        */
        [System.Serializable]
        public struct SaveData
        {
            public PlayerData playerData;
        }     

        // This is the local variable made to hold the data we want to be saved.
        // It is a variable of type struct that we just made above.
        private static SaveData s_CurrentData = new SaveData();

        [System.Serializable]
        public struct SceneData
        {
            public string SceneName;
        }



        public static void Save()
        {
            string savefile = Application.persistentDataPath + "/save.sav";

            try
            {       // Check if the file we want to save even exists.
                    // If yes, delete that save as we will be writing over it.
                    if (File.Exists(savefile))
                    {
                                Debug.Log("Data exists. Deleting old file and writing a new one!");
                                File.Delete(savefile);
                    }
                    else
                        {Debug.Log("Writing file for the first time!");}

                    // Using creating a new savefile with Filestream. Make sure to immediately close it afterwards.            
                    using FileStream stream = File.Create(savefile);
                    stream.Close();
                        {Debug.Log("Closed stream!");}

                // This "Serializes" or turns our struct of s_CurrentData into a json readable file.
                File.WriteAllText(savefile, JsonConvert.SerializeObject(s_CurrentData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");                         
            }
        }

        public static void Load()
        {
            string savefile = Application.persistentDataPath + "/save.sav";


            if (!File.Exists(savefile))
            {
                Debug.LogError($"Cannot load file at {savefile}. File does not exist!");
                throw new FileNotFoundException($"{savefile} does not exist!");
            }   
                try
                {     
                    s_CurrentData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(savefile));
                }  
                catch (Exception e)
                {
                        Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                        throw e;
                }
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        // After Load() finishes turning the json back into a SaveData struct which this game's script can read.
        // Call the individual script's Load() method and put in the parameters whatever data it original threw here to save.
        // ( Example: Our struct took in PlayerData, so call give our current SaveData struct's playerData variable back for that script to do something with that loaded data)
        static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //GameManager.Instance.PlayerData.Load(s_CurrentData.playerData);

            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}

