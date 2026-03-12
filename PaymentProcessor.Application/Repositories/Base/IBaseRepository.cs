namespace PaymentProcessor.Application.Repositories.Base;

public interface IBaseRepository<T> where T : class
{
    public Task Add(T entity);
    public Task<T?> GetByCorrelationId(Guid correlationId);

}
