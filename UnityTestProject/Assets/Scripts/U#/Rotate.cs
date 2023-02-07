namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// A simple script to rotate a gameobject 
    /// </summary>
    [AddComponentMenu("OpsLib/Rotate")]
    public class Rotate : UdonSharpBehaviour
    {
        [SerializeField]
        private float RotateRateX = 1.0f;
        [SerializeField]
        private float RotateRateY = 1.0f;
        [SerializeField]
        private float RotateRateZ = 1.0f;

        [SerializeField]
        private GameObject Target;

        private void Update()
        {
            if (Target == null)
            {
                Target = this.gameObject;
            }
            var rotation = new Vector3(RotateRateX, RotateRateY, RotateRateZ) * Time.deltaTime;
            Target.transform.Rotate(rotation);
        }
    }
}