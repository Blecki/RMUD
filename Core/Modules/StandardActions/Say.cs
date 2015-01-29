﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
	internal class Say : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
                Or(
                    Sequence(
                        Or(
                            KeyWord("SAY"),
                            KeyWord("'")),
                        MustMatch("Say what?", Rest("SPEECH"))),
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
                .Manual("Speak within your locale.")
                .Perform("speak", "ACTOR", "SPEECH");


            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("EMOTE"),
                        KeyWord("\"")),
                    MustMatch("You exist. Actually this is an error message, but that's what you just told me to say.", Rest("SPEECH"))))
                .Manual("Perform an action, visible within your locale.")
                .Perform("emote", "ACTOR", "SPEECH");
		}

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, String>("speak", "[Actor, Text] : Handle the actor speaking the text.", "actor", "text");

            GlobalRules.Perform<MudObject, String>("speak")
                .Do((actor, text) =>
                {
                    MudObject.SendLocaleMessage(actor, "^<the0> : \"" + text + "\"", actor);
                    return PerformResult.Continue;
                })
                .Name("Default motormouth rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, String>("emote", "[Actor, Text] : Handle the actor emoting the text.", "actor", "text");

            GlobalRules.Perform<MudObject, String>("emote")
                .Do((actor, text) =>
                {
                    MudObject.SendLocaleMessage(actor, "^<the0> " + text, actor);
                    return PerformResult.Continue;
                })
                .Name("Default exhibitionist rule.");
        }
    }
}