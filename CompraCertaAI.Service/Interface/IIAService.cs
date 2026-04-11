using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.Service.Interface
{
    public interface IIAService
    {
        Task<string> GetAiResponseAsync(string prompt);
    }
}