using HotChocolate.Skimmed;
using HotChocolate.Types;

namespace HotChocolate.Fusion.Composition.Types;

public class LookupDirectiveDefinition : DirectiveDefinition
{
    internal LookupDirectiveDefinition() : base(CompositeSchemaSpec.Lookup.Name)
    {
        Locations = DirectiveLocation.FieldDefinition;
    }
}
