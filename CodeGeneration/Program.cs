using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System.Xml.Linq;  // Add this line

class Program
{
    static void Main()
    {
        Generate("Filled");
        Generate("Regular");
    }

    private static void Generate(string iconType)
    {
        string jsonPath = $"Resources/FluentSystemIcons-{iconType}.json";
        string json = File.ReadAllText(jsonPath);
        var icons = JsonSerializer.Deserialize<Dictionary<string, int>>(json);

        GenerateCsFile(iconType, icons);
    }

private static string ToPascalCase(string str)
{
    var words = str.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < words.Length; i++)
    {
        if (words[i].Length > 0)
        {
            words[i] = words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
        }
    }
    return string.Join("", words);
}
private static string RemovePrefixAndPostfix(string str)
{
    if (str.StartsWith("IcFluent"))
    {
        str = str.Substring(8);
    }

    if (str.EndsWith("Regular"))
    {
        str = str.Remove(str.Length - 7);
    }
    else if (str.EndsWith("Filled"))
    {
        str = str.Remove(str.Length - 6);
    }

    return str;
}

private static void GenerateCsFile(string iconType, Dictionary<string, int> icons)
{
    var classDeclaration = SyntaxFactory.ClassDeclaration(ToPascalCase(iconType))
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));

    var addedProperties = new HashSet<string>();
    foreach (var icon in icons)
    {
        string propertyName = RemovePrefixAndPostfix(ToPascalCase(icon.Key));
        if (addedProperties.Contains(propertyName))
        {
            continue;
        }
        addedProperties.Add(propertyName);
        string iconValue = char.ConvertFromUtf32(icon.Value);

        var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("FontImageSource"), propertyName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("IconHelper"),
                        SyntaxFactory.IdentifierName("GetFontImageSource")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] {
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(iconValue))),
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(iconType == "Filled" ? "FluentIconConstants.FilledFontFamily" : "FluentIconConstants.RegularFontFamily")))
                    })))))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
    }

    var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("FluentIcons"))
        .AddMembers(classDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.Maui.Controls")))
        .AddMembers(namespaceDeclaration)
        .WithLeadingTrivia(SyntaxFactory.Comment("//\n//This file is auto generated\n//  Do not make edits or they will be removed later\n//\n\n/// Fluent Icons\n///\n/// View the full list of icons here:\n/// https://github.com/microsoft/fluentui-system-icons/blob/main/icons.md\n///\n"))
        .NormalizeWhitespace();

    File.WriteAllText($"../FluentIcons/{ToPascalCase(iconType)}.cs", compilationUnit.ToFullString());
}


    private static void GenerateXamlFile(string iconType, Dictionary<string, int> icons)
{
    var ns = XNamespace.Get("http://schemas.microsoft.com/dotnet/2021/maui");
    var nsX = XNamespace.Get("http://schemas.microsoft.com/winfx/2009/xaml");

    var root = new XElement(ns + "ResourceDictionary",
        new XAttribute(XNamespace.Xmlns + "x", nsX),
        new XAttribute("xmlns", ns));

    var addedKeys = new HashSet<string>();
    foreach (var icon in icons)
    {
        string key = "Icon" + icon.Key.Replace("_", "");
        if (addedKeys.Contains(key))
        {
            continue;
        }
        addedKeys.Add(key);
        string unicode = "&#x" + icon.Value.ToString("x4") + ";";
        var stringElement = new XElement(nsX + "String", new XAttribute(nsX + "Key", key));
        stringElement.Add(new XText(unicode));
        root.Add(stringElement);
    }

    var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
    doc.Save($"../FluentIcons/{iconType}.xaml");
}
}