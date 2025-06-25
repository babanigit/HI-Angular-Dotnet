

using todo_web_api.Dtos;
using todo_web_api.Models;

namespace todo_web_api.Interface
{
    public interface ITodoRespository
    {

        Task<List<Todo>> GetUserTodo(AppUser user);

        Task<Todo> CreateAsync(Todo todo);
        // Task CreateAsync(List<Todo> userTodo);
        Task<bool> DeleteTodoAsync(AppUser user, int todoId);

        Task<Todo?> UpdateTodoAsync(AppUser user, int todoId, UpdateTodoDto dto);

    }
}