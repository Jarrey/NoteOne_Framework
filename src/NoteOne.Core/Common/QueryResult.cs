using System;
using Newtonsoft.Json.Linq;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace NoteOne_Core.Common
{
    public class QueryResult
    {
        public QueryResult(object response, QueryResultTypes type = QueryResultTypes.Single)
        {
            Response = response;
            QueryResultType = type;
        }

        public QueryResult(ModelBase result)
        {
            Result = result;
        }

        public QueryResult(ModelBase[] results)
        {
            Results = results;
        }

        public object Response { get; private set; }
        public QueryResultTypes QueryResultType { get; private set; }
        public ModelBase Result { get; protected set; }
        public ModelBase[] Results { get; protected set; }
        public ResponseTypes ResponseType { get; protected set; }
        public object ResponseContent { get; private set; }

        public QueryResult ParseQueryResult(object result)
        {
            Response = result;
            ParseResponse();
            return this;
        }

        protected virtual void ParseResponse()
        {
            try
            {
                switch (ResponseType)
                {
                    case ResponseTypes.Xml:
                        if (Response is string)
                        {
                            var xmldoc = new XmlDocument();
                            xmldoc.LoadXml((string) Response);
                            ResponseContent = xmldoc;
                        }
                        break;

                    case ResponseTypes.Html:
                        if (Response is string)
                            ResponseContent = Response;
                        break;
                    case ResponseTypes.Object:
                        if (Response != null)
                            ResponseContent = Response;
                        break;

                    case ResponseTypes.Json:
                        if (Response != null)
                            ResponseContent = JObject.Parse(Response.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }
    }
}