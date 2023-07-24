using UnityEngine;
using TMPro;

namespace ProspectorPrototyp
{
    public class BankComponent : MonoBehaviour
    {
        static public BankComponent Bank;

        [SerializeField]
        private int _blind = 10;

        [SerializeField]
        private TextMeshProUGUI _bankCashText;
        private int _bankCash = 0;
        private int _lastRate = 0;

        public int Blind { get => _blind; }
        public int BankCash
        {
            get => _bankCash;
            private set 
            {
                _bankCash = value;
                _bankCashText.text = _bankCash.ToString(); 
            } 
        }

        private void Awake()
        {
            Bank = this;
            _bankCashText.text = "0";
        }

        public PlayerAction BankUpdate(Player pl) 
        {
            GamePhase gamePhase = PokerManager.Manager.GamePhase;

            //если нет денег покрыть ставку то пасс
            if (pl.cache < _lastRate) 
            {
                return pl.action = PlayerAction.pass;
            } 

            switch (gamePhase)
            {
                case GamePhase.first_rate:
                    
                    _lastRate = _blind;
                    break;
                case GamePhase.first_auction:
                case GamePhase.final_auction:

                    if (pl.action == PlayerAction.raise) _lastRate += 2*_blind;
                    if (pl.action == PlayerAction.allin) _lastRate = pl.cache;
                    break;
            }
            _bankCash += _lastRate;
            pl.curRate += _lastRate;
            pl.cache -= _lastRate;
            return pl.action;
        }
        public void TextUpdate() { _bankCashText.text = _bankCash.ToString(); }

        public void HumanBankUpdate(Player pl)
        {
            switch (pl.action)
            { 
                case PlayerAction.pass:
                    return;
                case PlayerAction.coll:
                    pl.curRate += _lastRate;
                    pl.cache -= _lastRate;
                    break;
                case PlayerAction.raise:
                    _lastRate = pl.insRate;
                    pl.cache -= _lastRate;
                    pl.curRate += _lastRate;
                    break;
                case PlayerAction.allin:
                    _lastRate = pl.cache;
                    pl.curRate += _lastRate;
                    pl.cache -= _lastRate;
                    break;
            }
            _bankCash += _lastRate;
        }

    }
}
