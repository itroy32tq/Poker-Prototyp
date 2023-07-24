using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProspectorPrototyp
    {
    public class AltCombDetect : MonoBehaviour
    {
        public static PlayerData AltCombDetectd(List<CardPoker> cards)
       {

            PokerCombination comb = new ();

            List<CardPoker> winList = new ();

            PlayerData playerData = new(PokerManager.CURRENT_PLAYER)
            {
                Orirginalplayerhand = cards
            };

            Dictionary<int, int> cardValues = new Dictionary<int, int>();

            foreach (CardPoker card in cards)
            {
                if (cardValues.ContainsKey(card.rank)) cardValues[card.rank]++;
                else cardValues[card.rank] = 0;
            }

            List<KeyValuePair<int, int>> mappings = cardValues.OrderByDescending(d => d.Value).ToList();
            
            if (mappings[0].Value == 3) 
            {
                
                comb = PokerCombination.FourOfKind;
                int maxValueKey = mappings[0].Key;
                var newList = cards.Where(p => p.rank == maxValueKey);
                winList = new List<CardPoker>(newList);

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = winList;
                return playerData;
            }

            if (mappings[0].Value == 2 & mappings[1].Value == 1)
            {
                comb = PokerCombination.FullHouse;
                int maxValueKey_1 = mappings[0].Key;
                int maxValueKey_2 = mappings[1].Key;
                var newList = cards.Where(p => p.rank == maxValueKey_1 || p.rank == maxValueKey_2);
                winList = new List<CardPoker>(newList);

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = winList;
                return playerData;
            }

            if (mappings[0].Value == 2 & mappings[1].Value == 0)
            {
                comb = PokerCombination.ThreeOfKind;
                int maxValueKey = mappings[0].Key;
                var newList = cards.Where(p => p.rank == maxValueKey);
                winList = new List<CardPoker>(newList);

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = winList;
                return playerData;
            }

            if (mappings[0].Value == 1 & mappings[1].Value == 1)
            {
                comb = PokerCombination.TwoPair;
                int maxValueKey_1 = mappings[0].Key;
                int maxValueKey_2 = mappings[1].Key;
                var newList = cards.Where(p => p.rank == maxValueKey_1 || p.rank == maxValueKey_2);
                winList = new List<CardPoker>(newList);

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = winList;
                return playerData;
            }

            if (mappings[0].Value == 1 & mappings[1].Value == 0)
            {
                comb = PokerCombination.Pair;
                int maxValueKey = mappings[0].Key;
                var newList = cards.Where(p => p.rank == maxValueKey);
                winList = new List<CardPoker>(newList);

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = winList;
                return playerData;
            }

            bool isSameSuit = cards.TrueForAll(card => card.suit == cards[0].suit);
            bool isOrdered = true;
            int? tmpValue = null;

            foreach (var cardValue in cards.OrderBy(card => card.rank))
            {
                if (tmpValue.HasValue)
                {
                    int deltarank = cardValue.rank - tmpValue.Value;
                    //todo алгоритм с переходным тузом будет гораздо сложнее
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

            if (isSameSuit)
            {
                if (isOrdered)
                {
                    comb =  PokerCombination.StraightFlush;

                }
                else
                {
                    comb = PokerCombination.Flush;
                }

                playerData.PlayerCombination = comb;
                playerData.PlayerCombinationList = cards;
                return playerData;
            }
            else
            {
                if (isOrdered)
                {
                    comb = PokerCombination.Straight;

                    playerData.PlayerCombination = comb;
                    playerData.PlayerCombinationList = cards;
                    return playerData;
                }
            }
            CardPoker tcard = cards.OrderByDescending(cd => cd.rank).First();
            winList.Add(tcard);

            playerData.PlayerCombination = comb;
            playerData.PlayerCombinationList = winList;
            return playerData;
       }
        public static int CompareCombination(PlayerData dataA, PlayerData dataB)
        {
            PokerCombination combA = dataA.PlayerCombination;
            PokerCombination combB = dataB.PlayerCombination;

            if ((int)combA > (int)combB) return +1;
            if ((int)combA < (int)combB) return -1;

            List<CardPoker> listA = dataA.PlayerCombinationList;
            List<CardPoker> listB = dataB.PlayerCombinationList;
            int num_card = Mathf.Min(listA.Count(), listB.Count());
            
            for (int i = 0; i < num_card; i++)
            {
                if (listA[i].rank > listB[i].rank) return +1;
                if (listA[i].rank < listB[i].rank) return -1;
            }

            return 0;
        }
    }
}
