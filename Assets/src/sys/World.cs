using UnityEngine;

namespace Supernova.Unity
{
    public class World : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        public Camera WorldCamera => worldCamera;
    }
}