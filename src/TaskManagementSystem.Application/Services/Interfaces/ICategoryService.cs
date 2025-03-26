namespace TaskManagementSystem.Application
{
    public interface ICategoryService
    {
        Task<ServiceResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        //Task<ServiceResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    }
}
