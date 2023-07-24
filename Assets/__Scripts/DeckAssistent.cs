using System;
using System.Collections.Generic;
using System.Linq;

namespace ProspectorPrototyp
{
    public class DeckAssistent
    {
        private readonly List<CardPoker> _drawPile;

        private readonly int numStartCards = 5;
        
        public DeckAssistent(List<CardPoker> drawPile)
        {
            _drawPile = drawPile;
        }

        public List<CardPoker> TakeRandomHand()
        { 
            List<CardPoker> randomHand = new();
            int ndx;
            var Rand = new Random();

            for (int i = 0; i < numStartCards; i++)
            {
                ndx = Rand.Next(0, _drawPile.Count);
                randomHand.Add(_drawPile[ndx]);
                _drawPile.RemoveAt(ndx);
            }
            _drawPile.AddRange(randomHand);
            return randomHand;
        }

        public void TakeCardsList(List<CardPoker> hand)
        {

            foreach (CardPoker card in hand)
            {
                var takeCard = _drawPile.Where(p => p.rank == card.rank && p.suit == card.suit);
                int ndx = _drawPile.IndexOf(new List<CardPoker>(takeCard)[0]);
                _drawPile.RemoveAt(ndx);
            }
        }

        public void RecollCardList(List<CardPoker> hand)
        {
            _drawPile.AddRange(hand);
        }

    }
}
