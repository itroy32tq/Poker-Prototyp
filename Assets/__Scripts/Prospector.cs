using UnityEngine;


namespace ProspectorPrototyp
{
    public class Prospector : MonoBehaviour
    {
        public static Prospector S;

        [Header("Set in Inspector")]
        public TextAsset deckXML;

        [Header("Set Dynamically")]
        public Deck deck;

        private void Awake()
        {
            S = this;
        }

        private void Start()
        {
            deck = GetComponent<Deck>();
            deck.InitDeck(deckXML.text);
            Deck.Shuffle(ref deck.cards);

            Card c;
            for (int cNum = 0; cNum < deck.cards.Count; cNum++)
            { 
                c = deck.cards[cNum];
                c.transform.localPosition= new Vector3(cNum%13*3, cNum/13*4, 0f);
            }
        }
    }
}
