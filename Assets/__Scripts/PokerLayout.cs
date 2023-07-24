using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ProspectorPrototyp
{
    [System.Serializable]
    public class SlotDef
    {
        public float x;
        public float y;
        public bool faceUp = false;
        public string layerName = "Default";
        public int layerID = 0;
        public int id;
        public List<int> hiddenBy = new List<int>();
        public float rot;
        public string type = "slot";
        public Vector2 stagger;
        public int player;
        public Vector3 pos;
    }
    public class PokerLayout : MonoBehaviour
    {
        [Header("Set Dynamically")]
        public PT_XMLReader xmlr;
        public PT_XMLHashtable xml;
        public Vector3 multiplier;
        public List<SlotDef> slotDefs;
        public List<SlotDef> poolDefs;
        public SlotDef drawPile;
        public SlotDef discardPile;
        public SlotDef target;

        private void ParseTarget(PT_XMLHashtable pool)
        {
            PT_XMLHashList poolX = pool["childSlot"];

            for (int i = 0; i < poolX.Count; i++)
            {
                SlotDef tSD;
                tSD = new SlotDef();
                if (poolX[i].HasAtt("type")) tSD.type = poolX[i].att("type");
                else tSD.type = "slot";

                tSD.x = float.Parse(poolX[i].att("x").Replace(",", "."), CultureInfo.InvariantCulture);
                tSD.y = float.Parse(poolX[i].att("y").Replace(",", "."), CultureInfo.InvariantCulture);
                tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0f);

                tSD.layerID = int.Parse(poolX[i].att("layer"));

                tSD.layerName = tSD.layerID.ToString();
                poolDefs.Add(tSD);
            }

        }
        public void ReadLayout(string xmlText)
        {
            xmlr = new PT_XMLReader();

            xmlr.Parse(xmlText);

            xml = xmlr.xml["xml"][0];

            multiplier.x = float.Parse(xml["multiplier"][0].att("x").Replace(",", "."), CultureInfo.InvariantCulture);
            multiplier.y = float.Parse(xml["multiplier"][0].att("y").Replace(",", "."), CultureInfo.InvariantCulture);

            SlotDef tSD;

            PT_XMLHashList slotsX = xml["slot"];

            for (int i = 0; i < slotsX.Count; i++)
            {
                tSD = new SlotDef();
                if (slotsX[i].HasAtt("type")) tSD.type = slotsX[i].att("type");
                else tSD.type = "slot";

                tSD.x = float.Parse(slotsX[i].att("x").Replace(",", "."), CultureInfo.InvariantCulture);
                tSD.y = float.Parse(slotsX[i].att("y").Replace(",", "."), CultureInfo.InvariantCulture);
                tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0f);

                tSD.layerID = int.Parse(slotsX[i].att("layer"));

                tSD.layerName = tSD.layerID.ToString();

                switch (tSD.type)
                {
                    case "slot":
                        break;
                    case "drawpile":
                        tSD.stagger.x = float.Parse(slotsX[i].att("xstagger").Replace(",", "."), CultureInfo.InvariantCulture);
                        drawPile = tSD;
                        break;
                    case "discardpile":
                        discardPile = tSD;
                        break;
                    case "target":
                        target = tSD;
                        ParseTarget(slotsX[i]);
                        break;
                    case "hand":
                        tSD.player = int.Parse(slotsX[i].att("player"));
                        tSD.rot = float.Parse(slotsX[i].att("rot").Replace(",", "."), CultureInfo.InvariantCulture);
                        slotDefs.Add(tSD);
                        break;
                }

            }
        }

    }
}
