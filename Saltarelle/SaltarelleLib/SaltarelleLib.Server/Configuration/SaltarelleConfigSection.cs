using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Saltarelle.Configuration {
	public class SaltarelleConfigSection : ConfigurationSection {
        internal string RawXml { get; private set; }
        private static ConfigurationPropertyCollection s_properties;
        private bool isModified;

        protected override ConfigurationPropertyCollection Properties {
            get { return EnsureStaticPropertyBag(); }
        }

        public SaltarelleConfigSection() {
            EnsureStaticPropertyBag();
        }

        private static ConfigurationPropertyCollection EnsureStaticPropertyBag() {
            if (s_properties == null)
                s_properties = new ConfigurationPropertyCollection();
            return s_properties;
        }

        protected override bool IsModified() {
            return isModified;
        }

        protected override void ResetModified() {
            isModified = false;
        }

        protected override void Reset(ConfigurationElement parentSection) {
            RawXml = string.Empty;
            isModified = false;
        }

        protected override void DeserializeSection(XmlReader xmlReader) {
            RawXml = xmlReader.ReadOuterXml();
            isModified = true;
        }

        protected override string SerializeSection(ConfigurationElement parentSection, string name, ConfigurationSaveMode saveMode) {
          return RawXml;
        }
	}
}
