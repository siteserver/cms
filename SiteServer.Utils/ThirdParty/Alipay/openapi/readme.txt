
            ╭───────────────────────╮
    ────┤           支付宝代码示例结构说明             ├────
            ╰───────────────────────╯ 
　                                                                  
	 Visual studio 版本：2010
	 Framework3.5以上版本
         版    权：支付宝（中国）网络技术有限公司

─────────
 1、主要类文件功能说明
─────────
┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉
DefaultAopClient.cs

public DefaultAopClient(string serverUrl, string appId, string privateKeyPem)
功能：构造方法
输入：serverUrl 非空，请求服务器地址（调试：http://openapi.alipaydev.com/gateway.do 线上：https://openapi.alipay.com/gateway.do ）
      appId 非空，应用ID
      privateKeyPem 非空，私钥
输出：调用客户端实例对象


public T Execute<T>(IAopRequest<T> request) where T : AopResponse
功能：执行请求调用（适用于不需要授权接口调用）
输入：request 接口请求对象
输出：T 请求返回对象。

public T Execute<T>(IAopRequest<T> request, string accessToken) where T : AopResponse
功能：执行请求调用（适用于需要授权接口调用）
输入：request 接口请求对象
      accessToken 授权令牌
输出：T 请求返回对象。

─────────
 2、调用示例
─────────
┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉
请参考： Aop.Api.Test.PublicTest.cs

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

─────────
 3、签名相关类
─────────
┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

Aop.Api.Util.AlipaySignature.cs

public static string RSASign(IDictionary<string, string> parameters, string privateKeyPem)
功能：RSA签名
输入：parameters 待签名参数map
      privateKeyPem 私钥
输出：签名结果

public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem)
功能：RSA验签
输入：parameters 签名参数内容map
      publicKeyPem 公钥
输出：验签结果

public static bool RSACheckContent(string signContent, string sign, string publicKeyPem)
功能：RSA验签
输入：signContent 签名参数内容字符串
      sign 签名
      publicKeyPem 公钥
输出：验签结果

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