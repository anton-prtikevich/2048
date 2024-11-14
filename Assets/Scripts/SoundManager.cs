using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip mergeSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Если AudioSource не назначен в инспекторе, добавляем его
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayMoveSound()
    {
        if (moveSound != null)
        {
            audioSource.PlayOneShot(moveSound);
        }
    }

    public void PlayMergeSound()
    {
        if (mergeSound != null)
        {
            audioSource.PlayOneShot(mergeSound);
        }
    }
}
