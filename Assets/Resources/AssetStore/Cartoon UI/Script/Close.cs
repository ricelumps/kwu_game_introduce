using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CartoonUI
{
    public class Close : MonoBehaviour
    {
        public GameObject targetObject;
        public void closeObject()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }
    }
}
