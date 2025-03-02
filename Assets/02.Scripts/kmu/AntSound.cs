using System.Collections;
using System.Collections.Generic;
using Team.manager;
using UnityEngine;

public class AntSound : MonoBehaviour
{
    public AudioSource audioSource;
    public void AntMoveSound()
    {
        AudioManager.Instance.PlaySFX(audioSource, SFXType.AntMove);
    }
}
