// ***************************************************************
// <copyright file="Configuration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System.Configuration;
    using System.Collections.Generic;
    using System.Xml;

    
    internal class CtsConfigurationSetting
    {
        private string name;
        private IList<CtsConfigurationArgument> arguments;

        internal CtsConfigurationSetting(string name)
        {
            this.name = name;
            arguments = new List<CtsConfigurationArgument>();
        }

        internal void AddArgument(string name, string value)
        {
            arguments.Add(new CtsConfigurationArgument(name, value));
        }

        public string Name => name;

        public IList<CtsConfigurationArgument> Arguments => arguments;
    }

    
    internal class CtsConfigurationArgument
    {
        private string name;
        private string value;

        internal CtsConfigurationArgument(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name => name;
        public string Value => value;
    }


    
    internal sealed class CtsConfigurationSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection properties;

        private Dictionary<string, IList<CtsConfigurationSetting>> subSections = new Dictionary<string, IList<CtsConfigurationSetting>>();

        public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary => subSections;

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new ConfigurationPropertyCollection();
                }

                return properties;
            }
        }

        protected override void DeserializeSection(XmlReader reader)
        {
            IList<CtsConfigurationSetting> unnamedSubSection = new List<CtsConfigurationSetting>();

            subSections.Add(string.Empty, unnamedSubSection);

            

            if (!reader.Read() || reader.NodeType != XmlNodeType.Element)
            {
                throw new ConfigurationErrorsException("error", reader);
            }

            if (!reader.IsEmptyElement)
            {
                

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.IsEmptyElement)
                        {
                            

                            var setting = DeserializeSetting(reader);

                            unnamedSubSection.Add(setting);
                        }
                        else
                        {
                            var subSectionName = reader.Name;

                            IList<CtsConfigurationSetting> subSection;

                            if (!subSections.TryGetValue(subSectionName, out subSection))
                            {
                                subSection = new List<CtsConfigurationSetting>();

                                subSections.Add(subSectionName, subSection);
                            }

                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.IsEmptyElement)
                                    {
                                        var setting = DeserializeSetting(reader);

                                        subSection.Add(setting);
                                    }
                                    else
                                    {
                                        throw new ConfigurationErrorsException("error", reader);
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }
                                else if ((reader.NodeType == XmlNodeType.CDATA) || (reader.NodeType == XmlNodeType.Text))
                                {
                                    throw new ConfigurationErrorsException("error", reader);
                                }
                            }
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    else if ((reader.NodeType == XmlNodeType.CDATA) || (reader.NodeType == XmlNodeType.Text))
                    {
                        throw new ConfigurationErrorsException("error", reader);
                    }
                }
            }
        }

        private CtsConfigurationSetting DeserializeSetting(XmlReader reader)
        {
            var settingName = reader.Name;

            var setting = new CtsConfigurationSetting(settingName);

            if (reader.AttributeCount > 0)
            {
                while (reader.MoveToNextAttribute())
                {
                    var argumentName = reader.Name;
                    var argumentValue = reader.Value;

                    setting.AddArgument(argumentName, argumentValue);
                }
            }

            return setting;
        }
    }
}
