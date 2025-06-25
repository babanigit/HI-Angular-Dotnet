using todo_web_api.Models;

namespace todo_web_api.Interface
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
