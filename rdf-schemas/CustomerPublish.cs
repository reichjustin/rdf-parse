using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.Writing;

namespace rdfin.rdf_schemas
{
    public class CustomerPublish : IRdfSchema
    {
        public string SubjectUri { get; set; }
        public string PredicateUri { get; set; }

        public CustomerPublish()
        {
            this.SubjectUri = "http://www.shiftwise.com";
            this.PredicateUri = "http://www.shiftwise.com/customer-publish";
        }

        public Content ConvertTriple(JObject obj)
        {
            var customerId = obj.GetValue("CustomerId").Value<string>();

            IGraph graph = new Graph();

            var subject = graph.CreateUriNode(UriFactory.Create(this.SubjectUri));
            var pred = graph.CreateUriNode(UriFactory.Create(this.PredicateUri));
            var value = graph.CreateLiteralNode(customerId);

            graph.Assert(new Triple(subject, pred, value));

            return new Content
            {
                meta_data = this.SerializeAsJson(graph),
                json_content = obj.ToString()
            };
        }

        public string SerializeAsJson(IGraph graph)
        {
            RdfJsonWriter rdfJson = new RdfJsonWriter();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            rdfJson.Save(graph, sw);

            return sw.ToString();
        }
    }
}
