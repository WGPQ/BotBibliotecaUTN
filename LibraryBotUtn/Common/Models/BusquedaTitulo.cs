using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;

namespace LibraryBotUtn.Common.Models
{
    public  class BusquedaTitulo
    {
        static string url = "http://repositorio.utn.edu.ec:8080/oai/request?verb=ListSets";

        public  async Task<List<SetsModels>> BuscarTitulo()
        {
            using (var client = new HttpClient())
            {
                var xml = await client.GetStringAsync(url);
                XDocument doc = XDocument.Parse(xml);
                RemoveNamespacePrefix(doc.Root);

                List<SetsModels> sets = (from item in doc.Descendants("set")

                 select new SetsModels
                 {
                     setName = item.Element("setName").Value,
                     setSpec = item.Element("setSpec").Value
                 }).ToList();

                return sets;
            }
        }
        public void RemoveNamespacePrefix(XElement element)
        {
            //Remove from element
            if (element.Name.Namespace != null)
                element.Name = element.Name.LocalName;

            //Remove from attributes
            var attributes = element.Attributes().ToArray();
            element.RemoveAttributes();
            foreach (var attr in attributes)
            {
                var newAttr = attr;

                if (attr.Name.Namespace != null)
                    newAttr = new XAttribute(attr.Name.LocalName, attr.Value);

                element.Add(newAttr);
            };

            //Remove from children
            foreach (var child in element.Descendants())
                RemoveNamespacePrefix(child);
        }

    }
}
