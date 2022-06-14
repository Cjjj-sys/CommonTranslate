using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tmt.V20180321;
using TencentCloud.Tmt.V20180321.Models;

namespace CommonTranslate
{
    public static class TencentTranslate
    {
        public static string secretId;
        public static string secretKey;

        public static string TranslateEn2Cn(string sourceText)
        {
            string targetText;
            string targetJson;
            try
            {
                Credential cred = new Credential
                {
                    SecretId = secretId,
                    SecretKey = secretKey
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tmt.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                TmtClient client = new TmtClient(cred, "ap-chengdu", clientProfile);
                TextTranslateRequest req = new TextTranslateRequest();
                req.SourceText = sourceText;
                req.Source = "en";
                req.Target = "zh";
                req.ProjectId = 0;
                TextTranslateResponse resp = client.TextTranslateSync(req);
                targetJson = AbstractModel.ToJsonString(resp);

                Response response = JsonConvert.DeserializeObject<Response>(targetJson);
                targetText = response.TargetText;
            }
            catch (Exception exception)
            {
                targetText = exception.Message;
            }

            return targetText;
        }
    }

    public class Response
    {
        /// <summary>
        /// 翻译后的文本
        /// </summary>
        public string? TargetText { get; set; }
        /// <summary>
        /// 源文本
        /// </summary>
        public string? Source { get; set; }
        /// <summary>
        /// 目标语言
        /// </summary>
        public string? Target { get; set; }
        /// <summary>
        /// 请求ID
        /// </summary>
        public string? RequestId { get; set; }
    }
    public class Root
    {
        /// <summary>
        /// 响应实体
        /// </summary>
        public Response? Response { get; set; }
    }
}
