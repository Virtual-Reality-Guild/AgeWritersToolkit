namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using TMPro;

    /// <summary>
    /// Makes and displays a version number on a TMP text gui
    /// TODO: Some automation for updating version numbering in a build process?
    /// </summary>
    [AddComponentMenu("OpsLib/VersionNoUpdater")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    public class VersionNoUpdater : UdonSharpBehaviour
    {
        [SerializeField] private string ApplicationName;
        [SerializeField] private string version;
        [SerializeField] private string postfix = "-alpha";
        private void Start()
        {
            TextMeshProUGUI t = gameObject.GetComponent<TextMeshProUGUI>();
            t.text = ApplicationName + " Version: " + version + postfix;
        }
    }
}