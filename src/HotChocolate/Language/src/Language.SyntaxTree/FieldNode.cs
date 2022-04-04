using System;
using System.Collections.Generic;
using System.Diagnostics;
using HotChocolate.Language.Utilities;

namespace HotChocolate.Language;

/// <summary>
/// <para>
/// A field describes one discrete piece of information available to
/// request within a selection set.
/// </para>
/// <para>
/// Some fields describe complex data or relationships to other data.
/// In order to further explore this data, a field may itself contain
/// a selection set, allowing for deeply nested requests.
/// </para>
/// <para>
/// All GraphQL operations must specify their selections down to fields
/// which return scalar values to ensure an unambiguously shaped response.
/// </para>
/// <para>Field : Alias? Name Arguments? Nullability? Directives? SelectionSet?</para>
/// </summary>
public sealed class FieldNode
    : NamedSyntaxNode
    , ISelectionNode
    , IEquatable<FieldNode>
{
    public FieldNode(
        Location? location,
        NameNode name,
        NameNode? alias,
        INullabilityNode? required,
        IReadOnlyList<DirectiveNode> directives,
        IReadOnlyList<ArgumentNode> arguments,
        SelectionSetNode? selectionSet)
        : base(location, name, directives)
    {
        Alias = alias;
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        Required = required;
        SelectionSet = selectionSet;
    }

    /// <inheritdoc />
    public override SyntaxKind Kind => SyntaxKind.Field;

    /// <summary>
    /// By default a field’s response key in the response object will use that field’s name.
    /// However, you can define a different response key by specifying an alias.
    /// </summary>
    /// <value></value>
    public NameNode? Alias { get; }

    /// <summary>
    /// Gets the assigned field argument values.
    /// </summary>
    /// <value></value>
    public IReadOnlyList<ArgumentNode> Arguments { get; }

    public INullabilityNode? Required { get; }

    /// <summary>
    /// Gets the fields selection set.
    /// </summary>
    public SelectionSetNode? SelectionSet { get; }

    /// <inheritdoc />
    public override IEnumerable<ISyntaxNode> GetNodes()
    {
        if (Alias is not null)
        {
            yield return Alias;
        }

        yield return Name;

        foreach (ArgumentNode argument in Arguments)
        {
            yield return argument;
        }

        if (Required is not null)
        {
            yield return Required;
        }

        foreach (DirectiveNode directive in Directives)
        {
            yield return directive;
        }

        if (SelectionSet is not null)
        {
            yield return SelectionSet;
        }
    }

    /// <summary>
    /// Returns the GraphQL syntax representation of this <see cref="ISyntaxNode"/>.
    /// </summary>
    /// <returns>
    /// Returns the GraphQL syntax representation of this <see cref="ISyntaxNode"/>.
    /// </returns>
    public override string ToString() => SyntaxPrinter.Print(this, true);

    /// <summary>
    /// Returns the GraphQL syntax representation of this <see cref="ISyntaxNode"/>.
    /// </summary>
    /// <param name="indented">
    /// A value that indicates whether the GraphQL output should be formatted,
    /// which includes indenting nested GraphQL tokens, adding
    /// new lines, and adding white space between property names and values.
    /// </param>
    /// <returns>
    /// Returns the GraphQL syntax representation of this <see cref="ISyntaxNode"/>.
    /// </returns>
    public override string ToString(bool indented) => SyntaxPrinter.Print(this, indented);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Location" /> with <paramref name="location" />.
    /// </summary>
    /// <param name="location">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="location" />.
    /// </returns>
    public FieldNode WithLocation(Location? location)
        => new(location, Name, Alias, Required, Directives, Arguments, SelectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Name" /> with <paramref name="name" />.
    /// </summary>
    /// <param name="name">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="name" />.
    /// </returns>
    public FieldNode WithName(NameNode name)
        => new(Location, name, Alias, Required, Directives, Arguments, SelectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Alias" /> with <paramref name="alias" />.
    /// </summary>
    /// <param name="alias">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="alias" />.
    /// </returns>
    public FieldNode WithAlias(NameNode? alias)
        => new(Location, Name, alias, Required, Directives, Arguments, SelectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Directives" /> with <paramref name="directives" />.
    /// </summary>
    /// <param name="directives">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="directives" />.
    /// </returns>
    public FieldNode WithDirectives(IReadOnlyList<DirectiveNode> directives)
        => new(Location, Name, Alias, Required, directives, Arguments, SelectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Arguments" /> with <paramref name="arguments" />.
    /// </summary>
    /// <param name="arguments">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="arguments" />.
    /// </returns>
    public FieldNode WithArguments(IReadOnlyList<ArgumentNode> arguments)
        => new(Location, Name, Alias, Required, Directives, arguments, SelectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="SelectionSet" /> with <paramref name="selectionSet" />.
    /// </summary>
    /// <param name="selectionSet">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="selectionSet" />.
    /// </returns>
    public FieldNode WithSelectionSet(SelectionSetNode? selectionSet)
        => new(Location, Name, Alias, Required, Directives, Arguments, selectionSet);

    /// <summary>
    /// Creates a new node from the current instance and replaces the 
    /// <see cref="Required" /> with <paramref name="required" />.
    /// </summary>
    /// <param name="required">
    /// The location that shall be used to replace the current location.
    /// </param>
    /// <returns>
    /// Returns the new node with the new <paramref name="required" />.
    /// </returns>
    public FieldNode WithRequired(INullabilityNode? required)
        => new(Location, Name, Alias, required, Directives, Arguments, SelectionSet);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter;
    /// otherwise, false.
    /// </returns>
    public bool Equals(FieldNode? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other)
            && Alias.IsEqualTo(other.Alias)
            && Required.IsEqualTo(other.Required)
            && SelectionSet.IsEqualTo(other.SelectionSet)
            && Arguments.IsEqualTo(other.Arguments);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current object.
    /// </param>
    /// <returns>
    /// true if the specified object  is equal to the current object; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) ||
            (obj is FieldNode other && Equals(other));

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// A hash code for the current object.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(
            base.GetHashCode(),
            Alias,
            EqualityHelper.GetHashCode(Arguments),
            Required,
            SelectionSet);

    public static bool operator ==(FieldNode? left, FieldNode? right)
        => Equals(left, right);

    public static bool operator !=(FieldNode? left, FieldNode? right)
        => !Equals(left, right);
}
