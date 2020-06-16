using TMPro;
using UnityEngine;

namespace Assets.Scripts.Plotter.Label
{
    class Label
    {
        public GameObject gameObject { get; }
        public TextMeshPro mesh { get; }

        public Label(string name)
        {
            gameObject = new GameObject();
            gameObject.name = name;

            mesh = gameObject.AddComponent<TextMeshPro>();
            mesh.enableAutoSizing = true;
            mesh.fontSizeMin = 1;
            mesh.fontSizeMax = 3;
        }

        public void Clear()
        {
            Object.Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
