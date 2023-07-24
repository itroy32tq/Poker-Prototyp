using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProspectorPrototyp
{
    [System.Serializable]
    public struct PlayerCashData
    {
        public int num;
        public int cash;

        public PlayerCashData(int Num, int Cash)
        { 
            num = Num; cash = Cash;
        }
    }

    [System.Serializable]
    public class Player
    {
        public PlayerType type = PlayerType.ai;
        public int playerNum;
        public SlotDef handSlotDef;
        public List<CardPoker> hand;
        public List<CardPoker> discardCardsInHand = new List<CardPoker>(); 

        public PlayerAction action = PlayerAction.idle;
        public int cache = 400;
        public int blind = 10;
        public int curRate = 0; 
        public int insRate = 0;
        public int exCount = 0;
        public float turnAwaitTimer = 1.5f;

        public PokerCombination combination;
        public List<CardPoker> combinationList;
        public ResProb probabiliti = null;
        public double pronWin = 0;

        /// <summary>
        /// Событие окончания принятия решения ИИ
        /// </summary>
        public event PlayerActionEvent OnPlayerActionEventFin;

        /// <summary>
        /// Событие на начала принятия решения ИИ
        /// </summary>
        public event PlayerActionEvent OnPlayerActionEventStart;

        public delegate void PlayerActionEvent(Player player);

        public CardPoker AddCard(CardPoker eCP)
        {
            if (hand == null) hand = new List<CardPoker>();

            hand.Add(eCP);

            if (type == PlayerType.human)
            {
                CardPoker[] cards = hand.ToArray();
                cards = cards.OrderBy(cd => cd.rank).ToArray();
                hand = new List<CardPoker>(cards);
            }

            eCP.SetSortingLayerName("10");
            eCP.eventualSortLayer = handSlotDef.layerName;
            FanHand();
            return eCP;
        }

        public CardPoker RemoveCard(CardPoker cp)
        {
            if (hand == null || !hand.Contains(cp)) return null;
            hand.Remove(cp);
            FanHand();
            return cp;
        }

        public void FanHand()
        {
            float startRot = handSlotDef.rot;
            if (hand.Count > 1) startRot += PokerManager.Manager.handFanDegrees * (hand.Count - 1) / 2;

            Vector3 pos;
            float rot;
            Quaternion rotQ;

            for (int i = 0; i < hand.Count; i++)
            {
                rot = startRot - PokerManager.Manager.handFanDegrees * i;
                rotQ = Quaternion.Euler(0f, 0f, rot);

                pos = Vector3.up * CardPoker.CARD_HEIGHT / 2f;
                pos = rotQ * pos;
                pos += handSlotDef.pos;
                pos.z = -0.5f * i;

                if (PokerManager.Manager.TurnPhase != TurnPhase.waiting) hand[i].timeStart = 0;

                hand[i].MoveTo(pos, rotQ);
                hand[i].state = PokerCardState.toHand;
                hand[i].FaceUp = (type == PlayerType.human);
                hand[i].eventualSortOrder = i * 4;
            }
        }
        public async UniTaskVoid TakeTurn(GamePhase gamePhase = GamePhase.idle)
        {
            Utils.tr("Player " + playerNum.ToString() + " start TakeTurn() in GamePhase ", gamePhase.ToString());

            if (type == PlayerType.human)
            {
                HumanTakeTurn(gamePhase);
                return;
            }

            PokerManager.Manager.TurnPhase = TurnPhase.waiting;

            PlayerAction curAction;
            switch (gamePhase)
            {
                case GamePhase.first_rate:

                    //асинхронный метод принятия решения, принятое решение;
                    curAction = await UniTask.RunOnThreadPool(() => PlayerActionSelect(gamePhase));
                  
                    OnPlayerActionEventFin?.Invoke(this);
                    break;

                case GamePhase.first_auction:

                    curAction = await UniTask.RunOnThreadPool(() => PlayerActionSelect(gamePhase));
                   
                    OnPlayerActionEventFin?.Invoke(this);
                    break;

                case GamePhase.card_exchange:

                    if (action == PlayerAction.pass) break;

                    List<CardPoker> cards = GetSelectCards();

                    OnPlayerActionEventFin?.Invoke(this);
                    await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer / 4));

                    PokerManager.Manager.MoveToDiscard(cards);

                    while (PokerManager.Manager.TurnPhase != TurnPhase.idle)
                    {
                        await UniTask.Yield();
                    }
                    break;

                case GamePhase.final_auction:

                    OnPlayerActionEventStart?.Invoke(this);

                    curAction = await UniTask.RunOnThreadPool(() => PlayerActionSelect(gamePhase));
                    OnPlayerActionEventFin?.Invoke(this);
                    break;

                case GamePhase.showdown:

                    if (action == PlayerAction.pass) break;

                    await UniTask.RunOnThreadPool(() => PlayerActionSelect(gamePhase));
                    Showdown();
                    OnPlayerActionEventFin?.Invoke(this);
                    break;
            }


            BankComponent.Bank.TextUpdate();
            await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer/2));

            PokerManager.Manager.TurnPhase = TurnPhase.idle;
            OnPlayerActionEventFin?.Invoke(this);
            await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer/2));
            PokerManager.Manager.PassTurn();
        }
        public void Showdown()
        {
            foreach (CardPoker tCP in hand) if (!tCP.FaceUp) tCP.FaceUp = true;

            PlayerData playerData = AltCombDetect.AltCombDetectd(hand);
            combination = playerData.PlayerCombination;
            combinationList = playerData.PlayerCombinationList;

            Debug.Log(PokerManager.CURRENT_PLAYER.playerNum + " " + PokerManager.CURRENT_PLAYER.combination.ToString());
        }
        public async UniTaskVoid HumanTakeTurn(GamePhase gamePhase = GamePhase.idle)
        {
            PokerManager.Manager.TurnPhase = TurnPhase.waiting;

            switch (gamePhase)
            {
                case GamePhase.first_rate:

                    while (!PokerManager.Manager.PassTurnClick)
                    {
                        await UniTask.Yield();
                    }
                    BankComponent.Bank.BankUpdate(this);
                    PokerManager.Manager.PassTurnClick = false;
                    break;

                case GamePhase.first_auction:
                case GamePhase.final_auction:

                    if (action == PlayerAction.pass) break;
                    while (!PokerManager.Manager.PassTurnClick)
                    {
                        await UniTask.Yield();
                    }
                    BankComponent.Bank.HumanBankUpdate(this);
                    PokerManager.Manager.PassTurnClick = false;

                    break;

                case GamePhase.card_exchange:

                    if (action == PlayerAction.pass) break;
                    while (!PokerManager.Manager.CardsExConfirmClick)
                    {
                        await UniTask.Yield();
                    }

                    PokerManager.Manager.MoveToDiscard(GetSelectCards());

                    while (PokerManager.Manager.TurnPhase != TurnPhase.idle)
                    {
                        await UniTask.Yield();
                    }
                    PokerManager.Manager.CardsExConfirmClick = false;

                    await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer));
                    break;

                case GamePhase.showdown:
                    if (action == PlayerAction.pass) break;

                    await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer));
                    Showdown();
                    break;
            }
            BankComponent.Bank.TextUpdate();
            PokerManager.Manager.TurnPhase = TurnPhase.idle;
            PokerManager.Manager.SwitchGamePhase();
        }

        public void CPCallback(CardPoker tCP)
        {
            Utils.tr("Player.CPCallback()", tCP.name, "Player " + playerNum);
            PokerManager.Manager.PassTurn();
        }

        public async UniTask<PlayerAction> PlayerActionSelect(GamePhase gamePhase)
        {
            PlayerAction curAction = PlayerAction.idle;

            switch (gamePhase)
            {
                case GamePhase.first_rate:

                    await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer/2));
                    return BankComponent.Bank.BankUpdate(this);

                case GamePhase.first_auction:
                    //получаем саму комбинацию и списко карт ее составляющую
                    PlayerData playerData = AltCombDetect.AltCombDetectd(hand);
                    combination = playerData.PlayerCombination;
                    combinationList = playerData.PlayerCombinationList;

                    Utils.tr("Player " + playerNum.ToString() + " before first auction have comb ", combination.ToString());

                    //создаем сущность вероятности победы
                    probabiliti = new ResProb();
                    //высчитываем первую вероятность
                    probabiliti = ActionPlayerManager.A_Manager.CalcProbability();

                    pronWin = probabiliti.FinaleProbCombination();

                    Debug.Log(pronWin);

                    //todo продвинутый алгоритм принятия решений
                    switch (pronWin)
                    {
                        case < 0.6:
                            action = PlayerAction.coll;
                            break;
                        case > 0.6:
                            action = PlayerAction.raise;
                            break;
                    }

                    return BankComponent.Bank.BankUpdate(this);

                case GamePhase.final_auction:

                    playerData = AltCombDetect.AltCombDetectd(hand);
                    combination = playerData.PlayerCombination;
                    combinationList = playerData.PlayerCombinationList;

                    Utils.tr("Player " + playerNum.ToString() + " before final auction have comb ", combination.ToString());

                    probabiliti = ActionPlayerManager.A_Manager.CalcProbability();

                    pronWin = probabiliti.FinaleProbCombination();

                    Debug.Log(pronWin);

                    switch (pronWin)
                    {
                        case < 0.2:
                            return action = PlayerAction.pass;
                        case < 0.6:
                            action = PlayerAction.coll;
                            break;
                        case < 0.96:
                            action = PlayerAction.raise;
                            break;
                        case < 1:
                            action = PlayerAction.allin;
                            break;
                    }
                    return BankComponent.Bank.BankUpdate(this);

                case GamePhase.showdown:
                    await UniTask.Delay(System.TimeSpan.FromSeconds(turnAwaitTimer/2));
                    return 0;
            }
            return curAction;
        }
        public List<CardPoker> GetSelectCards()
        {
            List<CardPoker> selectCards = new List<CardPoker>();

            if (type == PlayerType.ai) AISelectCards();

            foreach (CardPoker tCP in hand)
            {
                if (tCP.isСhosen) selectCards.Add(tCP);
            }

            discardCardsInHand = selectCards;
            return selectCards;
        }

        private void AISelectCards() 
        {
            foreach (CardPoker tCP in hand)
            {
                if (combinationList.Contains(tCP)) tCP.isСhosen = false;
                else
                {
                    tCP.isСhosen = true;
                    discardCardsInHand.Add(tCP);
                }
            }
        }
    }
}
