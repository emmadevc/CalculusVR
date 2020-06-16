namespace Assets.Scripts.PlotterHandler.Utils
{
    class ZoomHandler
    {
        private const int zoomLevels = 20;
        private const float minZoom = 0.08f;
        private const float maxZoom = 0.3f;
        private const float zoomInterval = (maxZoom - minZoom) / (zoomLevels - 1);

        public static float Get(float zoom)
        {
            return minZoom + zoomInterval * (zoom - 1);
        }
    }
}
