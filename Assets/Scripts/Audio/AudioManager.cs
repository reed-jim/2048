using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundSound;
    [SerializeField] private AudioSource shootBlockSound;
    [SerializeField] private AudioSource mergeBlockSound;
    [SerializeField] private AudioSource openPopupSound;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        backgroundSound.clip.LoadAudioData();
        openPopupSound.clip.LoadAudioData();

        if (shootBlockSound != null)
        {
            shootBlockSound.clip.LoadAudioData();
        }

        if (mergeBlockSound != null)
        {
            mergeBlockSound.clip.LoadAudioData();
        }
    }

    public void PlayBackgroundSound()
    {
        backgroundSound.Play();
    }

    public void PlayShootBlockSound()
    {
        shootBlockSound.Play();
    }

    public void PlayMergeBlockSound()
    {
        mergeBlockSound.Play();
    }

    public void PlayPopupSound()
    {
        openPopupSound.Play();
    }
}