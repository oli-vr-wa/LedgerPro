using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LedgerPro.Infrastructure.Extensions;

public static class EntityExtensions
{
    /// <summary>
    /// Updates the current values of an EntityEntry with the values from a source entity, excluding primary key properties. 
    /// This is useful for scenarios where you want to update an existing entity with new values while keeping the same identity.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entry">The EntityEntry representing the entity to update.</param>
    /// <param name="source">The source entity containing the new values.</param>
    /// <exception cref="ArgumentNullException">Thrown when the source entity is null.</exception>
    public static void UpdateFrom<T>(this EntityEntry<T> entry, T source) where T : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source), "The source entity cannot be null.");

        var properties = entry.Metadata.GetProperties();

        foreach (var property in properties)
        {
            if (property.IsPrimaryKey())
                continue;

            var newValue = property.PropertyInfo?.GetValue(source);
            entry.Property(property.Name).CurrentValue = newValue;
        }
    }
}
