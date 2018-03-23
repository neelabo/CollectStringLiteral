using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;


namespace CollectStringLiteral
{
    public class Analyser
    {
        private Regex _regex;

        public Analyser(Regex regex)
        {
            _regex = regex;
        }

        public async Task AnalyseAsync(string path, CancellationToken cancellationToken)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.LoadMetadataForReferencedProjects = true;
                switch (Path.GetExtension(path).ToLower())
                {
                    case ".sln":
                        var solution = await workspace.OpenSolutionAsync(path, cancellationToken);
                        await AnalyseSoltionAsync(solution, cancellationToken);
                        break;

                    case ".csproj":
                        var project = await workspace.OpenProjectAsync(path, cancellationToken);
                        await AnalyseProjectAsync(project, cancellationToken);
                        break;

                    case ".cs":
                        await AnalyseCodeAsync(path, cancellationToken);
                        break;

                    default:
                        throw new NotSupportedException($"not support filetype: {path}");
                }
            }
        }

        public async Task AnalyseSoltionAsync(Solution solution, CancellationToken cancellationToken)
        {
            foreach (var project in solution.Projects)
            {
                await AnalyseProjectAsync(project, cancellationToken);
            }
        }

        public async Task AnalyseProjectAsync(Project project, CancellationToken cancellationToken)
        {
            foreach (var document in project.Documents)
            {
                // exclude ...
                var filename = Path.GetFileName(document.FilePath);
                if (filename == "AssemblyInfo.cs") continue;
                if (filename.StartsWith(".")) continue;

                var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
                await AnalyzeCodeAsync(syntaxTree, cancellationToken);
            }
        }

        public async Task AnalyseCodeAsync(string codePath, CancellationToken cancellationToken)
        {
            using (var reader = new StreamReader(codePath))
            {
                string code = await reader.ReadToEndAsync();
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                await AnalyzeCodeAsync(syntaxTree, cancellationToken);
            }
        }

        private async Task AnalyzeCodeAsync(SyntaxTree syntaxTree, CancellationToken cancellationToken)
        {
            var walker = new CustomSyntaxWalker(syntaxTree, _regex);
            await walker.AnalyseAsync(cancellationToken);
        }
    }
}
