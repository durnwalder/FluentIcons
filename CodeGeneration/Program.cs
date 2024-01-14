using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

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
        GenerateXamlFile(iconType, icons);
    }

    private static void GenerateCsFile(string iconType, Dictionary<string, int> icons)
    {
        using var writer = new StreamWriter($"../FluentIcons/{iconType}.cs");
        writer.WriteLine("namespace FluentIcons");
        writer.WriteLine("{");
        writer.WriteLine($"    public static class {iconType}");
        writer.WriteLine("    {");
        var addedProperties = new HashSet<string>();
        foreach (var icon in icons)
        {
            string propertyName = "Icon" + icon.Key.Replace("_", "");
            if (addedProperties.Contains(propertyName))
            {
                continue;
            }
            addedProperties.Add(propertyName);
            string unicode = "\\u" + icon.Value.ToString("x4");
            writer.WriteLine($"        public static string {propertyName} => \"{unicode}\";");
        }
        writer.WriteLine("    }");
        writer.WriteLine("}");
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