using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TaidouCommon;
using ExitGames.Client.Photon;

public abstract class ClientHandlerBase : MonoBehaviour {

    protected OperationCode code;

    public abstract void Start();
    
    public abstract void HandlerResponseMessage(OperationResponse response);

    public abstract void OnDestroy();

    public virtual void HandlerEvent(EventData eventData) { }
}
