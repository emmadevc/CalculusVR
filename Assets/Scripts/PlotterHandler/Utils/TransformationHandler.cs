using Assets.Scripts.EMath.Set;
using UnityEngine;

namespace Assets.Scripts.PlotterHandler.Utils
{
    class TransformationHandler
    {

        public static void Reset(GameObject gameObject)
        {
            gameObject.transform.parent = null;

            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        public static void Transform(GameObject gameObject, float rotation, float zoom)
        {
            gameObject.transform.rotation = Quaternion.Euler(rotation, 0, 0);

            /*float scale = autoZoom ? AutoZoom(xInterval, yInterval) : ZoomHandler.Get(zoom);

            gameObject.transform.localScale = new Vector3(scale, scale, scale);*/
        }
    }
}
