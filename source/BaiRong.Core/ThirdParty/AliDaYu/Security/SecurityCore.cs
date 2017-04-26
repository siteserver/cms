using System;
using System.Collections.Generic;
using Top.Api.Util;
using System.Threading;
using Top.Api.Report;


namespace Top.Api.Security
{

    /// <summary>
    /// 加、解密核心类
    /// </summary>
    public class SecurityCore : SecurityConstants
    {
        private static readonly ITopLogger Log = DefaultTopLogger.GetDefault();
        // 缓存用户单独分配秘钥,需要加同步锁
        private static readonly IDictionary<string, SecretContext> AppUserSecretCache = new Dictionary<string, SecretContext>();
        private static readonly IDictionary<string, SecretContext> AppSecretCache = new Dictionary<string, SecretContext>();
        private static readonly object EmptyObject = new object();
        private static readonly object CacheLock = new object();
        private static readonly object AsynQueueKeyLock = new object();

        private string randomNum;// 伪随机码
        private ITopClient topClient;
        private static IDictionary<string, object> asynQueueKey = new Dictionary<string, object>();
        private static IDictionary<string, object> appConfig;


        public static IDictionary<string, SecretContext> GetAppUserSecretCache()
        {
            return AppUserSecretCache;
        }

        /// <summary>
        /// 判断密文是否支持检索
        /// </summary>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsIndexEncrypt(string key, Nullable<Int64> version)
        {
            if (version != null && version < 0)
            {
                key = PREVIOUS + key;
            }
            else
            {
                key = CURRENT + key;
            }
            if (appConfig == null)
            {
                return false;
            }

            object encryptType = null;
            appConfig.TryGetValue(key, out encryptType);

            return INDEX_ENCRYPT_TYPE.Equals(encryptType);
        }

        /// <summary>
        /// 获取压缩长度
        /// </summary>
        /// <returns></returns>
        public int GetCompressLen()
        {
            if (appConfig != null)
            {
                object compressLen = null;
                appConfig.TryGetValue(ENCRYPT_INDEX_COMPRESS_LEN, out compressLen);

                if (compressLen != null)
                {
                    return Convert.ToInt32(compressLen);
                }
            }
            return DEFAULT_INDEX_ENCRYPT_COMPRESS_LEN;
        }

        /// <summary>
        /// 获取滑动窗口大小
        /// </summary>
        /// <returns></returns>
        public int GetSlideSize()
        {
            if (appConfig != null)
            {
                object encryptSlideSize = null;
                appConfig.TryGetValue(ENCRYPT_SLIDE_SIZE, out encryptSlideSize);
                if (encryptSlideSize != null)
                {
                    return Convert.ToInt32(encryptSlideSize);
                }
            }
            return DEFAULT_ENCRYPT_SLIDE_SIZE;
        }

        public SecurityCore(ITopClient topClient, string randomNum)
        {
            this.topClient = topClient;
            this.randomNum = randomNum;
            // 初始化报表
            ApiReporter apiReporter = new ApiReporter();
            apiReporter.InitSecret(topClient);
        }

        public void SetRandomNum(string randomNum)
        {
            this.randomNum = randomNum;
        }
        
        /// <summary>
        /// 获取秘钥
        /// </summary>
        /// <param name="session"></param>
        /// <param name="secretVersion"></param>
        /// <returns></returns>
        public SecretContext GetSecret(string session, Nullable<Int64> secretVersion)
        {
            SecretContext secretContext = GetSecret(session, GenerateSecretKey(session, secretVersion));
            if (secretContext != null)
            {
                if (secretContext.IsValid())
                {
                    return secretContext;
                }

                if (secretContext.IsMaxValid())
                {
                    // 异步更新秘钥
                    AsynUpdateSecret(session, secretVersion);
                    return secretContext;
                }

                string cacheKey = GenerateSecretKey(session, secretVersion);
                lock (CacheLock)
                {
                    if (session != null)
                    {
                        AppUserSecretCache.Remove(cacheKey);
                    }
                    else
                    {
                        AppSecretCache.Remove(cacheKey);
                    }
                }
                // 同步调用获取秘钥
                return CallSecretApi(session, secretVersion);
            }
            else
            {
                // 同步调用获取秘钥
                return CallSecretApi(session, secretVersion);
            }
        }

        private string GenerateSecretKey(string session, Nullable<Int64> secretVersion)
        {
            if (secretVersion == null)
            {
                return session;
            }

            return session + "_" + secretVersion;
        }

        /// <summary>
        /// 从本地获取秘钥信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        private SecretContext GetSecret(string session, string cacheKey)
        {
            SecretContext secretContext;
            if (session != null)
            {
                AppUserSecretCache.TryGetValue(cacheKey, out secretContext);
            }
            else
            {
                AppSecretCache.TryGetValue(cacheKey, out secretContext);
            }
            return secretContext;
        }

