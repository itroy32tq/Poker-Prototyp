using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProspectorPrototyp
{
    public class CardPoker : Card, IPointerClickHandler
    {
        static public float MOVE_DURATION = 0.5F;
        static public string MOVE_EASING = Easing.InOut;
        static public float CARD_HEIGHT = 3.5F;
        static public float CARD_WIDTH = 2F;

        [Header("Set Dynamically")]
        public PokerCardState state = PokerCardState.drawpile;
        public List<Vector3> bezierPts;
        public List<Quaternion> bezierRots;
        public float timeStart, timeDuration;
        public int eventualSortOrder;
        public string eventualSortLayer;
        public bool is—hosen = false;

        public GameObject reportFinishTo = null;

        [System.NonSerialized]
        public Player callbackPlayer = null;
        public void MoveTo(Vector3 ePos, Quaternion eRot)
        { 
            bezierPts= new List<Vector3>();
            bezierPts.Add(transform.localPosition);
            bezierPts.Add(ePos);

            bezierRots= new List<Quaternion>();
            bezierRots.Add(transform.rotation);
            bezierRots.Add(eRot);

            if (timeStart == 0) timeStart = Time.time;
            timeDuration = MOVE_DURATION;
            state = PokerCardState.to;
        }

        public void MoveTo(Vector3 ePos)
        {
            MoveTo(ePos, Quaternion.identity);
        }


        public void Update()
        {
            switch (state) 
            {
                case PokerCardState.toHand:
                case PokerCardState.toTarget:
                case PokerCardState.toDrawpile:
                case PokerCardState.toDiscard:
                case PokerCardState.to:
                    float u = (Time.time - timeStart) / timeDuration;
                    float uC = Easing.Ease(u, MOVE_EASING);
                    if (u < 0)
                    {
                        transform.localPosition = bezierPts[0];
                        transform.rotation = bezierRots[0];
                        return;
                    }
                    else if (u >= 1)
                    {
                        uC = 1;
                        if (state == PokerCardState.toHand) state = PokerCardState.hand;
                        if (state == PokerCardState.toTarget) state = PokerCardState.target;
                        if (state == PokerCardState.toDrawpile) state = PokerCardState.drawpile;
                        if (state == PokerCardState.to) state = PokerCardState.idle;
                        if (state == PokerCardState.toDiscard) state = PokerCardState.discard;


                        transform.localPosition = bezierPts[bezierPts.Count - 1];
                        transform.rotation = bezierRots[bezierRots.Count - 1];
                        timeStart = 0;

                        if (reportFinishTo != null)
                        {
                            reportFinishTo.SendMessage("DrowCardEx", this);
                            reportFinishTo = null;
                        }
                        else if (callbackPlayer != null)
                        {
                            callbackPlayer.CPCallback(this);
                            callbackPlayer = null;
                        }
                        else { }
                    }
                    else
                    {
                        Vector3 pos = Utils.Bezier(uC, bezierPts);
                        transform.localPosition = pos;
                        Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                        transform.rotation = rotQ;
                    }
                    if (u > 0.5f)
                    { 
                        SpriteRenderer sRend = spriteRenderers[0];
                        if (sRend.sortingOrder != eventualSortOrder) SetSortOrder(eventualSortOrder);
                        if (sRend.sortingLayerName != eventualSortLayer) SetSortingLayerName(eventualSortLayer);
                    }

                    break;
                case PokerCardState.discard:
                    if (FaceUp) FaceUp = false;
                    break;
            }
        }
    }
}
