using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace UnityEditor.UgsPlistEditor
{
	public class ProjectInfoPlist
	{
		private string _filename;
		private XmlDocument _doc;

		public ProjectInfoPlist(string filename)
		{
			_filename = filename;
			_doc = new XmlDocument();
			_doc.Load(filename);
		}

		public void Save()
		{
			_doc.Save(_filename);
			var file = System.IO.File.ReadAllText(_filename);
			file = file.Replace("dtd\"[]>", "dtd\">");
			System.IO.File.WriteAllText(_filename, file);
		}

		private XmlNode GetRoot() { return _doc.SelectSingleNode("/plist/dict"); }

		private void AddValueNode(string name, XmlNode value)
		{
			var root = GetRoot();
			var key = _doc.CreateNode(XmlNodeType.Element, "key", "");
			key.InnerText = name;
			root.AppendChild(key);
			root.AppendChild(value);
		}

		private XmlNode CreateNode(string name, string value)
		{
			var node = _doc.CreateNode(XmlNodeType.Element, name, "");
			node.InnerText = value;
			return node;
		}

		public void AddStringValue(string name, string value)
		{
			var valueNode = _doc.CreateNode(XmlNodeType.Element, "string", "");
			valueNode.InnerText = value;
			AddValueNode(name, valueNode);
		}

		public void AddUrlType(string name, string role, IEnumerable<string> urlSchemes)
		{
			var dict = _doc.CreateNode(XmlNodeType.Element, "dict", "");

			dict.AppendChild(CreateNode("key", "CFBundleTypeRole"));
			dict.AppendChild(CreateNode("string", role));
			
			dict.AppendChild(CreateNode("key", "CFBundleURLName"));
			dict.AppendChild(CreateNode("string", name));

			dict.AppendChild(CreateNode("key", "CFBundleURLSchemes"));
			var schemes = _doc.CreateNode(XmlNodeType.Element, "array", "");
			foreach(var scheme in urlSchemes)
				schemes.AppendChild(CreateNode("string", scheme));
			dict.AppendChild(schemes);

			var valueNode = _doc.CreateNode(XmlNodeType.Element, "array", "");
			valueNode.AppendChild(dict);

			AddValueNode("CFBundleURLTypes", valueNode);
		}
	}
}
