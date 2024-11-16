using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Validators;

public class ProductValidator : IValidator<CreateProductDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public ProductValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task ValidateAsync(CreateProductDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(dto.Name))
            errors.Add("Name is required");

        if (dto.Price <= 0)
            errors.Add("Price must be greater than 0");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            errors.Add($"Category with id {dto.CategoryId} not found");

        if (errors.Any())
            throw new ValidationException(errors);
    }
}
