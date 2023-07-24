using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProspectorPrototyp
{
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        [Header("Set Dynamically")]
        public string suit;
        public int rank;
        public Color color= Color.black;
        public string colS = "Black";
        public List<GameObject> decoGOs= new();
        public List<GameObject> pipGOs = new();

        public GameObject back;
        public CardDefinition def;
        public SpriteRenderer[] spriteRenderers;

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        public delegate void ClickEventHandler(PointerEventData eventData);

        public bool FaceUp 
        {
            get { return !back.activeSelf; }
            set { back.SetActive(!value);  }
        }

        private void Start()
        {
            SetSortOrder(0);
        }

        public void PopulateSpriteRenderers()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0) spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public void SetSortingLayerName(string tSLN)
        { 
            PopulateSpriteRenderers();

            foreach (SpriteRenderer tSR in spriteRenderers) tSR.sortingLayerName = tSLN;
        }

        public void SetSortOrder(int sOrd) 
        { 
            PopulateSpriteRenderers();

            foreach (SpriteRenderer tSR in spriteRenderers)
            {
                if (tSR.gameObject == this.gameObject)
                { 
                    tSR.sortingOrder = sOrd;
                    continue;
                }

                switch (tSR.gameObject.name)
                {
                    case "back":
                        tSR.sortingOrder = sOrd + 2;
                        break;
                    case "face":
                    default:
                        tSR.sortingOrder= sOrd + 1;
                        break;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEventHandler?.Invoke(eventData);
        }
    }

    [System.Serializable]
    public class Decorator 
    {
        public string type;

        public Vector3 loc;

        public bool flip = false;
        public float scale = 1f;
    }

    [System.Serializable]
    public class CardDefinition
    {
        public string face;

        public int rank;

        public bool flip = false;
        public List<Decorator> pips = new();
    }
}
