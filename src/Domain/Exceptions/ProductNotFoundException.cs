namespace Domain.Exceptions;

public class ProductNotFoundException(Guid id) : Exception($"Product with ID: '{id}' not found");