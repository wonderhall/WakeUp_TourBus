using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishAlive
{

    public class DemoScript : MonoBehaviour
    {
        public GameObject Targets;
        public GameObject Fish;
        public float SpinDelay = 2.0f;
        public float SpinSpeed = 20f;
        public bool ShowUI = true;
        
        private List<GameObject> FishPrefabs = new List<GameObject>();
        private float _spinTimer = 0;
        private Quaternion _rotationStart;
        private Quaternion _rotationEnd;
        private int _fishCounter = 0;
        private const int AddAmount = 10;

        private float elapsedTime = 0.0f;
        private int frameCount = 0;
        private float fps = 0.0f;

        void Start()
        {
            if (Fish)
            {                
                for (int i = 0; i < Fish.transform.childCount; i++)
                {
                    var child = Fish.transform.GetChild(i).gameObject;
                    FishPrefabs.Add(child);
                }
            }
            _fishCounter = FishPrefabs.Count;
        }

        // Update is called once per frame
        void Update()
        {
            _spinTimer += Time.deltaTime;
            if (_spinTimer > SpinDelay)
            {
                _spinTimer = 0;
                _rotationStart = Targets.transform.rotation;
                _rotationEnd = Random.rotation;
            }
            Targets.transform.rotation = Quaternion.RotateTowards(_rotationStart, _rotationEnd, _spinTimer * SpinSpeed);

            CalcFPS();

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ShowUI = !ShowUI;
            }
        }

        private void OnGUI()
        {
            if (!ShowUI) return;
            GUILayout.Label("FPS: " + fps.ToString("F2"));
            foreach (var fish in FishPrefabs)
            {
                if (GUILayout.Button("+"+ AddAmount.ToString() +" " + fish.name))
                {
                    for (int i=0; i<AddAmount; i++)
                    {
                        Instantiate(fish);                        
                    }
                    _fishCounter += AddAmount;
                }
            }
            GUILayout.Label("count: " + _fishCounter.ToString());
            if (GUILayout.Button("Reset"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void CalcFPS()
        {
            elapsedTime += Time.deltaTime;
            frameCount++;

            if (elapsedTime >= 1.0f)
            {
                fps = frameCount / elapsedTime;
                elapsedTime = 0;
                frameCount = 0;
            }
        }
    }
}
