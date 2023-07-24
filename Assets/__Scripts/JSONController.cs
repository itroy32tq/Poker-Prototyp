using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProspectorPrototyp
{
    public class JSONController : MonoBehaviour
    {

        [ContextMenu("Save")]
        public void SavePlayer(PlayerCashData pl)
        {
            File.WriteAllText(Application.streamingAssetsPath + "/player_"+pl.num.ToString()+".json", JsonUtility.ToJson(pl));
        }

        [ContextMenu("Load")]
        public Player LoadPlayer(Player pl)
        {
            try
            {
                PlayerCashData d = JsonUtility.FromJson<PlayerCashData>(File.ReadAllText(Application.streamingAssetsPath + "/player_" + pl.playerNum.ToString() + ".json"));
                pl.cache = d.cash;
            }
            catch
            { 
            
            }
            return pl;
        }
    }
}
