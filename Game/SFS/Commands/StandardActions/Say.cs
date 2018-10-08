﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
	internal class Say
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Or(
                    Sequence(
                        Or(
                            KeyWord("SAY"),
                            KeyWord("'")),
                        MustMatch("@say what", Rest("SPEECH"))),
                    Generic((pm, context) =>
                    {
                        var r = new List<PossibleMatch>();
                        if (pm.Next == null || pm.Next.Value.Length <= 1 || pm.Next.Value[0] != '\'')
                            return r;

                        pm.Next.Value = pm.Next.Value.Substring(1); //remove the leading '

                        var builder = new StringBuilder();
                        var node = pm.Next;
                        for (; node != null; node = node.Next)
                        {
                            builder.Append(node.Value);
                            builder.Append(" ");
                        }

                        builder.Remove(builder.Length - 1, 1);
                        r.Add(pm.EndWith("SPEECH", builder.ToString()));
                        return r;
                    }, "'[TEXT => SPEECH]")))
                .ID("StandardActions:Say")
                .Manual("Speak within your locale.")
                .Perform("speak", "ACTOR", "SPEECH");
            
            Core.DefaultParser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("EMOTE"),
                        KeyWord("\"")),
                    MustMatch("@emote what", Rest("SPEECH"))))
                .ID("StandardActions:Emote")
                .Manual("Perform an action, visible within your locale.")
                .Perform("emote", "ACTOR", "SPEECH");

            Core.StandardMessage("say what", "Say what?");
            Core.StandardMessage("emote what", "You exist. Actually this is an error message, but that's what you just told me to say.");
            Core.StandardMessage("speak", "^<the0> : \"<s1>\"");
            Core.StandardMessage("emote", "^<the0> <s1>");

            Core.GlobalRules.DeclarePerformRuleBook<Actor, String>("speak", "[Actor, Text] : Handle the actor speaking the text.", "actor", "text");

            Core.GlobalRules.Perform<Actor, String>("speak")
                .Do((actor, text) =>
                {
                    SendLocaleMessage(actor, "@speak", actor, text);
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Default motormouth rule.");

            Core.GlobalRules.DeclarePerformRuleBook<Actor, String>("emote", "[Actor, Text] : Handle the actor emoting the text.", "actor", "text");

            Core.GlobalRules.Perform<Actor, String>("emote")
                .Do((actor, text) =>
                {
                    SendLocaleMessage(actor, "@emote", actor, text);
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Default exhibitionist rule.");
        }
    }
}
