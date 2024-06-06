using HotChocolate.CostAnalysis.Directives;
using HotChocolate.Types;

namespace HotChocolate.CostAnalysis.DescriptorExtensions;

public static class CostEnumTypeDescriptorExtensions
{
    /// <summary>
    /// Applies the <c>@cost</c> directive. The purpose of the <c>cost</c> directive is to define a
    /// <c>weight</c> for GraphQL types, fields, and arguments. Static analysis can use these
    /// weights when calculating the overall cost of a query or response.
    /// </summary>
    /// <param name="descriptor">
    /// The enum type descriptor.
    /// </param>
    /// <param name="weight">
    /// The <c>weight</c> argument defines what value to add to the overall cost for every
    /// appearance, or possible appearance, of this enum type.
    /// </param>
    /// <returns>
    /// Returns the enum type descriptor for configuration chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IEnumTypeDescriptor Cost(this IEnumTypeDescriptor descriptor, string weight)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(new CostDirective(weight));
    }

    /// <summary>
    /// Applies the <c>@cost</c> directive.
    /// The purpose of the <c>cost</c> directive is to define a <c>weight</c> for GraphQL types,
    /// fields, and arguments. Static analysis can use these weights when calculating the overall
    /// cost of a query or response.
    /// </summary>
    /// <param name="descriptor">
    /// The enum type descriptor.
    /// </param>
    /// <param name="weight">
    /// The <c>weight</c> argument defines what value to add to the overall cost for every
    /// appearance, or possible appearance, of this enum type.
    /// </param>
    /// <returns>
    /// Returns the enum type descriptor for configuration chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IEnumTypeDescriptor<TRuntimeType> Cost<TRuntimeType>(
        this IEnumTypeDescriptor<TRuntimeType> descriptor,
        string weight)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(new CostDirective(weight));
    }
}