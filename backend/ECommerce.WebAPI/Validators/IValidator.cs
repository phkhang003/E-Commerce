namespace ECommerce.WebAPI.Validators;

public interface IValidator<T>
{
    Task ValidateAsync(T entity);
}
