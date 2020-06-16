using UnityEngine;
using Vuforia;

public class AreaTargetHandler : MonoBehaviour, ITrackableEventHandler
{
    public AreaPlotterHandler area;
    public UIHandler handler;

    private TrackableBehaviour mTrackableBehaviour;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.TRACKED)
        {
            handler.Plot(area);
        }
    }
}
