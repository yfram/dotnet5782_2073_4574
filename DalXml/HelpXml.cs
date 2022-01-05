namespace Dal
{
    internal class HelpXml
    {
        private static T CastObject<T>(object input)
        {
            return (T)input;
        }
        /*
        static object readObject(int id, string idPropName, Object obj, Type t)
        {
            var objectRoot = XElement.Load($"@/xml/{t.Name+'s'}");//new XElement($"{t.Name+'s'}");

            XmlSerializer reader =
                new XmlSerializer(typeof(List<t>));

            System.IO.StreamReader file = new System.IO.StreamReader(
                @"c:\temp\SerializationOverview.xml");
            t overview = (t)reader.Deserialize(file);
            file.Close();
        }
        */
    }
}
