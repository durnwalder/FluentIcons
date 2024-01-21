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

        GenerateCsFile("iconType", icons);
    }

    private static void GenerateCsFile(string iconType, string filePath)
    {
        var enumMembers = new List<EnumMemberDeclarationSyntax>();
        var caseStatements = new List<SwitchSectionSyntax>();

        // Read each line from the file
        foreach (var line in File.ReadAllLines(filePath))
        {
            var enumName = line.Trim();
            var stringValue = enumName;

            enumMembers.Add(SyntaxFactory.EnumMemberDeclaration(enumName));
            caseStatements.Add(SyntaxFactory.SwitchSection()
                .WithLabels(SyntaxFactory.SingletonList<SwitchLabelSyntax>(
                    SyntaxFactory.CaseSwitchLabel(SyntaxFactory.IdentifierName(enumName))))
                .WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(stringValue))))));
        }

        var enumDeclaration = SyntaxFactory.EnumDeclaration(iconType)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(enumMembers.ToArray());

        var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("string"), "GetResourceString")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("resource"))
                .WithType(SyntaxFactory.ParseTypeName(iconType)))
            .WithBody(SyntaxFactory.Block(SyntaxFactory.SwitchStatement(SyntaxFactory.IdentifierName("resource"))
                .WithSections(SyntaxFactory.List(caseStatements))));

        var syntaxTree = SyntaxFactory.CompilationUnit()
            .AddMembers(enumDeclaration, methodDeclaration)
            .NormalizeWhitespace();

        var formattedCode = Formatter.Format(syntaxTree, MSBuildWorkspace.Create());

        // Write the formatted code to a .cs file
        File.WriteAllText($"{iconType}.cs", formattedCode.ToFullString());
    }


private static void GenerateCsFile(string iconType, Dictionary<string, int> icons)
{
    var classDeclaration = SyntaxFactory.ClassDeclaration(iconType)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));

    var addedProperties = new HashSet<string>();
    foreach (var icon in icons)
    {
        string propertyName = icon.Key;
        if (addedProperties.Contains(propertyName))
        {
            continue;
        }
        addedProperties.Add(propertyName);
        string iconValue = char.ConvertFromUtf32(icon.Value);

        var fieldDeclaration = SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(propertyName))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(iconValue)))))))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword));

        classDeclaration = classDeclaration.AddMembers(fieldDeclaration);
    }

    var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("FluentIcons"))
        .AddMembers(classDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddMembers(namespaceDeclaration)
        .NormalizeWhitespace();

    File.WriteAllText($"../FluentIcons/{iconType}.cs", compilationUnit.ToFullString());
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