using System;
using System.IO;
using System.Linq;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using Nancy;
using Newtonsoft.Json;

namespace AutoQueryable.Nancy.Filter
{
    public static class AfterPipelineExtensions
    {
        public static void AutoQueryable(this AfterPipeline afterPipeline, NancyContext context, IQueryable<dynamic> query, IAutoQueryableContext autoQueryableContext)
        {
            context.Items.Add("autoqueryable-query", query);
            afterPipeline += ctx =>
            {
                if (query == null) throw new Exception("Unable to retrieve value of IQueryable from context result.");
                ctx.Response.Contents = stream =>
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var result = query.AutoQueryable(autoQueryableContext).ToAutoQueryListResult(autoQueryableContext);
                        writer.Write(JsonConvert.SerializeObject(result));
                    }
                };
            };
        }
    }
}