        /// <summary>
        /// 调用获取秘钥api
        /// </summary>
        /// <param name="session"></param>
        /// <param name="secretVersion"></param>
        /// <returns></returns>
        private SecretContext CallSecretApi(string session, Nullable<Int64> secretVersion)
        {
            // 获取伪随机码
            if (string.IsNullOrEmpty(randomNum))
            {
                throw new ArgumentException("randomNum can`t be empty");
            }

            TopSecretGetRequest request = new TopSecretGetRequest();
            request.RandomNum = randomNum;
            request.SecretVersion = secretVersion;

            TopSecretGetResponse response;
            if (session != null && session.StartsWith(UNDERLINE))
            {
                string customerUserId = session.Substring(1);
                if (!StringUtil.IsDigits(customerUserId))
                {
                    throw new ArgumentException("session invalid");
                }
                request.CustomerUserId = Convert.ToInt64(customerUserId);
                response = topClient.Execute(request, null);
            }
            else
            {
                response = topClient.Execute(request, session);
            }
            if (!response.IsError)
            {
                if (!string.IsNullOrEmpty(response.AppConfig))
                {
                    appConfig = (IDictionary<string, Object>)TopUtils.JsonToObject(response.AppConfig);
                }
                SecretContext secretContext = new SecretContext();
                if (response.Secret != null)
                {
                    long currentTime = TopUtils.GetCurrentTimeMillis();
                    secretContext.InvalidTime = currentTime + (response.Interval * 1000);
                    secretContext.MaxInvalidTime = (currentTime + (response.MaxInterval * 1000));
                    secretContext.Secret = Convert.FromBase64String(response.Secret);
                    secretContext.SecretVersion = response.SecretVersion;
                }
                else
                {
                    if (appConfig != null)
                    {
                        object publishStatus = null;
                        appConfig.TryGetValue(PUBLISH_STATUS, out publishStatus);
                        if (BETA_STATUS.Equals(publishStatus))
                        {
                            // 设置空缓存
                            SetNullCache(secretContext);
                        }
                    }
                }
                
                PutToCache(session, secretVersion, secretContext);
                return secretContext;
            }
            else
            {
                // 查找不到历史秘钥
                if ("20005".Equals(response.SubErrCode))
                {
                    SecretContext secretContext = new SecretContext();
                    // 设置空缓存
                    SetNullCache(secretContext);

                    PutToCache(session, secretVersion, secretContext);
                    return secretContext;
                }
                throw new SecretException(response.ErrCode, response.ErrMsg, response.SubErrCode, response.SubErrMsg);
            }
        }

        private void PutToCache(string session, Nullable<Int64> secretVersion, SecretContext secretContext)
        {
            string cacheKey = GenerateSecretKey(session, secretVersion);

            lock (CacheLock)
            {
                if (session != null)
                {
                    if (!AppUserSecretCache.ContainsKey(cacheKey))
                    {
                        AppUserSecretCache.Add(cacheKey, secretContext);
                    }
                }
                else
                {
                    if (!AppSecretCache.ContainsKey(cacheKey))
                    {
                        AppSecretCache.Add(cacheKey, secretContext);
                    }
                }
            }
        }

        /// <summary>
        /// 设置空缓存
        /// </summary>
        /// <param name="secretContext"></param>
        private void SetNullCache(SecretContext secretContext)
        {
            long currentTime = TopUtils.GetCurrentTimeMillis();
            secretContext.InvalidTime = currentTime + (DEFAULT_INTERVAL * 1000);
            secretContext.MaxInvalidTime = currentTime + (DEFAULT_MAX_INTERVAL * 1000);
        }

        /// <summary>
        /// 异步更新秘钥
        /// </summary>
        /// <param name="session"></param>
        /// <param name="secretVersion"></param>
        private void AsynUpdateSecret(string session, Nullable<Int64> secretVersion)
        {
            string cacheKey = GenerateSecretKey(session, secretVersion);

            lock (AsynQueueKeyLock)
            {
                // 不需要重复提交秘钥请求
                if (asynQueueKey.ContainsKey(cacheKey))
                {
                    return;
                }
                SecretContext secretContext = GetSecret(session, GenerateSecretKey(session, secretVersion));
                if (secretContext != null && secretContext.IsValid())
                {
                    return;
                }

                asynQueueKey.Add(cacheKey, EmptyObject);
            }


            WaitCallback secretApiCallback = (state) =>
            {
                try
                {
                    CallSecretApi(session, secretVersion);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("asyn update secret error: {0}", e.Message));

                }
                finally
                {
                    lock (AsynQueueKeyLock)
                    {
                        asynQueueKey.Remove(cacheKey);
                    }
                }
            };

            try
            {
                ThreadPool.QueueUserWorkItem(secretApiCallback);
            }
            catch (Exception e)
            {
                lock (AsynQueueKeyLock)
                {
                    asynQueueKey.Remove(cacheKey);
                }
                Log.Error(string.Format("add QueueUserWorkItem error: {0}", e.Message));
            }
           
        }


    }
}
