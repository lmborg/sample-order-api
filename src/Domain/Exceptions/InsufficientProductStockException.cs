namespace Domain.Exceptions;

public class InsufficientProductStockException(List<Guid> productIds) : Exception($"Products with IDs: '{string.Join(',',productIds)}' have insufficient stock");