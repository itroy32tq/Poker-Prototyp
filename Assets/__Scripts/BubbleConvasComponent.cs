using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace ProspectorPrototyp
{
    public class BubbleConvasComponent : MonoBehaviour
    {
        [SerializeField]
        private Button _exitButton;

        [SerializeField]
        private GameObject _prefabBubble;

        private BubbleController _bubbleController;
        private GameObject _bubble;
        private RectTransform _rectTransform;
        [SerializeField]
        private List<RectTransform> _playerUIData = new();

        [SerializeField]
        private GameObject _prefabButtonsPanel;
        private MaineMeneController _menuController;
        private GameObject _menuPanel;

        public void ExitGame()
        { 
            Application.Quit();
        }

        public void CreateBubble()
        {
            if (_bubble == null) _bubble = Instantiate(_prefabBubble);

            _bubble.transform.SetParent(transform, false);
            _rectTransform = _bubble.GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.zero;
            _bubbleController = _bubble.GetComponent<BubbleController>();
            _bubbleController.SubscribeBubble();
            _bubbleController.PlayerUIData = _playerUIData;
        }
        public MaineMeneController CreateButtonsPanel()
        {
            if (_menuPanel == null)
            {
                _menuPanel = Instantiate(_prefabButtonsPanel);
                _menuPanel.transform.SetParent(transform, false);
                _menuController = _menuPanel.GetComponent<MaineMeneController>();
                return _menuController;
            }
            else return _menuController;
        }

    }
}
