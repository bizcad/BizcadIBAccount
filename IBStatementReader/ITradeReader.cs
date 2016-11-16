using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace IBStatementReader
{
    public interface ITradeReader<T>
    {
        List<T> GetTrades(string whereSerialized);
    }
}