// ------------------------------------------------------------------------------
// <copyright file="ElseIfCustomScriptActionParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Pidgin;

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass
{
    internal partial class JassParser
    {
        internal static Parser<char, IStatementLineSyntax> GetElseIfCustomScriptActionParser(
            Parser<char, IExpressionSyntax> expressionParser,
            Parser<char, Unit> whitespaceParser)
        {
            return Keyword.ElseIf.Then(whitespaceParser).Then(expressionParser).Before(Keyword.Then.Then(whitespaceParser))
                .Select<IStatementLineSyntax>(expression => new JassElseIfCustomScriptAction(expression));
        }
    }
}