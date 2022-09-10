using Strem.Todos.Events.Base;

namespace Strem.Todos.Events;

public record TodoCreatedEvent(Guid TodoId) : TodoEvent(TodoId);