using System;
using System.IO;
using AutoQueryable.Helpers;
using AutoQueryable.Models;
using Nancy;
using Newtonsoft.Json;

namespace AutoQueryable.Nancy.Filter
{
    public static class AfterPipelileExtensions
    {
        public static void AutoQueryable(this AfterPipeline afterPipeline, NancyContext context, dynamic query, AutoQueryableProfile profile = null)
        {
            context.Items.Add("autoqueryable-query", query);
            
            afterPipeline += ctx =>
            {
                if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
                Type entityType = query.GetType().GenericTypeArguments[0];

                string queryString = ctx.Request.Url.Query;

                ctx.Response.Contents = stream =>
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var result = QueryableHelper.GetAutoQuery(queryString, entityType, query, profile);
                        writer.Write(JsonConvert.SerializeObject(result));
                    }
                };
            };
        }
    }
}