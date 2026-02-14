using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;

    public void PlayRandomAudioClip()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];
        if (audioSource != null)
        {
            audioSource.PlayOneShot(randomClip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(randomClip, transform.position);
        }
    }
}
