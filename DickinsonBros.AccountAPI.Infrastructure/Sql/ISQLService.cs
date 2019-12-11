using System.Data;
using System.Threading.Tasks;

namespace DickinsonBros.AccountAPI.Infrastructure.Sql
{
    public interface ISQLService
    {
        Task ExecuteAsync(string connectionString, string sql, object param = null, CommandType? commandType = null);
        Task<T> QueryFirstAsync<T>(string connectionString, string sql, object param = null, CommandType? commandType = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string connectionString, string sql, object param = null, CommandType? commandType = null);
    }
}