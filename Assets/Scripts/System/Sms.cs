using UnityEngine;
using System;
using System.Collections.Generic;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;

public class Sms : MonoBehaviour {

    private static Sms m_instance;
    private DefaultAcsClient client;

    
    public string regionId;
    public string accessKeyId;
    public string accessSecret;
    public static Sms instance { get { return m_instance; } }



    void Awake()
    {

        if (m_instance == null)
            m_instance = this;

        InitSms();
    }

    /// <summary>
    /// 初始化Sms服务
    /// </summary>
    void InitSms()
    {
        IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessSecret);
        client = new DefaultAcsClient(profile);
    }

    /// <summary>
    /// 发送手机号
    /// </summary>
    /// <returns></returns>
    public static int Send(string phoneNum)
    {
        CommonRequest request = new CommonRequest();
        request.Method = MethodType.POST;
        request.Domain = "dysmsapi.aliyuncs.com";
        request.Version = "2017-05-25";
        request.Action = "SendSms";
        request.Protocol = ProtocolType.HTTP;
        request.AddQueryParameters("PhoneNumbers", phoneNum);
        request.AddQueryParameters("SignName", "科比论坛");
        request.AddQueryParameters("TemplateCode", "SMS_168592365");

        int code = UnityEngine.Random.Range(10000, 100000);
        request.AddQueryParameters("TemplateParam", "{\"code\":" + "\"" + code.ToString() + "\"" + "}");

        try
        {
            CommonResponse response = instance.client.GetCommonResponse(request);
            Debug.Log(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
        }
        catch (ServerException e)
        {
            Debug.LogError(e);
        }
        catch (ClientException e)
        {
            Debug.LogError(e);
        }

        return code;
    }
}
