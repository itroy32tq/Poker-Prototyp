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

        [SerializeField, Tooltip("����� ����������� ��� ������ ������ ����")]
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
            //����� � �������� ������ � ���� ����������
            ResProb r = new();

            //������ 1000 �������� ��� ��������� ���������� �� ���� ������
            for (; r.cAll < cInterate; r.cAll++)
            {
                int c;

                //�������� ��� ������ �������� �� ������ ���� ���������
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

            //������� ���� ������ �� ������
            _deckAssistent.TakeCardsList(plHand);

            //��������� 5 ���� �� ������
            List<CardPoker> alienHand = _deckAssistent.TakeRandomHand();

            //���������� ���� ������ � ������
            _deckAssistent.RecollCardList(plHand);

            if (checkFlag)
            {
                //�� ������ �������� �� ��������� �������� �� �����
                if (PokerManager.Manager.GamePhase == GamePhase.first_auction) return CalcOneRandomVariant(alienHand);
                //�� ������ �������� ��������� ����������� ����, ��� ��� ��� ������� ���� ����, ��� ��������� ����� ������� ������������� �����������
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


            //������ ����������
            if (playerData == null) playerData = new PlayerData(PokerManager.CURRENT_PLAYER);
                
            PlayerData alienPlayerData = AltCombDetect.AltCombDetectd(alienHand);


            return AltCombDetect.CompareCombination(playerData, alienPlayerData);
        }

        private bool CheckFold(List<CardPoker> hand)
        {

            PlayerData pseudoPlayerData = AltCombDetect.AltCombDetectd(hand);
            
            //���������� ������ �������� ��������� �� 100, ���� 1000, ������� ����� ������
            int pseudocInterate = 100;
            ResProb r = CalcProbability(true, pseudoPlayerData, pseudocInterate);

            if (r.ProbeWin() <= probAlienFold) return true;

            return false;
        }
    }

    
}
