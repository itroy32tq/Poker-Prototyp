using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ProspectorPrototyp

{
    public class PokerManager : MonoBehaviour
    {
        static public PokerManager Manager;
        static public Player CURRENT_PLAYER = null;
        static public Player VICTORY_PLAYER = null;
        static public List<Player> VICTORY_PLAYERS = null;

        [Header("Set in Inspector")]
        public TextAsset deckXML;
        public TextAsset LayuotXML;
        public Vector3 LayoutCenter = Vector3.zero;
        public float handFanDegrees = 5f;
        public int numStartCards = 5;
        public int numPlayers = 4;
        public float drawTimeStagger = 0.1f;
        public ActionPlayerManager actionPlayerManager;

        [SerializeField]
        private BubbleConvasComponent _uiComponent;

        public float _turnTime = 1f;

        [Header("Set Dynamically")]
        public Deck deck;
        public List<CardPoker> drawPile;
        public List<CardPoker> deckAssistentClonedrawPile;
        public List<CardPoker> discardPile;
        public List<Player> players;
        public CardPoker targetCard;
        public TurnPhase TurnPhase = TurnPhase.idle;
        public GamePhase GamePhase = GamePhase.idle;

        public bool PassTurnClick = false;
        public bool CardsExConfirmClick = false;

        private PokerLayout layout;
        private Transform layoutAnchor;

        private DeckAssistent deckAssistent;
        private MaineMeneController _menuController;

        [SerializeField]
        private JSONController jSONController;

        private void Awake()
        {
            Manager = this;
        }
        public List<CardPoker> UpgradeCardsList(List<Card> LCD)
        { 
            List<CardPoker> LCB = new();

            foreach (Card card in LCD) 
            {
                LCB.Add(card as CardPoker);
            }
            return LCB;
        }

        private void Start()
        {
            deck = GetComponent<Deck>();
            deck.InitDeck(deckXML.text);
            Deck.Shuffle(ref deck.cards);

            layout = GetComponent<PokerLayout>();
            layout.ReadLayout(LayuotXML.text);

            drawPile = UpgradeCardsList(deck.cards);
            actionPlayerManager.DrowPile = new List<CardPoker>(drawPile);
            actionPlayerManager.Initial();

            //формируем и шафлим колоду, конфигурируем игроков
            LayoutGame();
            
        }

        private void PointerClickHandler(PointerEventData eventData)
        {
            
            if (CURRENT_PLAYER == null) return;
            
            if (GamePhase != GamePhase.card_exchange || CURRENT_PLAYER.type == PlayerType.ai) return;

            CardPoker card = eventData.pointerEnter.GetComponentInParent<CardPoker>();

            Vector3 delta_pos = Vector3.up * CardPoker.CARD_HEIGHT / 4f;
            Quaternion rotQ = card.transform.rotation;

            if (card != null && !card.isСhosen)
            {
               
                card.transform.position += rotQ * delta_pos;
                card.isСhosen = true;
              
            }
            else 
            {
                card.transform.position -= rotQ * delta_pos;
                card.isСhosen = false;
            }
        }

        public void ArrangeDrawPile()
        {
            CardPoker tCP;

            for (int i = 0; i < drawPile.Count; i++)
            { 
                tCP = drawPile[i];
                tCP.transform.SetParent(layoutAnchor);
                tCP.transform.localPosition = layout.drawPile.pos;
                tCP.FaceUp = false;
                tCP.SetSortingLayerName(layout.drawPile.layerName);
                tCP.SetSortOrder(-i * 4);
                tCP.state = PokerCardState.drawpile;
                tCP.OnClickEventHandler += PointerClickHandler;
            }
        }
        private void LayoutGame() 
        {
            if (layoutAnchor == null)
            {
                GameObject tGO = new GameObject("_LayoutAnchor");
                layoutAnchor = tGO.transform;
                layoutAnchor.transform.position = LayoutCenter;
            }

            //сбор колоды
            ArrangeDrawPile();

            //конфигурируем икроков
            Player pl;
            players = new List<Player>();

            foreach (SlotDef tSD in layout.slotDefs)
            {
                pl = new Player
                {
                    handSlotDef = tSD
                };
                players.Add(pl);
                pl.playerNum = tSD.player;
                pl = jSONController.LoadPlayer(pl);
            }

            _uiComponent.CreateBubble();
            players[0].type = PlayerType.human;
            
            StartGame();
        }

        private void CardsDistribution()
        {
            TurnPhase = TurnPhase.waiting;
            CardPoker tCP;
            for (int i = 0; i < numStartCards; i++)
            {
                for (int j = 0; j < numPlayers; j++)
                {
                    tCP = Draw();
                    tCP.timeStart = Time.time + drawTimeStagger * (i * numPlayers + j);
                    players[(j+1)% numPlayers].AddCard(tCP);
                }
            }
            TurnPhase = TurnPhase.idle;
            Invoke("SwitchGamePhase", drawTimeStagger * (numStartCards * numPlayers + numPlayers));    
        }

        public void StartGame()
        {
            //первая фаза игры начальная ставка
            GamePhase = GamePhase.first_rate;
            //запуск передачи хода
            PassTurn(1);
        }

        public void PassTurn(int num=-1) 
        {
            if (num == -1)
            {
                int ndx = players.IndexOf(CURRENT_PLAYER);
                num = (ndx+1)%numPlayers;
            }

            int lastPlayerNum = -1;

            if (CURRENT_PLAYER != null) lastPlayerNum = CURRENT_PLAYER.playerNum;

            CURRENT_PLAYER = players[num];

            Utils.tr("PokerManager:PassTurn()", "Old:" + lastPlayerNum,
                "New: " + CURRENT_PLAYER.playerNum);

            //проверка на победу
            if (ChekEarlyVictory())
            {
                GamePhase = GamePhase.gameover;
                Invoke("RestartGame", 3);
                return;
            }

            CURRENT_PLAYER.TakeTurn(GamePhase);
        }

        private bool ChekEarlyVictory()
        {
            var map = players.Where(p => p.action == PlayerAction.pass);
            if (map.Count() == 3)
            {
                Player pl = new List<Player>(players.Where(p => p.action != PlayerAction.pass))[0];
                pl.cache += BankComponent.Bank.BankCash;
                VICTORY_PLAYERS = new()
                {
                    pl
                };
                return true;
            } 
            return false;
        }
    
        public void MoveToDiscard(List<CardPoker> tCPList)
        {
            TurnPhase = TurnPhase.waiting;

            for (int i = 0; i < tCPList.Count; i++)
            {
                CardPoker tCP = tCPList[i];
                CURRENT_PLAYER.RemoveCard(tCP);
                tCP.state = PokerCardState.discard;
                discardPile.Add(tCP);
                tCP.SetSortingLayerName(layout.discardPile.layerName);
                tCP.SetSortOrder(discardPile.Count * 4);
                Vector3 pos = layout.discardPile.pos + Vector3.back / 2;

                tCP.timeStart = Time.time + drawTimeStagger * (i * tCPList.Count);
                tCP.state = PokerCardState.toDiscard;
                tCP.MoveTo(pos);
                
                if (i == tCPList.Count - 1) tCP.reportFinishTo = this.gameObject;
            }
        }

        public void DrowCardEx()
        {
            TurnPhase = TurnPhase.waiting;
            int exCount = CURRENT_PLAYER.discardCardsInHand.Count;

            for (int i = 0; i < exCount; i++)
            {
                CardPoker tCP;
                tCP = Draw();
                tCP.timeStart = Time.time + drawTimeStagger * (i * CURRENT_PLAYER.discardCardsInHand.Count);
                CURRENT_PLAYER.AddCard(tCP);
                if (i == CURRENT_PLAYER.discardCardsInHand.Count - 1) TurnPhase = TurnPhase.idle;
            }
        }

        public CardPoker Draw()
        {
            CardPoker cd = drawPile[0];
            drawPile.RemoveAt(0);
            return cd;
        }

        private void GetVictoryPlayer()
        {

            //убираем тех кто сказал пасж
            var map = players.Where(p => p.action != PlayerAction.pass);
            //сортируем по комбинации
            map = map.OrderByDescending(d => d.combination);
            //старшая комбинация
            var tComb = map.First().combination;
            //коллекция всех таких комбинаций
            map = map.Where(d => d.combination == tComb);
            //самый простой вариант когда она одна
            if (map.Count() == 1) VICTORY_PLAYERS = new()
            {
                map.First()
            };
            else 
            {
                
                var l = new List<Player>(map);
                PlayerData et = new(l[0]);
                List<PlayerData> dataList = new()
                {
                    et
                };

                for (int i = 1; i < l.Count(); i++)
                {
                    PlayerData playerDataB = new(l[i]);
                    dataList.Add(playerDataB);
                    int res = AltCombDetect.CompareCombination(et, playerDataB);
                    if (res < 0) et = playerDataB;
                    else if (res == 0)
                    {
                        et.DrowOpponent = playerDataB;
                        playerDataB.DrowOpponent = et;
                    };
                }
                VICTORY_PLAYERS = new()
                {
                    et.Player
                };

                var fin = from p in dataList where p.DrowOpponent == et select p.Player;
                if (fin != null) VICTORY_PLAYERS.AddRange(new List<Player>(fin));
             
            }

            foreach (Player pl in VICTORY_PLAYERS)
            {
                pl.cache += (int)BankComponent.Bank.BankCash / VICTORY_PLAYERS.Count();
            }
            SavePlayer();
        }
        private void SavePlayer()
        {
            foreach (Player pl in players)
            {
                PlayerCashData data = new PlayerCashData(pl.playerNum, pl.cache);
                jSONController.SavePlayer(data);
            }
        }
        private void RestartGame()
        {
            CURRENT_PLAYER = null;
            _menuController = _uiComponent.CreateButtonsPanel();
            _menuController.OpenDialog();
        }
        public void SwitchGamePhase()
        {
            switch (GamePhase)
            {
                case GamePhase.first_rate:
                    GamePhase = GamePhase.dealing;
                    CardsDistribution();
                    break;
                case GamePhase.dealing:
                    GamePhase = GamePhase.first_auction;
                    PassTurn(1);
                    break;
                case GamePhase.first_auction:
                    GamePhase = GamePhase.card_exchange;
                    PassTurn(1);
                    break;
                case GamePhase.card_exchange:
                    GamePhase = GamePhase.final_auction;
                    PassTurn(1);
                    break;
                case GamePhase.final_auction:
                    GamePhase = GamePhase.showdown;
                    PassTurn(1);
                    break;
                case GamePhase.showdown:
                    GamePhase = GamePhase.gameover;
                    GetVictoryPlayer();
                    Invoke("RestartGame", 3);
                    break;
            }
        }
    }
}
