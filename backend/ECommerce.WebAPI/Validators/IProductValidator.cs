using ECommerce.WebAPI.Models.DTOs;

namespace ECommerce.WebAPI.Validators;

public interface IProductValidator
{
    Task<FluentValidation.Results.ValidationResult> ValidateAsync(CreateProductDto createProductDto);
}