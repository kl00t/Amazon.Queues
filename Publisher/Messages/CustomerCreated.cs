namespace Publisher.Messages;

public class CustomerCreated
{
    public int Id { get; init; }
    public string FullName { get; init; } = default!;
}