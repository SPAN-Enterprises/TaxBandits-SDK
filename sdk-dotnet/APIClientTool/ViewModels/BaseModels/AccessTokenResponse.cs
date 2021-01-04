using System.Collections.Generic;
using System.Runtime.Serialization;

namespace APIClientTool.ViewModels.BaseModels
{
    [DataContract]
    public class AccessTokenResponse:BaseResponseStatus
    {
        [DataMember(Order = 1)]
        public string AccessToken { get; set; }
        [DataMember(Order = 2)]
        public string TokenType { get; set; }
        [DataMember(Order = 3)]
        public int ExpiresIn { get; set; }
        [DataMember(Order = 4)]
        public List<Error> Errors { get; set; }
    }
}