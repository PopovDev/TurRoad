using UnityEngine;

namespace AI.TrafficLights
{
    public class TrafficLight : MonoBehaviour
    {
        [Space]
        [SerializeField] private Material red;
        [SerializeField] private Material yellow;
        [SerializeField] private Material green;
        [SerializeField] private Material gray;
        [Space]
        [SerializeField] private MeshRenderer redLight;
        [SerializeField] private MeshRenderer yellowLight;
        [SerializeField] private MeshRenderer greenLight;
        [Space]
        [SerializeField] private GameObject lightsPart;
        [Space]
        [SerializeField] private Transform normal;
        [SerializeField] private Transform up;
        private CarEditor _car;

        private void Start()
        {
            _car = FindObjectOfType<CarEditor>();
            _car.CamModeChanged += CamOnCamModeChanged;
        }

        public void SetColor(bool r, bool y, bool g)
        {
            redLight.material = r ? red : gray;
            yellowLight.material = y ? yellow : gray;
            greenLight.material = g ? green : gray;
        }

        private void OnDestroy() => _car.CamModeChanged -= CamOnCamModeChanged;

        private void CamOnCamModeChanged(bool camUp)
        {
            if (camUp)
            {
                lightsPart.transform.position = up.position;
                lightsPart.transform.rotation = up.rotation;
            }
            else
            {
                lightsPart.transform.position = normal.position;
                lightsPart.transform.rotation = normal.rotation;
            }
        }
    }
}