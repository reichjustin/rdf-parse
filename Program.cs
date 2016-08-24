using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using rdfin.rdf_schemas;
using VDS.RDF;
using VDS.RDF.Ontology;
using VDS.RDF.Writing;

namespace rdfin
{
    class Program
    {
        private static List<JSchema> _schemas;

        private static void Main(string[] args)
        {
            //load the schemas
            LoadSchemas();

           //load all our test messages from the messageas.json file
           var msgs = JArray.Parse(File.ReadAllText(Environment.CurrentDirectory + "/messages.json"));

            //go thru each message
            foreach (var msg in msgs)
            {
                //parse to a JObject so we can validate
                JObject obj = JObject.Parse(msg.ToString());

                //go thru each schema in memory that we want to validate against and see if it matches
                _schemas.ForEach(s =>
                {
                    //if the object matches a specific schema then we need to package for delivery to rti/generate a triple from it
                    if (obj.IsValid(s))
                    {
                        //do all the packaging
                        var result = GenerateContentForRtiDelivery(obj, s.Description);

                        //display it all nicely
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(string.Format("Message: {0} matched on Schema: {1}",
                            obj.GetValue("MessageType").Value<string>(), s.Description));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result.meta_data);
                    }
                });
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Loads all the schemas in the "json-schemas" folder into memory
        /// </summary>
        private static void LoadSchemas()
        {
            _schemas = new List<JSchema>();
            foreach (string f in Directory.EnumerateFiles(Environment.CurrentDirectory + "/json-schemas/"))
            {
                JSchema s = JSchema.Parse(File.ReadAllText(f));
                _schemas.Add(s);
            }
        }
        
        private static Content GenerateContentForRtiDelivery(JObject obj,string objectType)
        {
            var typeFromJsonSchema = Type.GetType(objectType);

            if (typeFromJsonSchema == null) throw new NullReferenceException("JSON Schema Description does not match any class types");

            var rdfSchema = Activator.CreateInstance(typeFromJsonSchema) as IRdfSchema;
          
            if (rdfSchema == null) throw new NullReferenceException("JSON Schema Description does not match any IRdfSchema classes");

            return rdfSchema.ConvertTriple(obj);
        }
    }

}
