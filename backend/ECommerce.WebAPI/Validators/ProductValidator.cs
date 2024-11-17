using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;

namespace ECommerce.WebAPI.Validators;

public class ProductValidator : AbstractValidator<CreateProductDto>, IProductValidator
{
    private readonly ICategoryRepository _categoryRepository;

    public ProductValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        RuleFor(x => x.CategoryId).MustAsync(async (id, _) => 
            await _categoryRepository.ExistsAsync(id)).WithMessage("Category not found");
    }

    async Task<FluentValidation.Results.ValidationResult> IProductValidator.ValidateAsync(CreateProductDto createProductDto)
    {
        return await ValidateAsync(createProductDto);
    }
}
