namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Switches a material between different textures every {slideLength} seconds
    /// </summary>
    [AddComponentMenu("OpsLib/AdvanceImageTex")]
    public class AdvanceImageTex : UdonSharpBehaviour
    {
        public Material Material;
        public Texture[] Images;
        public float slideLength = 5.0f;
        public Color offColor;

        private float _elapsedTime = 0.0f;
        private int _index = 0;

        private void Start()
        {
            if (Images.Length == 0)
            {
                Material.color = offColor;
                Material.mainTexture = null;
            }
            else
            {
                Material.color = new Color(1, 1, 1, 0.95f);
                Material.mainTexture = Images[0];
            }
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > slideLength)
            {
                if (_index + 1 >= Images.Length)
                    _index = -1;

                _elapsedTime = 0.0f;
                _index++;
                Material.mainTexture = Images[_index];
            }
        }
    }
}