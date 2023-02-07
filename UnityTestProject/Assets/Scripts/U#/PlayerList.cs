namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using TMPro;
    using System;

    /// <summary>
    /// Gets the list of players in the instance and sets it on a TMP field or sends it to an UdonBehaviour
    /// </summary>
    [AddComponentMenu("OpsLib/PlayerList")]
    public class PlayerList : UdonSharpBehaviour
    {
        [SerializeField]
        private string FromLine = "";
        [SerializeField]
        private string SubjectLine = "";
        [SerializeField]
        [Tooltip("TMP Text object if you want to write the PlayerList to a TMP Text field")]
        private TextMeshProUGUI TMPField;
        [SerializeField]
        [Tooltip("GameObject if you want to send the PlayerList to an UdonBehaviour")]
        private UdonBehaviour target; //Can be the AdvanceText Udon Graph program, or another UdonBehaviour
        [SerializeField]
        private string targetProgramVariableName = "TextBlurbs";
        [SerializeField]
        [Tooltip("Index to set the text at if sending to UdonBehaviour with string[]")]
        private int textIndex = 0;
        [UdonSynced]
        public string PlayerListText;
        private string _welcomeLine;

        void Start()
        {
            _welcomeLine = $"{FromLine}\n{SubjectLine}\n\n";
            if (TMPField != null)
                TMPField.text = _welcomeLine;
            if (target != null)
            {
                SetTextOnTarget(_welcomeLine);
            }

            //Test();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsMaster)
            {
                AppendPlayerName(player.displayName);
            }
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            Resync();
        }

        private void Resync()
        {
            if (TMPField != null)
                TMPField.text = (_welcomeLine + PlayerListText);
            if (target != null)
                SetTextOnTarget(_welcomeLine + PlayerListText);
        }

        private void AppendPlayerName(string playerName)
        {
            string timestamp = DateTime.Now.ToString();
            PlayerListText = $"{timestamp}\t{playerName}\n" + PlayerListText;
            if (TMPField != null)
                TMPField.text = (_welcomeLine + PlayerListText);
            if (target != null)
                SetTextOnTarget(_welcomeLine + PlayerListText);
        }

        private void SetTextOnTarget(string text)
        {
            var textVar = target.GetProgramVariable(targetProgramVariableName);
            if (textVar.GetType() == typeof(string[]))
            {
                string[] textArr = (string[])textVar;
                if(textArr.Length > textIndex)
                {
                    textArr[textIndex] = text;
                }
                target.SetProgramVariable(targetProgramVariableName, textArr);
            }
            else if (textVar.GetType() == typeof(string))
            {
                target.SetProgramVariable(targetProgramVariableName, text);
            }
            else
                Debug.LogError($"Error -- Can't set text on {target.name}, {targetProgramVariableName} is not a string or string[] type! Actual type: {textVar.GetType()}");
        }

        private void Test()
        {
            for(int i = 0; i < 20; i++)
            {
                AppendPlayerName("Player" + i);
            }
        }
    }
}
