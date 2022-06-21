namespace Publisher.Messages;

public class CustomerDeleted
{
    public int Id { get; init; }
    public string FullName { get; init; } = default!;
}