using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
    internal class Rules
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!RULES"),
                    Optional(Object("OBJECT", InScope)),
                    Optional(Rest("BOOK-NAME"))))
                .Manual("Lists rules and rulebooks. Both arguments are optional. If no object is supplied, it will list global rules. If no book name is supplied, it will list books rather than listing rules.")
                .ProceduralRule((match, actor) =>
                {
                    if (match.ContainsKey("OBJECT"))
                    {
                        if (match.ContainsKey("BOOK-NAME"))
                            DisplaySingleBook(actor, (match["OBJECT"] as MudObject).Rules, match["BOOK-NAME"].ToString());
                        else
                            DisplayBookList(actor, (match["OBJECT"] as MudObject).Rules);
                    }
                    else if (match.ContainsKey("BOOK-NAME"))
                        DisplaySingleBook(actor, GlobalRules.Rules, match["BOOK-NAME"].ToString());
                    else
                        DisplayBookList(actor, GlobalRules.Rules);
                    return PerformResult.Continue;
                });
        }

        private static void DisplaySingleBook(Actor Actor, RuleSet From, String BookName)
        {
            if (From == null || From.FindRuleBook(BookName) == null)
                SendMessage(Actor, "[no rules]");
            else
            {
                var book = From.FindRuleBook(BookName);
                DisplayBookHeader(Actor, book);
                foreach (var rule in book.Rules)
                    SendMessage(Actor, rule.DescriptiveName == null ? "[Unnamed rule]" : rule.DescriptiveName);
            }
        }

        private static void DisplayBookHeader(Actor Actor, RuleBook Book)
        {
            SendMessage(Actor, Book.Name + " -> " + Book.ResultType.Name + " : " + Book.Description);
        }

        private static void DisplayBookList(Actor Actor, RuleSet Rules)
        {
            if (Rules == null || Rules.RuleBooks.Count == 0)
                SendMessage(Actor, "[no rules]");
            else
                foreach (var book in Rules.RuleBooks)
                    DisplayBookHeader(Actor, book);
        }

        
    }
}