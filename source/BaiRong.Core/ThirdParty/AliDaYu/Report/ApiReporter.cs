using System;
using Top.Api.Util;
using System.Threading;
using System.Collections.Generic;
using Top.Api.Security;

namespace Top.Api.Report
{

    public class ApiReporter
    {
        private static readonly ITopLogger Log = DefaultTopLogger.GetDefault();
        private static readonly object InitSecretLock = new object();
        private static readonly int FlushInterval = 1000 * 60 * 5;// 5分钟
        private static readonly int MinFlushInterval = 1000 * 60 * 1;// 1分钟
        private static readonly string SecretType = "1";
        private static bool SecretTreadCreated = false;
        private ITopClient topClient;

        public void InitSecret(ITopClient topClient)
        {
            this.topClient = topClient;
            lock (InitSecretLock)
            {
                if (!SecretTreadCreated)
                {
                    InitSecretThread();
                    SecretTreadCreated = true;
                }
            }
        }

        private void InitSecretThread()
        {
            Thread uploadThread = new Thread(o =>
            {
                int uploadInterval = FlushInterval;
                while (true)
                {
                    try
                    {
                        Thread.Sleep(uploadInterval);
                        IDictionary<string, object> jsonMap = new Dictionary<string, object>();
                        jsonMap.Add("sessionNum", SecurityCore.GetAppUserSecretCache().Count);
                        jsonMap.Add("encryptPhoneNum", SecurityCounter.GetEncryptPhoneNum());
                        jsonMap.Add("encryptNickNum", SecurityCounter.GetEncryptNickNum());
                        jsonMap.Add("encryptReceiverNameNum", SecurityCounter.GetEncryptReceiverNameNum());

                        jsonMap.Add("decryptPhoneNum", SecurityCounter.GetDecryptPhoneNum());
                        jsonMap.Add("decryptNickNum", SecurityCounter.GetDecryptNickNum());
                        jsonMap.Add("decryptReceiverNameNum", SecurityCounter.GetDecryptReceiverNameNum());

                        jsonMap.Add("searchPhoneNum", SecurityCounter.GetSearchPhoneNum());
                        jsonMap.Add("searchNickNum", SecurityCounter.GetSearchNickNum());
                        jsonMap.Add("searchReceiverNameNum", SecurityCounter.GetSearchReceiverNameNum());
                        SecurityCounter.Reset();

                        String contentJson = TopUtils.ObjectToJson(jsonMap);
                        uploadInterval = DoUpload(contentJson, SecretType);
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("flushSecretApiReporter error: {0}", e.Message));
                    }
                }
            });
            uploadThread.IsBackground = true;
            uploadThread.Name = "flushSecretApiReporter-thread";
            uploadThread.Start();
        }

        private int DoUpload(string contentJson, string type)
        {
            int uploadInterval = FlushInterval;
            TopSdkFeedbackUploadRequest request = new TopSdkFeedbackUploadRequest();
            request.Type = type;
            request.Content = contentJson;

            TopSdkFeedbackUploadResponse response = topClient.Execute(request, null);
            if (!response.IsError)
            {
                uploadInterval = response.UploadInterval;
                if (uploadInterval < MinFlushInterval)
                {
                    uploadInterval = FlushInterval;
                }
            }
            return uploadInterval;
        }
    }
}
