using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Services.BotConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibraryBotUtn.Infrastructure.ApiServices.Repositories
{
    public class SemilleroRepository: ISemilleroRepositori
    {
        private readonly string _baseUrl;

        public SemilleroRepository(string baseUrl)
        {
            this._baseUrl = $"{baseUrl}request?verb=ListSets";
        }

        public async Task<List<SetsModels>> GetCollections()
        {
            using (var client = new HttpClient())
            {
                var xml = await client.GetStringAsync(_baseUrl);
                XDocument doc = XDocument.Parse(xml);
                RemoveNamespacePrefix(doc.Root);
                List<SetsModels> sets = new List<SetsModels>();
                try
                {
                    sets = (from item in doc.Descendants("set")

                            select new SetsModels
                            {
                                setName = item.Element("setName").Value,
                                setSpec = item.Element("setSpec").Value
                            }).Where((s) => s.setSpec == "com_123456789_8198" || s.setSpec == "com_123456789_19" || s.setSpec == "com_123456789_8197").ToList();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }


                return sets;
            }
        }
        static void RemoveNamespacePrefix(XElement element)
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
