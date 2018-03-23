using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CollectStringLiteral
{
    public class CustomSyntaxWalker : SyntaxWalker
    {
        private SyntaxTree _tree;
        private Regex _regex;
        private SourceText _text;
        private List<int> _lines;

        public CustomSyntaxWalker(SyntaxTree tree, Regex regex)
        {
            _tree = tree;
            _regex = regex;
        }

        public async Task AnalyseAsync(CancellationToken cancellationToken)
        {
            int analyzeErrorCount = 0;
            foreach (var item in _tree.GetDiagnostics())
            {
                analyzeErrorCount++;
                Console.WriteLine($"Analyse Error: {analyzeErrorCount}");
                Console.WriteLine(item.Severity.ToString());
                Console.WriteLine(item.GetMessage());
                Console.WriteLine(item.Id);
                Console.WriteLine(item.Location.SourceSpan.ToString());
                Console.WriteLine("");
            }

            if (analyzeErrorCount > 0)
            {
                throw new ApplicationException($"Analyse Failed: {_tree.FilePath}");
            }

            _text = await _tree.GetTextAsync(cancellationToken);
            _lines = new List<int>();

            foreach (var token in _tree.GetRoot().DescendantTokens())
            {
                this.VisitToken(token);
            }

            foreach (var line in _lines.Distinct())
            {
                Console.WriteLine($"{_tree.FilePath}({line + 1}): {_text.Lines[line]}");
            }
        }

        protected override void VisitToken(SyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }

            if (token.IsKeyword())
            {
                ////Console.WriteLine($"{"Keyword",-30}: {token.ValueText}");
            }
            else
            {
                ////Console.WriteLine($"{token.Kind(),-30}: {token.ValueText}");
                switch (token.Kind())
                {
                    case SyntaxKind.CharacterLiteralToken:
                    case SyntaxKind.StringLiteralToken:
                    case SyntaxKind.InterpolatedStringToken:
                    case SyntaxKind.InterpolatedStringText:
                    case SyntaxKind.InterpolatedStringTextToken:
                        if (IsMatch(token.ValueText))
                        {
                            GetLineAndOffset(token.SpanStart, out int line, out int offset);
                            _lines.Add(line);
                        }
                        break;
                }
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }
        }

        protected override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.WhitespaceTrivia || trivia.Kind() == SyntaxKind.EndOfLineTrivia)
            {
                return;
            }

            ////Console.WriteLine($"{trivia.Kind(),-30}: {trivia.ToFullString()}");
            base.VisitTrivia(trivia);
        }

        private void GetLineAndOffset(int position, out int lineNumber, out int offset)
        {
            var line = _text.Lines.GetLineFromPosition(position);

            lineNumber = line.LineNumber;
            offset = position - line.Start;
        }

        private bool IsMatch(string text)
        {
            return _regex == null || _regex.IsMatch(text);
        }
    }

}
