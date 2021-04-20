using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Parme.Net.Tests")]


// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    // This is required to support record types in runtimes prior to .net 5.  According to
    // https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809
    // since this type is only defined in .net 5, we have to define it ourselves.
    internal static class IsExternalInit {}
}