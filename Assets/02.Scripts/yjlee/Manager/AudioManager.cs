using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team.manager
{
    public enum SFXType
    {
        AntMove,
        AntEat,
        AntColorChange,
        GlassClick,
        GameClear,
        GameOver,
        GameRestart,
        GameStart,
        StageClick,
        StageLock
    }

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance { get { return instance; } }

        [SerializeField] AudioClip[] bgms;
        [SerializeField] AudioClip[] sfxs;

        [SerializeField] AudioSource bgmPlayer;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Init()
        {
            bgmPlayer = GetComponent<AudioSource>();
        }

        public void PlayBGM(int sceneNum)
        {
            bgmPlayer.clip = bgms[sceneNum];
            bgmPlayer.Play();
        }

        public void PlaySFX(AudioSource sfxPlayer, SFXType soundType)
        {
            sfxPlayer.clip = sfxs[(int)soundType];
            sfxPlayer.Play();
        }
    }
}
