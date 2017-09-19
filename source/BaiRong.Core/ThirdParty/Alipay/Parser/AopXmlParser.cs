using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Aop.Api.Request;
using Aop.Api.Util;

namespace Aop.Api.Parser
{
    /// <summary>
    /// AOP XML响应通用解释器。
    /// </summary>
    public class AopXmlParser<T> : IAopParser<T> where T : AopResponse
    {
        private static Regex regex = new Regex("<(\\w+?)[ >]", RegexOptions.Compiled);
        private static Dictionary<string, XmlSerializer> parsers = new Dictionary<string, XmlSerializer>();

        #region IAopParser<T> Members

        public T Parse(string body, string charset)
        {
            XmlSerializer serializer = null;
            string rootTagName = GetRootElement(body);

            bool inc = parsers.TryGetValue(rootTagName, out serializer);
            if (!inc || serializer == null)
            {
                XmlAttributes rootAttrs = new XmlAttributes();
                rootAttrs.XmlRoot = new XmlRootAttribute(rootTagName);

                XmlAttributeOverrides attrOvrs = new XmlAttributeOverrides();
                attrOvrs.Add(typeof(T), rootAttrs);

                serializer = new XmlSerializer(typeof(T), attrOvrs);
                parsers[rootTagName] = serializer;
            }

            object obj = null;
            Encoding encoding = null;
            if (string.IsNullOrEmpty(charset))
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.GetEncoding(charset);
            }
            using (Stream stream = new MemoryStream(encoding.GetBytes(body)))
            {
                obj = serializer.Deserialize(stream);
            }

            T rsp = (T)obj;
            if (rsp != null)
            {
                rsp.Body = body;
            }
            return rsp;
        }


        public SignItem GetSignItem(IAopRequest<T> request, string reponseBody)
        {
          
            if (string.IsNullOrEmpty(reponseBody))
            {
                return null;
            }

            SignItem signItem = new SignItem();
            string sign = GetSign(reponseBody);
            signItem.Sign = sign;

            string signSourceData = GetSignSourceData(request, reponseBody);
            signItem.SignSourceDate = signSourceData;

            return signItem;
        }

        #endregion

        /// <summary>
        /// 获取XML响应的根节点名称
        /// </summary>
        private string GetRootElement(string body)
        {
            Match match = regex.Match(body);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                throw new AopException("Invalid XML response format!");
            }
        }

        private static string GetSign(string body)
        {
            string signNodeName = "<" + AlipayConstants.SIGN + ">";
            string signEndNodeName = "</" + AlipayConstants.SIGN + ">";

            int indexOfSignNode = body.IndexOf(signNodeName);
            int indexOfSignEndNode = body.IndexOf(signEndNodeName);

            if (indexOfSignNode < 0 || indexOfSignEndNode < 0)
            {
                return null;
            }

            //  签名
            int startPos = indexOfSignNode + signNodeName.Length;
            return body.Substring(startPos, indexOfSignEndNode - startPos);
        }

        private static string GetSignSourceData(IAopRequest<T> request, string body)
        {
            string rootNode = request.GetApiName().Replace(".", "_") + AlipayConstants.RESPONSE_SUFFIX;
            string errorRootNode = AlipayConstants.ERROR_RESPONSE;

            int indexOfRootNode = body.IndexOf(rootNode);
            int indexOfErrorRoot = body.IndexOf(errorRootNode);

            string result = null;
            if (indexOfRootNode > 0)
            {
                result = ParseSignSourceData(body, rootNode, indexOfRootNode);
            }
            else if (indexOfErrorRoot > 0)
            {
                result = ParseSignSourceData(body, errorRootNode, indexOfErrorRoot);
            }

            return result;
        }

        private static string ParseSignSourceData(string body, string rootNode, int indexOfRootNode)
        {

            //  第一个字母+长度+>
            int signDataStartIndex = indexOfRootNode + rootNode.Length + 1;
            int indexOfSign = body.IndexOf("<" + AlipayConstants.SIGN);
            if (indexOfSign < 0)
            {
                return null;
            }

            // 签名前减去
            int signDataEndIndex = indexOfSign;

            return body.Substring(signDataStartIndex, signDataEndIndex - signDataStartIndex);
        }


        public string EncryptSourceData(IAopRequest<T> request, string body, string encryptType, string encryptKey, string charset)
        {
            EncryptParseItem item = ParseEncryptData(request, body);

            string bodyIndexContent = body.Substring(0, item.startIndex);
            string bodyEndContent = body.Substring(item.endIndex);
            string encryptContent = AlipayEncrypt.AesDencrypt(encryptKey, item.encryptContent, charset);

            return bodyIndexContent + encryptContent + bodyEndContent;
        }


        private static EncryptParseItem ParseEncryptData(IAopRequest<T> request, string body)
        {
            string rootNode = request.GetApiName().Replace(".", "_") + AlipayConstants.RESPONSE_SUFFIX;
            string errorRootNode = AlipayConstants.ERROR_RESPONSE;

            int indexOfRootNode = body.IndexOf(rootNode);
            int indexOfErrorRoot = body.IndexOf(errorRootNode);

            EncryptParseItem result = null;
            if (indexOfRootNode > 0)
            {
                result = ParseEncryptItem(body, rootNode, indexOfRootNode);
            }
            else if (indexOfErrorRoot > 0)
            {
                result = ParseEncryptItem(body, errorRootNode, indexOfErrorRoot);
            }

            return result;
        }

        private static EncryptParseItem ParseEncryptItem(string body, string rootNode, int indexOfRootNode)
        {

            //  第一个字母+长度+>
            int signDataStartIndex = indexOfRootNode + rootNode.Length + 1;

            string xmlStartNode = "<" + AlipayConstants.ENCRYPT_NODE_NAME + ">";
            string xmlEndNode = "</" + AlipayConstants.ENCRYPT_NODE_NAME + ">";
            int indexOfEncryptNode = body.IndexOf(xmlEndNode);

            if (indexOfEncryptNode < 0)
            {
                EncryptParseItem item = new EncryptParseItem();
                item.encryptContent = null;
                item.startIndex = 0;
                item.endIndex = 0;

                return item;
            }

            int startIndex = signDataStartIndex + xmlStartNode.Length;
            int bizLen = indexOfEncryptNode - startIndex;

            string encryptBizContent = body.Substring(startIndex, bizLen);

            EncryptParseItem item2 = new EncryptParseItem();
            item2.encryptContent = encryptBizContent;
            item2.startIndex = signDataStartIndex;
            item2.endIndex = indexOfEncryptNode + xmlEndNode.Length;

            return item2;

        }

    }
}
