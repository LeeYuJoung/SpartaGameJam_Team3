using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team.manager
{
    public class StageManager : MonoBehaviour
    {
        private static StageManager instance;
        public static StageManager Instance { get { return instance; } }

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
        }
    }
}
