using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProspectorPrototyp
{
    public class PokerCombinationDetected
    {
        public static PokerCombination DetectCombination(List<CardPoker> cards)
        {
            ulong handValue = 0;
            //List<CardPoker> cardscombinationList = new List<CardPoker>();

            CardPoker[] Acards = cards.ToArray();
            Acards = Acards.OrderByDescending(cd => cd.rank).ToArray();
            cards = new List<CardPoker>(Acards);

            int[] bin = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                       
            Dictionary<int, int> cardValues = new Dictionary<int, int>();

            foreach (CardPoker card in cards) 
            {
                if (cardValues.ContainsKey(card.rank)) cardValues[card.rank]++;
                else cardValues[card.rank] = 0;
            }


            foreach (int cardValue in cardValues.Keys)
            { 
                int cardsCount = cardValues[cardValue];

                for (int i = 0; i <= cardsCount; i++)
                {
                    var ind = (bin.Length - 1) - (cardValue) * 4;
                    bin[ind - i] = 1;
                    //handValue += (ulong)System.Math.Pow(2, i) << (cardValue) * 4;
                }
            }

            for (int i = bin.Length - 1; i >= 0; i--)
            {
                handValue += (ulong) (bin[i] * System.Math.Pow(2, bin.Length - i - 1)); 
            }

            var normalizedValue = handValue % 15;

            GetCombList(PokerCombination.FourOfKind, cardValues, cards);

            switch (normalizedValue)
            {
                case 1:
                    GetCombList(PokerCombination.FourOfKind, cardValues, cards);
                    return PokerCombination.FourOfKind;
                case 10:
                    return PokerCombination.FullHouse;
                case 9:
                    return PokerCombination.ThreeOfKind;
                case 7:
                    return PokerCombination.TwoPair;
                case 6:
                    return PokerCombination.Pair;
            }

            


            bool isSameSuit = cards.TrueForAll(card => card.suit == cards[0].suit);
            bool isOrdered = true;
            int? tmpValue = null;

            foreach (var cardValue in cards.OrderBy(card => card.rank))
            {
                if (tmpValue.HasValue)
                {
                    int deltarank = cardValue.rank - tmpValue.Value;
                    //todo с тузом пока не работает
                    if (deltarank != 1)
                    {
                        isOrdered = false;
                        break;
                    }
                    else
                    {
                        tmpValue = cardValue.rank;
                    }
                }
                else
                {
                    tmpValue = cardValue.rank;
                }
            }

            //bool isHighAce = Constants.PokerSingleSuitCardsCount - 1 == tmpValue.Value;

            if (isSameSuit)
            {
                if (isOrdered)
                {
                    return PokerCombination.StraightFlush;
                }
                else
                {
                    return PokerCombination.Flush;
                }
            }
            else
            {
                if (isOrdered)
                {
                    return PokerCombination.Straight;
                }
            }

            //PokerManager.CURRENT_PLAYER.combinationList = cardscombinationList;
            return PokerCombination.HighCard;
        }

        private static void GetCombList(PokerCombination comb, Dictionary<int, int> cardValues, List<CardPoker> cards)
        {

            int maxValueKey = cardValues.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            var newList = cards.Where(p => p.rank == maxValueKey);

            var Ncards = new List<CardPoker>(newList);
            Debug.Log(Ncards);
        }
    }
}
