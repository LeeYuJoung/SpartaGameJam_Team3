using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team.manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance { get { return instance; } }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }

            Application.targetFrameRate = 65;
        }
    }
}
