# 概要

C#コードに埋め込まれた文字列がある行を全て検索する、多言語化支援ツールです。

# CollectStringLiteral

C#コードに埋め込まれた文字列を全て検索します。

Roslyn を使用してコード解析を行い、文字列リテラルを含む行を抽出します。

    Usage: CollectStringLiteral.exe [-r Regex] .sln or .csproj files
    -r Regex      Regular expressions filter.

使用例：英数字以外の文字が含まれたコード行を検索する

    > CollectStringLiteral.exe -r "[^\u0000-\u007F]" foobar.sln

参考：

* [CodeFormatter](https://github.com/dotnet/codeformatter/)
* [RoslynのCodeAnalysis機能によるC#コードの構文解析](http://www.casleyconsulting.co.jp/blog-engineer/c/roslyn%e3%81%aecodeanalysis%e6%a9%9f%e8%83%bd%e3%81%ab%e3%82%88%e3%82%8bc%e3%82%b3%e3%83%bc%e3%83%89%e3%81%ae%e6%a7%8b%e6%96%87%e8%a7%a3%e6%9e%90/)

# CollectStringLiteralXaml

XAMLコードに埋め込まれた文字列を全て検索します。

XMLパースを行い、テキストエレメント、属性を全て抽出しているだけなので、本当に文字列であるかは判別できていません。
このため、正規表現オプションを使用するのが通常の使用方法となります。

    Usage: CollectStringLiteralXaml.exe [-r Regex] .xaml or folder
    -r Regex      Regular expressions filter.

使用例：英数字以外の文字が含まれたコード行を検索する

    > CollectStringLiteralXaml.exe -r "[^\u0000-\u007F]" foobarFolder

