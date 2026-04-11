using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Repositorio.Contexto;

namespace CompraCertaAI.Repositorio
{
    public class BaseRepositorio
    {
        protected readonly IDbConnection _connection;

        protected BaseRepositorio(IDbConnection connection)
        {
            _connection = connection;
        }

    }
}