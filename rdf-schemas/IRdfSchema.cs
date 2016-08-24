using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF;

namespace rdfin.rdf_schemas
{
    public interface IRdfSchema
    {
        string SubjectUri { get; set; }
        string PredicateUri { get; set; }

        string SerializeAsJson(IGraph graph);

        Content ConvertTriple(JObject obj);
    }
}
