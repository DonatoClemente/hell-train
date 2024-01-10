using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                                    //      QUICK NOTE: This script should get attached to whatever gameobject the Player will be
namespace HellTrain.PlayerSystems
{

        /**************************************************************************
            </> Summary </>

            There is a serializable struct made, PlayerData, which holds info about player we want saved. 
            (Example: The amount of health you had upon saving)

            If you want something to save, create a method that passes along Player's info to this script through that scripts.
            Using a method in that script 
            Create methods inside that class Save() and Load() that when called grabs the info you want saved or loaded
            and puts them into the struct you created over there.
        
        */
[System.Serializable]
public struct PlayerData
{
    public Vector3 Position;   
    public int Health;
}
    
        /**************************************************************************
            </> Summary </>

            Above was the struct which will hold value types we want to save and use.
            Below is the actual PlayerSavaData script, which will use the stuct we created above.

            This is purely for Player specific data.

            Save gets passed info, and places them into the stuct type we just made, PlayerData.
            Load gets passed a PlayerData struct from the SaveSystem script. 
            Once you have that struct, take out the data and place it where it belongs.
            
            As an example, I saved the players position for you to see and understand what that looks like.
        */
    public class PlayerSaveData : MonoBehaviour
    {
        // Lines commented out don't exist yet, but are concepts that can be implimented.
        public void Save(ref PlayerData data)
        {
                data.Position = GetComponent<Transform>().position;
                //data.Inventory = new List<InventorySaveData>();
                //_inventory.Save(ref data.Inventory);

                //data.Health = _healthBar.currentHP();
        }

        public void Load(PlayerData data)
        {
                //_inventory.Load(data.Inventory);
                GetComponent<Transform>().position = data.Position;

                //_healthBar.SetHealth(data.Health);
        }

        public void GetSomeDataFromPlayer()
        {
               // Since this script will hold the actual instances of the data actually information Player will use and not some random instance
                // Other scripts may want that data. You can pass them that data through this method after you change its return type, and name.
        }
    }

}