using System.Runtime.CompilerServices;

namespace HotChocolate.Skimmed;

internal static class ArgumentAssertExtensions
{
    public static ITypeDefinition ExpectInputType(this ITypeDefinition type, [CallerArgumentExpression("type")] string name = "type")
    {
        if (type is null)
        {
            throw new ArgumentNullException(name);
        }

        if (!type.IsInputType())
        {
            throw new ArgumentException("Must be an input type.", name);
        }

        return type;
    }
}
