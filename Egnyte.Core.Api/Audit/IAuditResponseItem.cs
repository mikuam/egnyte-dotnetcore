using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Audit
{
    public interface IAuditResponseItem 
    {
        string Username { get; }
        int UserID { get; }
        string Access { get; }
        DateTime Time { get; }
    }
}
