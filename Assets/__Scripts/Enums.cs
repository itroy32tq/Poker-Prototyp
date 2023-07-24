
namespace ProspectorPrototyp
{
    public enum PlayerType
    {
        human,
        ai
    }
    public enum PlayerAction
    {
        pass,
        coll,
        raise,
        allin,
        idle
    }

    public enum PokerCardState
    {
        toDrawpile,
        drawpile,
        toHand,
        hand,
        toTarget,
        target,
        toDiscard,
        discard,
        to,
        idle


    }

    public enum TurnPhase
    {
        idle,
        pre,
        waiting,
        pos,
        gameover
    }

    public enum GamePhase
    {
        idle,
        first_rate,
        dealing,
        first_auction,
        card_exchange,
        final_auction,
        showdown,
        gameover
    }

    public enum PokerCombination
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfKind,
        Straight,
        Flush,
        FullHouse,
        FourOfKind,
        StraightFlush
    }

}
