﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRuleEngine
{
    public enum NewRuleQueueingMode
    {
        QueueNewRules,
        ImmediatelyAddNewRules
    }

    public partial class RuleEngine
    {
        public RuleSet Rules;
        internal NewRuleQueueingMode QueueingMode = NewRuleQueueingMode.ImmediatelyAddNewRules;
        internal List<Action> NewRuleQueue = new List<Action>();

        public RuleEngine(NewRuleQueueingMode QueueingMode)
        {
            this.QueueingMode = QueueingMode;
            Rules = new RuleSet(this);
        }

        public void FinalizeNewRules()
        {
            if (QueueingMode != NewRuleQueueingMode.QueueNewRules) return;
            QueueingMode = NewRuleQueueingMode.ImmediatelyAddNewRules;
            foreach (var act in NewRuleQueue) act();
            NewRuleQueue.Clear();
        }

        private void NewRule(Action act)
        {
            if (QueueingMode == NewRuleQueueingMode.QueueNewRules) NewRuleQueue.Add(act);
            else act();
        }

        internal bool CheckGlobalRuleBookTypes(String Name, Type ResultType, params Type[] ArgumentTypes)
        {
            if (Rules == null) return true; // This means that rules were declared before global rulebooks were discovered. Queueing prevents this from happening.

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;

            if (book.ResultType != ResultType) return false;
            return book.CheckArgumentTypes(ResultType, ArgumentTypes);
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            Rules.DeleteRule(RuleBookName, RuleID);
        }

        private IEnumerable<RuleSet> EnumerateRuleSets(Object[] Arguments)
        {
            if (Arguments != null)
            {
                var objectsExamined = new List<RuleSource>();

                foreach (var arg in Arguments)
                    if (arg is RuleSource
                        && !objectsExamined.Contains(arg as RuleSource)
                        && (arg as RuleSource).Rules != null)
                    {
                        objectsExamined.Add(arg as RuleSource);
                        yield return (arg as RuleSource).Rules;
                    }

                foreach (var arg in Arguments)
                    if (arg is RuleSource)
                        if ((arg as RuleSource).LinkedRuleSource != null)
                            if (!objectsExamined.Contains((arg as RuleSource).LinkedRuleSource))
                                if ((arg as RuleSource).LinkedRuleSource.Rules != null)
                                {
                                    objectsExamined.Add((arg as RuleSource).LinkedRuleSource);
                                    yield return (arg as RuleSource).LinkedRuleSource.Rules;
                                }
            }
        }

        public PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            //A single null value passed to a params argument is interpretted by C# as a null Object[]
            //reference, not as an array with a single element that is null.
            if (Arguments == null) Arguments = new Object[] { null };

            foreach (var ruleset in EnumerateRuleSets(Arguments))
                if (ruleset.ConsiderPerformRule(Name, Arguments) == PerformResult.Stop)
                    return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Arguments);
        }

        public CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            if (Arguments == null) Arguments = new Object[] { null };

            foreach (var ruleset in EnumerateRuleSets(Arguments))
            {
                var r = ruleset.ConsiderCheckRule(Name, Arguments);
                if (r != CheckResult.Continue) return r;
            }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderCheckRule(Name, Arguments);
        }

        public RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            if (Arguments == null) Arguments = new Object[] { null };

            bool valueReturned = false;

            foreach (var ruleset in EnumerateRuleSets(Arguments))
            {
                var r = ruleset.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
                if (valueReturned) return r;
            }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
        }
    }    
}
