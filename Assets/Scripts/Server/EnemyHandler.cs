using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.ParamTools;


public class EnemyHandler : ClientHandlerBase
{
    public override void Start()
    {
        code = OperationCode.EnemySync;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode = (ReturnCode)response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.Success:
                break;
            case ReturnCode.Exception:
                Debug.LogError(response.DebugMessage);
                break;
        }
    }

    public override void HandlerEvent(EventData eventData)
    {
        if (eventData.Parameters.ContainsKey((byte)ParamCode.EnemyCreate))
        {
            List<EnemyCreateModel> para = ParamTool.GetParam<List<EnemyCreateModel>>(ParamCode.EnemyCreate, eventData.Parameters);
            if (para == null)
            {
                Debug.LogError("服务器传回的 敌人创建 参数解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncCreateEnemy(para);
            }
        }
        else if (eventData.Parameters.ContainsKey((byte)ParamCode.EnemyMove))
        {
            List<EnemyMoveModel> para = ParamTool.GetParam<List<EnemyMoveModel>>(ParamCode.EnemyMove, eventData.Parameters);
            if (para == null)
            {
                Debug.LogError("服务器传回的 敌人移动 参数解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncMoveEnemy(para);
            }
        }
        else if (eventData.Parameters.ContainsKey((byte)ParamCode.EnemyAm))
        {
            List<EnemyAmModel> para = ParamTool.GetParam<List<EnemyAmModel>>(ParamCode.EnemyAm, eventData.Parameters);
            if (para == null)
            {
                Debug.LogError("服务器传回的 敌人动画 参数解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncAmEnemy(para);
            }
        }
        else if (eventData.Parameters.ContainsKey((byte)ParamCode.EnemyTakeDamage))
        {
            List<EnemyTakeAttackModel> para = ParamTool.GetParam<List<EnemyTakeAttackModel>>(ParamCode.EnemyTakeDamage,eventData.Parameters);
            if (para == null)
            {
                Debug.LogError(" 服务器传回的 敌人受到攻击 参数解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncTakeAttackEnemy(para);
            }
        }
    }


    //敌人创建的请求发送
    public void CreateEnemysRequest(List<EnemyCreateModel> enemies)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.EnemyCreate, enemies);
        TaidouClient.Instance.SendOperationRequest(OperationCode.EnemySync, para);
    }
    //敌人位置和旋转同步的请求发送
    public void EnemysMoveRequest(List<EnemyMoveModel>enemies)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.EnemyMove, enemies);
        TaidouClient.Instance.SendOperationRequest(OperationCode.EnemySync, para);
    }
    //敌人动画同步的请求发送
    public void EnemysAmRequest(List<EnemyAmModel>enemies)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.EnemyAm, enemies);
        TaidouClient.Instance.SendOperationRequest(OperationCode.EnemySync, para);
    }
    //敌人受到攻击的请求发送
    public void EnemysTakeDamageRequest(List<EnemyTakeAttackModel>enemies)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.EnemyTakeDamage, enemies);
        TaidouClient.Instance.SendOperationRequest(OperationCode.EnemySync, para);
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

}
