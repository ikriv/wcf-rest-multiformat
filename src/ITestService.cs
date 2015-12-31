using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TestWcfService
{
    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Login { get; set; }
    }

    [ServiceContract]
    public interface ITestService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "users?format={format}")]
        [ServiceKnownType(typeof(List<UserInfo>))]
        [ServiceKnownType(typeof(MemoryStream))]
        Stream GetUsers(string format);
    }
}
