using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EventReceiver : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;

        if (eventCode == 100)
        {
            var dataArray = (object[]) photonEvent.CustomData;
            var data = (string) dataArray[0];
            Debug.Log($"Received event {eventCode}: {data}");
        }
    }
}
