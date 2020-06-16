using UnityEngine;
using Vuforia;

public class RevolutionSolidTargetHandler : MonoBehaviour, ITrackableEventHandler
{
    public RevolutionSolidPlotterHandler solid;
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
            handler.Plot(solid);
        }
    }
}
