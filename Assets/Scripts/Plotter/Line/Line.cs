using UnityEngine;

namespace Assets.Scripts.Plotter.Line
{
    internal class Line
    {
        public GameObject gameObject { get; }
        public GameObject cylinder { get; }
        public MeshFilter meshFilter { get; }
        public MeshRenderer meshRenderer { get; }

        public Line(string name)
        {
            gameObject = new GameObject();
            gameObject.name = name;

            cylinder = new GameObject();
            cylinder.transform.parent = gameObject.transform;

            meshFilter = cylinder.AddComponent<MeshFilter>();
            meshRenderer = cylinder.AddComponent<MeshRenderer>();
        }

        public void Clear()
        {
            Object.Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
