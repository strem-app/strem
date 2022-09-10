using Strem.Todos.Events.Base;

namespace Strem.Todos.Events;

public record TodoRemovedEvent(Guid TodoId) : TodoEvent(TodoId);