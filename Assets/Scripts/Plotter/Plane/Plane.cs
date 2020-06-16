using UnityEngine;

namespace Assets.Scripts.Plotter.Plane
{
    internal class Plane
    {
        public GameObject gameObject { get; }
        public Mesh mesh { get; }
        public MeshFilter meshFilter { get; }
        public MeshRenderer meshRenderer { get; }

        public Plane(string name)
        {
            gameObject = new GameObject();
            gameObject.name = name;

            mesh = new Mesh();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        public void Clear()
        {
            Object.Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}
