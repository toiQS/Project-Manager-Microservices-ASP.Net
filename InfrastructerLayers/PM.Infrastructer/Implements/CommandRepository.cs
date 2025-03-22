using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using System.Linq.Expressions;

namespace PM.Infrastructer.Implements
{
    public class CommandRepository<T, TKey> : ICommandRepository<T, TKey> where T : class where TKey : notnull
    {
        private readonly AuthDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<CommandRepository<T, TKey>> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommandRepository(AuthDbContext context, ILogger<CommandRepository<T, TKey>> logger, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        // Thêm, cập nhật, xóa dữ liệu (Mutation Methods)
        #region Data Modification - AddAsync
        /// <summary>
        /// Adds a new entity to the database if it does not already exist in the provided list.
        /// </summary>
        public async Task<ServicesResult<bool>> AddAsync(List<T> arr, T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                _logger.LogWarning("[Repository] AddAsync - Entity is null. Operation aborted.");
                return ServicesResult<bool>.Failure("Entity is null.");
            }

            try
            {
                if (!arr.Any())
                {
                    await _dbSet.AddAsync(entity, cancellationToken);
                    _logger.LogInformation("[Repository] AddAsync - Entity added successfully: {EntityName}.", typeof(T).Name);
                    return ServicesResult<bool>.Success(true);
                }
                else
                {
                    var isCheckPropertyName = typeof(T).GetProperty("Name");
                    if (isCheckPropertyName != null)
                    {
                        var isCheckName = arr.Any(x => EF.Property<TKey>(x, "Name").Equals(EF.Property<TKey>(entity, "Name")));
                        if (isCheckName)
                        {
                            _logger.LogWarning("[Repository] AddAsync - Duplicate entity detected: {EntityName}.", typeof(T).Name);
                            return ServicesResult<bool>.Failure("Name already exists.");
                        }   
                    }
                    await _dbSet.AddAsync(entity, cancellationToken);
                    _logger.LogInformation("[Repository] AddAsync - Entity added successfully: {EntityName}.", typeof(T).Name);
                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] AddAsync - Error occurred while adding {EntityName}.", typeof(T).Name);
                return ServicesResult<bool>.Failure("An error occurred while adding data.");
            }
        }
        #endregion


        #region Data Modification - UpdateAsync
        /// <summary>
        /// Updates an existing entity in the database if it does not conflict with existing records.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateAsync(List<T> arr, T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                _logger.LogWarning("[Repository] UpdateAsync - Entity is null. Operation aborted.");
                return ServicesResult<bool>.Failure("Entity is null.");
            }

