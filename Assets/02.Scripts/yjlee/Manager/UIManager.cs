using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team.manager
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }

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
