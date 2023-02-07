namespace OpsLib
{
    using TMPro;
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Dummy simple script to set text on a TMP text gui
    /// </summary>
    [AddComponentMenu("OpsLib/TextController")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextController : UdonSharpBehaviour
    {
        [SerializeField]
        private string _defualtText = "";

        [SerializeField]
        private TextMeshProUGUI _textOutput;

        void Start()
        {
            _textOutput = GetComponent<TextMeshProUGUI>();
            if (_textOutput != null)
                _textOutput.text = _defualtText;
            else
                Debug.LogError("TextController -- Cannot find TextMeshProUGUI component!");
        }

        public void SetText(string text)
        {
            if (_textOutput != null)
                _textOutput.text = text;
        }

        public void ResetText()
        {
            if (_textOutput != null)
                _textOutput.text = _defualtText;
        }
    }
}