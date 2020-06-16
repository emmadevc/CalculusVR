using UnityEngine;

namespace Assets.Scripts.Plotter.Cone
{
    class Cone
    {
        public GameObject gameObject { get; }
        public GameObject wrapper { get; }
        public Mesh mesh { get; }
        public MeshFilter meshFilter { get; }
        public MeshRenderer meshRenderer { get; }

        public Cone(string name)
        {
            gameObject = new GameObject();
            gameObject.name = name;

            wrapper = new GameObject();
            wrapper.transform.parent = gameObject.transform;

            mesh = new Mesh();
            meshFilter = wrapper.AddComponent<MeshFilter>();
            meshRenderer = wrapper.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
        }

        public void Clear()
        {
            Object.Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}
