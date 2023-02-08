namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Makes sounds for physics objects when they hit/roll across a surface
    /// </summary>
    [AddComponentMenu("OpsLib/PhysSounds")]
    [RequireComponent(typeof(AudioSource))]
    public class PhysSounds : UdonSharpBehaviour
    {
        public AudioClip[] HitSounds;
        public AudioClip[] SlideSounds;
        public float VolumeScale = 10.0f;

        private AudioSource _audioSource;
        private float _resetVolume;

        void Start()
        {
            _audioSource = (AudioSource)gameObject.GetComponent(typeof(AudioSource));
            _resetVolume = _audioSource.volume;
        }

        private void OnCollisionEnter(Collision collision)
        {
            float audioVolume = collision.relativeVelocity.magnitude / VolumeScale;
            int index = Random.Range(0, HitSounds.Length);
            AudioSource.PlayClipAtPoint(HitSounds[index], this.transform.position, audioVolume);
        }

        private void OnCollisionStay(Collision collision)
        {
            //Debug.Log($"Stay Collider: {collision.gameObject.layer.ToString()}");
            if (collision.gameObject.layer != 9 && collision.gameObject.layer != 10 && collision.gameObject.layer != 28) //The Player, PlayerLocal, and PlayerInteractionCollider layers
            {
                float movementForce = collision.relativeVelocity.magnitude;
                if (movementForce > 0.1f)
                {
                    float audioVolume = movementForce / VolumeScale;
                    _audioSource.loop = true;
                    _audioSource.volume = audioVolume;

                    if (!_audioSource.isPlaying)
                    {
                        int index = Random.Range(0, SlideSounds.Length);
                        _audioSource.clip = SlideSounds[index];
                        _audioSource.Play();
                    }
                }
                else
                {
                    _audioSource.loop = false;
                    _audioSource.Stop();
                    _audioSource.volume = _resetVolume;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //Debug.Log($"Exited Collider: {collision.gameObject.layer.ToString()}");
            _audioSource.loop = false;
            _audioSource.Stop();
            _audioSource.volume = _resetVolume;
        }
    }
}