using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DickinsonBros.AccountAPI.Infrastructure.Middleware
{
    public interface IMiddleware
    {
        Task InvokeAsync(HttpContext context);
    }
}