            try
            {
                var identityProperty = typeof(T).GetProperty("Id");
                if (identityProperty == null)
                {
                    _logger.LogWarning("[Repository] UpdateAsync - Identity property 'Id' not found in {EntityName}.", typeof(T).Name);
                    return ServicesResult<bool>.Failure("Identity property 'Id' not found.");
                }

                var identityValue = identityProperty.GetValue(entity);

                // Kiểm tra xem entity có tồn tại không
                bool isCheckIdentity = await _dbSet.AnyAsync(x => EF.Property<object>(x, "Id").Equals(identityValue), cancellationToken);
                if (!isCheckIdentity)
                {
                    _logger.LogWarning("[Repository] UpdateAsync - Entity with Id={IdentityValue} not found in {EntityName}.", identityValue, typeof(T).Name);
                    return ServicesResult<bool>.Failure("Entity does not exist.");
                }

                if (arr.Count == 0)
                {
                    _context.Update(entity);
                    _logger.LogInformation("[Repository] UpdateAsync - Entity with Id={IdentityValue} updated successfully in {EntityName}.", identityValue, typeof(T).Name);
                }
                else
                {
                    var nameProperty = typeof(T).GetProperty("Name");
                    if (nameProperty == null)
                    {
                        _logger.LogWarning("[Repository] UpdateAsync - Property 'Name' not found in {EntityName}.", typeof(T).Name);
                        return ServicesResult<bool>.Failure("Property 'Name' not found.");
                    }

                    var entityNameValue = nameProperty.GetValue(entity);

                    // Kiểm tra trùng tên với các entity khác
                    bool isCheckName = await _dbSet.AnyAsync(x => EF.Property<object>(x, "Name").Equals(entityNameValue) && !EF.Property<object>(x, "Id").Equals(identityValue), cancellationToken);
                    if (isCheckName)
                    {
                        _logger.LogWarning("[Repository] UpdateAsync - Name '{EntityNameValue}' already exists in {EntityName}.", entityNameValue, typeof(T).Name);
                        return ServicesResult<bool>.Failure("Name already exists.");
                    }

                    _context.Update(entity);
                    _logger.LogInformation("[Repository] UpdateAsync - Entity with Id={IdentityValue} and Name='{EntityNameValue}' updated successfully in {EntityName}.", identityValue, entityNameValue, typeof(T).Name);
                }

                // Không cần SaveChangesAsync vì Unit of Work sẽ đảm nhiệm
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] UpdateAsync - Error occurred while updating {EntityName}.", typeof(T).Name);
                return ServicesResult<bool>.Failure("An error occurred while updating data.");
            }
        }
        #endregion




        #region Data Modification - PatchAsync
        /// <summary>
        /// Partially updates an entity based on the provided key-value pairs.
        /// </summary>
        public async Task<ServicesResult<bool>> PatchAsync(List<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default)
        {
            if (updateValue == null || updateValue.Count == 0)
            {
                _logger.LogWarning("[Repository] PatchAsync - Update values are null or empty.");
                return ServicesResult<bool>.Failure("Update values are null or empty.");
            }

            if (primaryKey == null)
            {
                _logger.LogWarning("[Repository] PatchAsync - Primary key is null.");
                return ServicesResult<bool>.Failure("Primary key is null.");
            }

            try
            {
                var entity = arr.FirstOrDefault(x => EF.Property<TKey>(x, "Id").Equals(primaryKey));

                if (entity == null)
                {
                    _logger.LogWarning("[Repository] PatchAsync - Entity with Id={PrimaryKey} not found.", primaryKey);
                    return ServicesResult<bool>.Failure("Entity not found.");
                }

                // Check for duplicate 'Name'
                if (updateValue.ContainsKey("Name"))
                {
                    var newName = updateValue["Name"];
                    bool isDuplicate = arr.Any(x => EF.Property<object>(x, "Name").Equals(newName) && !EF.Property<TKey>(x, "Id").Equals(primaryKey));
                    if (isDuplicate)
                    {
                        _logger.LogWarning("[Repository] PatchAsync - Name '{NewName}' already exists in {EntityName}.", newName, typeof(T).Name);
                        return ServicesResult<bool>.Failure("Name already exists.");
                    }
                }

                // Cập nhật từng property
                foreach (var item in updateValue)
                {
                    var property = entity.GetType().GetProperty(item.Key);
                    if (property == null || !property.CanWrite)
                    {
                        _logger.LogWarning("[Repository] PatchAsync - Property '{PropertyKey}' not found or is read-only in {EntityName}.", item.Key, typeof(T).Name);
                        return ServicesResult<bool>.Failure($"Property '{item.Key}' not found or is read-only.");
                    }

                    try
                    {
                        object convertedValue = ConvertToType(item.Value, property.PropertyType);
                        property.SetValue(entity, convertedValue);
                        _logger.LogInformation("[Repository] PatchAsync - Property '{PropertyKey}' updated successfully for Id={PrimaryKey}.", item.Key, primaryKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[Repository] PatchAsync - Failed to convert value for property '{PropertyKey}' on Id={PrimaryKey}.", item.Key, primaryKey);
                        return ServicesResult<bool>.Failure($"Failed to convert value for property '{item.Key}'.");
                    }
                }

                // Đánh dấu entity là Modified để UnitOfWork xử lý commit
                _context.Entry(entity).State = EntityState.Modified;

                _logger.LogInformation("[Repository] PatchAsync - Entity with Id={PrimaryKey} successfully patched.", primaryKey);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] PatchAsync - Error occurred while patching entity {EntityName} with Id={PrimaryKey}.", typeof(T).Name, primaryKey);
                return ServicesResult<bool>.Failure("An error occurred while updating data.");
            }
        }

        /// <summary>
        /// Chuyển đổi giá trị về kiểu dữ liệu phù hợp.
        /// </summary>
        private object ConvertToType(object value, Type targetType)
        {
            if (targetType == typeof(Guid))
                return Guid.Parse(value.ToString()!);

            if (targetType == typeof(DateTime))
                return DateTime.Parse(value.ToString()!);

            if (Nullable.GetUnderlyingType(targetType) != null)
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType)!);

            return Convert.ChangeType(value, targetType);
        }
        #endregion



        #region Data Modification - DeleteAsync
        /// <summary>
        /// Deletes an entity from the database based on its primary key.
        /// </summary>
        public async Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            if (primaryKey == null)
            {
                _logger.LogWarning("[Repository] DeleteAsync - Primary key is null.");
                return ServicesResult<bool>.Failure("Primary key is null.");
            }

            try
            {
                _logger.LogInformation("[Repository] DeleteAsync - Attempting to delete entity with Id={PrimaryKey}.", primaryKey);

                var entity = await _dbSet.FindAsync(new object[] { primaryKey }, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("[Repository] DeleteAsync - Entity with Id={PrimaryKey} not found. Deletion skipped.", primaryKey);
                    return ServicesResult<bool>.Failure("Entity not found.");
                }

                _dbSet.Remove(entity);

                _logger.LogInformation("[Repository] DeleteAsync - Entity with Id={PrimaryKey} marked for deletion.", primaryKey);

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] DeleteAsync - Error occurred while deleting entity with Id={PrimaryKey}.", primaryKey);
                return ServicesResult<bool>.Failure("An error occurred while deleting data.");
            }
        }
        #endregion


        #region Delete Many
        /// <summary>
        /// Deletes multiple entities based on a specific key and value.
        /// </summary>
        /// <param name="key">The property name to filter entities.</param>
        /// <param name="valueKey">The value to match for deletion.</param>
        /// <returns>A ServicesResult indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> DeleteManyAsync(string key, TKey valueKey)
        {
            if (string.IsNullOrEmpty(key) || valueKey == null)
            {
                _logger.LogError("[Repository] DeleteManyAsync failed: Invalid key or value.");
                return ServicesResult<bool>.Failure("Invalid key or value.");
            }

            try
            {
                var property = typeof(T).GetProperty(key);
                if (property == null)
                {
                    _logger.LogError("[Repository] DeleteManyAsync failed: Property '{Key}' not found.", key);
                    return ServicesResult<bool>.Failure($"Property '{key}' not found.");
                }

                var entities = await _dbSet
                    .Where(x => EF.Property<object>(x, key).Equals(valueKey))
                    .ToListAsync();

                if (!entities.Any())
                {
                    _logger.LogInformation("[Repository] No entities found for deletion with key '{Key}' and value '{ValueKey}'.", key, valueKey);
                    return ServicesResult<bool>.Success(true);
                }

                _dbSet.RemoveRange(entities);
                _logger.LogInformation("[Repository] Successfully deleted {Count} entities with key '{Key}' and value '{ValueKey}'.", entities.Count, key, valueKey);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] DeleteManyAsync encountered an error for key '{Key}' and value '{ValueKey}'.", key, valueKey);
                return ServicesResult<bool>.Failure("An unexpected error occurred.");
            }
        }
        #endregion


        #region Data Modification - DeleteManyAsync
        /// <summary>
        /// Deletes multiple entities based on dynamic key-value conditions.
        /// </summary>
        public async Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default)
        {
            if (value == null || value.Count == 0)
            {
                _logger.LogWarning("[Repository] DeleteManyAsync - Empty filter conditions. Deletion aborted.");
                return ServicesResult<bool>.Failure("Value is null or empty.");
            }

            try
            {
                IQueryable<T> query = _dbSet; // Loại bỏ AsNoTracking để EF Core theo dõi entity khi xóa
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression? combinedExpression = null;

                foreach (var condition in value)
                {
                    var property = typeof(T).GetProperty(condition.Key);
                    if (property == null)
                    {
                        _logger.LogWarning("[Repository] DeleteManyAsync - Property '{PropertyName}' not found in {EntityName}.", condition.Key, typeof(T).Name);
                        return ServicesResult<bool>.Failure($"Property '{condition.Key}' not found.");
                    }

                    var propertyExpression = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(propertyExpression, constant);

                    combinedExpression = combinedExpression == null
                        ? equalsExpression
                        : useAndOperator
                            ? Expression.AndAlso(combinedExpression, equalsExpression)
                            : Expression.OrElse(combinedExpression, equalsExpression);
                }

                if (combinedExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                    query = query.Where(lambda);
                }

                var entitiesToDelete = await query.ToListAsync(cancellationToken);
                if (entitiesToDelete.Count == 0)
                {
                    _logger.LogWarning("[Repository] DeleteManyAsync - No matching records found. Deletion skipped.");
                    return ServicesResult<bool>.Failure("No matching records found.");
                }

                _dbSet.RemoveRange(entitiesToDelete);

                _logger.LogInformation("[Repository] DeleteManyAsync - {DeletedCount} entities marked for deletion.", entitiesToDelete.Count);

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] DeleteManyAsync - Error occurred while deleting multiple entities.");
                return ServicesResult<bool>.Failure("An error occurred while deleting data.");
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<bool>> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("");
                    return ServicesResult<bool>.Failure("");
                }
                var checkPasswork = await _userManager.CheckPasswordAsync(user, password);
                if (checkPasswork)
                {
                    _logger.LogInformation("");
                    return ServicesResult<bool>.Success(true);
                }
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<bool>> Register(string email, string username, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var user = new User()
                {
                    UserName = username,
                    Email = email,
                };
                var register = await _userManager.CreateAsync(user, password);
                if (register.Succeeded)
                {
                    _logger.LogInformation("");
                    return ServicesResult<bool>.Success(true);
                }
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
        }
        #endregion
        
    }
}
