using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text;

namespace TestWcfService
{
    public class TestService : ITestService
    {
        public Stream GetUsers(string format)
        {
            if (format == null) format = "json";
            switch (format.ToLower())
            {
                case "json":
                    return GetUsersJson();

                case "csv":
                    return GetUsersCsv();

                default:
                    return BadRequest("Invalid content format: " + format);
            }
        }

        private static Stream ToStream(string data)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }

        private static Stream BadRequest(string data)
        {
            var context = WebOperationContext.Current;
            if (context == null) throw new InvalidOperationException("context is null!");
            var response = context.OutgoingResponse;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.StatusDescription = "Bad Request";
            response.ContentType = "text/plain; charset=utf-8";
            return ToStream(data);
        }

        private List<UserInfo> GetUsersImpl()
        {
            return new List<UserInfo>
            {
                new UserInfo {Name = "Vasya", Login = "vasya@pupkin.com"},
                new UserInfo {Name = "Petya", Login = "petya@foobarov.com"}
            };
        }

        private Stream GetUsersJson()
        {
            var context = WebOperationContext.Current;
            if (context == null) throw new InvalidOperationException("context is null!");

            var data = GetUsersImpl();

            var serializer = new DataContractJsonSerializer(data.GetType());
            var stream = new MemoryStream();
            serializer.WriteObject(stream, data);
            stream.Seek(0, SeekOrigin.Begin);
            context.OutgoingResponse.ContentType = "application/json";
            return stream;
        }

        private Stream GetUsersCsv()
        {
            var context = WebOperationContext.Current;
            if (context == null) throw new InvalidOperationException("context is null!");

            var users = GetUsersImpl();
            var output = new MemoryStream();

            var text = new StreamWriter(output);
            text.WriteLine("Name,Login");
            foreach (var user in users)
            {
                text.WriteLine("\"{0}\",\"{1}\"", user.Name, user.Login);
            }
            text.Flush();

            context.OutgoingResponse.ContentType = "text/csv";
            context.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=users.csv");

            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

    }
}
