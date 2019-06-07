using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TaidouCommon;

public class TaidouClient : MonoBehaviour,IPhotonPeerListener {

    #region Constants and Fields
    public ConnectionProtocol protocol = ConnectionProtocol.Tcp;
    public string serverAddress = "127.0.0.1:4530";
    public string applicationName = "TaidouServer";
    public delegate void OnConnectToServerEvent();
    public event OnConnectToServerEvent OnConnectToServer;
    

    private static TaidouClient instance;
    private PhotonPeer peer;
    private bool isConnected = false;
    private Dictionary<OperationCode, ClientHandlerBase> clientHandlers;
    #endregion

    #region Properties
    public static TaidouClient Instance { get { return instance; } }
    public bool IsConnected { get { return isConnected; } }
    #endregion

    #region Method
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {//避免重复创建TaidouClient
            Destroy(gameObject);
            return;
        }
        clientHandlers = new Dictionary<OperationCode, ClientHandlerBase>();
        peer = new PhotonPeer(this,protocol);
        peer.Connect(serverAddress,applicationName);
        isConnected = false;
        peer.Service();
    }

	
	// Update is called once per frame
	void Update () {
        if (peer != null)
        {
            peer.Service();
        }
	}

    private ClientHandlerBase GetClientHandler(OperationCode code)
    {
        ClientHandlerBase handler = null;
        if(clientHandlers.TryGetValue(code,out handler))
        {
            return handler;
        }
        if (code != OperationCode.Sync&&code!=OperationCode.EnemySync)
        {
            Debug.Log("*********Unkown operation. No exits ClientHandler:" + code.ToString() + " *********");
        }
        return handler;
    }
   
    #endregion

    #region Public Method
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Debug level: " + level.ToString() + " :" + message);
    }

    public void OnEvent(EventData eventData)
    {
        OperationCode code = (OperationCode)eventData.Code;
        ClientHandlerBase handler = GetClientHandler(code);
        if (handler == null)
        {
            return;
        }
        else
        {
            handler.HandlerEvent(eventData);
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        OperationCode code = (OperationCode)operationResponse.OperationCode;
        ClientHandlerBase handler = GetClientHandler(code);
        if (handler == null)
        {
            return;
        }
        else
        {
            handler.HandlerResponseMessage(operationResponse);
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                isConnected = true;
                Debug.Log("Connect server successfully.");
                if (OnConnectToServer != null) OnConnectToServer();
                break;
            case StatusCode.Disconnect:
                isConnected = false;
                break;
        }
    }

   

    public void RegistClientHandler(OperationCode code,ClientHandlerBase handler)
    {
        if (!clientHandlers.ContainsKey(code))
        {
            clientHandlers.Add(code, handler);
        }
    }

    public void DisRegisterCllientHandler(OperationCode code)
    {
        if (clientHandlers.ContainsKey(code))
        {
            clientHandlers.Remove(code);
        }
    }

    public void SendOperationRequest(OperationCode code,Dictionary<byte,object> param)
    {
        peer.OpCustom((byte)code,param,true);
    }
    #endregion
}
