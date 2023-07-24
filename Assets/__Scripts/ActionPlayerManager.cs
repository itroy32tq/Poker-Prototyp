using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProspectorPrototyp
{
    public class ActionPlayerManager : MonoBehaviour
    {
        static public ActionPlayerManager A_Manager;

        private List<CardPoker> _drawPile;

        public List<CardPoker>  DrowPile { get => _drawPile; set => _drawPile = value; }

        private DeckAssistent _deckAssistent;

        [SerializeField, Tooltip("порог вероятности для сброса псевдо руки")]
        private double probAlienFold;

        private void Awake()
        {
            A_Manager = this;

        }

        public void Initial()
        {
            _deckAssistent = new DeckAssistent(_drawPile);
        }

        public  ResProb CalcProbability(bool pseudoProb = false, PlayerData playerData = null, int cInterate = 1000)
        {
            //клсаа с хранения данных о силе комбинации
            ResProb r = new();

            //делаем 1000 прогонов для получения статистике по силе колоды
            for (; r.cAll < cInterate; r.cAll++)
            {
                int c;

                //развилка для псевдо рекурсии по слабой руке оппонента
                if (pseudoProb) c = CalcOneAlienRandomVariant(false, playerData);
                else c = CalcOneAlienRandomVariant();

                if (c <= -2) r.cAll--;

                if (c > 0) r.cWin++;
                else if (c<0) r.cLoss++;
                else r.cDraw++;
            }

            return r;
        }

        private int CalcOneAlienRandomVariant(bool checkFlag = true, PlayerData playerData = null)
        {
            int r = -2;

            List<CardPoker> plHand = new();

            if (checkFlag) plHand = new List<CardPoker>(PokerManager.CURRENT_PLAYER.hand.Concat(PokerManager.CURRENT_PLAYER.discardCardsInHand));
            else plHand = playerData.Orirginalplayerhand;

            //изымаем руку игрока из колоды
            _deckAssistent.TakeCardsList(plHand);

            //случайных 5 кард из колоды
            List<CardPoker> alienHand = _deckAssistent.TakeRandomHand();

            //возвращаем руку игрока в колоду
            _deckAssistent.RecollCardList(plHand);

            if (checkFlag)
            {
                //на первом аукционе не проверяем оппнента на сброс
                if (PokerManager.Manager.GamePhase == GamePhase.first_auction) return CalcOneRandomVariant(alienHand);
                //на втором аукционе проверяем вероятность того, что опп сам сбросит свою руку, для получения более точного распределения вероятности
                if (CheckFold(alienHand)) return CalcOneRandomVariant(alienHand);
            } 
            else
            { 
                return CalcOneRandomVariant(alienHand, playerData);
            }

            return r;
        }
        private int CalcOneRandomVariant(List<CardPoker> alienHand, PlayerData playerData = null)
        {


            //расчет комбинаций
            if (playerData == null) playerData = new PlayerData(PokerManager.CURRENT_PLAYER);
                
            PlayerData alienPlayerData = AltCombDetect.AltCombDetectd(alienHand);


            return AltCombDetect.CompareCombination(playerData, alienPlayerData);
        }

        private bool CheckFold(List<CardPoker> hand)
        {

            PlayerData pseudoPlayerData = AltCombDetect.AltCombDetectd(hand);
            
            //количество псевод итераций установил на 100, если 1000, слишком долго думает
            int pseudocInterate = 100;
            ResProb r = CalcProbability(true, pseudoPlayerData, pseudocInterate);

            if (r.ProbeWin() <= probAlienFold) return true;

            return false;
        }
    }

    
}
