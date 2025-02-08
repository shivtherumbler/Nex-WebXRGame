using UnityEngine;
using Tilia.Output.InteractorHaptics;

public class VRTKSoundAndHaptic : MonoBehaviour
{
    public InteractorHapticsFacade facade;
    //public bool haptic;
    //public int hapticProfile;
    //public bool sound;
    public bool left;
    public AudioSource audioSource;
    public void OnShoot(int hapticProfile, AudioClip clip, bool audioOveride)
    {
        facade.Profile = hapticProfile;
        if (left)
        {
            facade.PerformProfileHaptics(facade.LeftInteractor);
        }
        else
        {
            facade.PerformProfileHaptics(facade.RightInteractor);
        }

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
        }
        if (audioOveride||!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    public void OnShoot(int hapticProfile)
    {
        facade.Profile = hapticProfile;
        if (left)
        {
            facade.PerformProfileHaptics(facade.LeftInteractor);
        }
        else
        {
            facade.PerformProfileHaptics(facade.RightInteractor);
        }
    }
    public void OnShoot(AudioClip clip,bool audioOveride)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
        }
        if (audioOveride||!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
