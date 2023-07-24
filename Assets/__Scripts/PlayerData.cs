using System.Collections.Generic;

namespace ProspectorPrototyp
{
    public class PlayerData
    {
        public PokerCombination PlayerCombination { get; set; }
        public List<CardPoker> PlayerCombinationList { get; set; } = new List<CardPoker>();
        public List<CardPoker> Orirginalplayerhand { get; set; } = new List<CardPoker>();
        public PlayerData DrowOpponent { get; set; } = null;

        private Player _player = null;
        public Player Player { get => _player; private set { _player = value; } }
        public PlayerData(Player pl) 
        {
            PlayerCombination = pl.combination;
            PlayerCombinationList = pl.combinationList;
            Player = pl;
        }
    }
}
