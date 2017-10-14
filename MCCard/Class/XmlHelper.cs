using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace MCCard
{
    public class XmlHelper
    {
        public string toXmlString<T>(T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                xsn.Add(String.Empty, String.Empty);
                xs.Serialize(sw, t, xsn);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        ///
        /// 序列化XML字串
        /// 
        /// 要序列化的對象
        /// 編碼方式
        /// 序列化產生的XML字符串
        public string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {                    return reader.ReadToEnd();
                }
            }
        }
        private void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null) throw new ArgumentNullException("o");
            if (encoding == null) throw new ArgumentNullException("encoding");
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = " ";
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);

                writer.Close();
            }
        }
    }
}