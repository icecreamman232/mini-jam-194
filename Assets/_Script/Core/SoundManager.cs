using System;
using System.Collections;
using SGGames.Script.Core;
using UnityEngine;

public enum SFXID
{
    Shoot,
    Reload,
    PlayerHurt,
    BossCry,
    COUNT
}

public class SoundManager : MonoBehaviour, IGameService
{
    [SerializeField] private AudioSource m_bgmAudio;
    [SerializeField] private AudioSource m_sfxShootAudio;
    [SerializeField] private AudioSource m_sfxReloadAudio;
    [SerializeField] private AudioSource m_sfxHurtAudio;
    [SerializeField] private AudioSource m_sfxBossCryAudio;
    [SerializeField] private AudioClip[] m_bgmList;

    private int m_bgmIndex;
    private Coroutine m_bgmCoroutine;

    
    private void Awake()
    {
        ServiceLocator.RegisterService<SoundManager>(this);
        PlayBGM();
    }

    public void ChangeMusicVolume(float volume)
    {
        m_bgmAudio.volume = volume;
    }

    public void ChangeSfxVolume(float volume)
    {
        m_sfxShootAudio.volume = volume;
        m_sfxReloadAudio.volume = volume;
        m_sfxHurtAudio.volume = volume;
        m_sfxBossCryAudio.volume = volume;
    }

    public void PlaySfx(SFXID id)
    {
        switch (id)
        {
            case SFXID.Shoot:
                m_sfxShootAudio.Play();
                break;
            case SFXID.Reload:
                m_sfxReloadAudio.Play();
                break;
            case SFXID.PlayerHurt:
                m_sfxHurtAudio.Play();
                break;
            case SFXID.BossCry:
                m_sfxBossCryAudio.Play();
                break;
        }
    }
    
    public void PlayBGM()
    {
        if (m_bgmList == null || m_bgmList.Length == 0)
            return;

        // Stop any existing BGM coroutine
        if (m_bgmCoroutine != null)
        {
            StopCoroutine(m_bgmCoroutine);
        }

        // Start the BGM playlist coroutine
        m_bgmCoroutine = StartCoroutine(PlayBGMPlaylist());
    }

    private IEnumerator PlayBGMPlaylist()
    {
        while (true)
        {
            // Ensure index is within bounds
            if (m_bgmIndex >= m_bgmList.Length)
                m_bgmIndex = 0;

            // Set and play the current clip
            m_bgmAudio.clip = m_bgmList[m_bgmIndex];
            m_bgmAudio.Play();

            // Wait for the clip to finish playing
            yield return new WaitForSeconds(m_bgmAudio.clip.length);

            // Move to next clip
            m_bgmIndex = (m_bgmIndex + 1) % m_bgmList.Length;
        }
    }

    public void StopBGM()
    {
        if (m_bgmCoroutine != null)
        {
            StopCoroutine(m_bgmCoroutine);
            m_bgmCoroutine = null;
        }
        m_bgmAudio.Stop();
    }

    private void OnDestroy()
    {
        StopBGM();
    }

}
