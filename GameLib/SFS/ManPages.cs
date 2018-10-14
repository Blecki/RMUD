﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using static SFS.Core;

namespace SFS
{
    public interface ManPage
    {
        String Name { get; }
        void SendManPage(Actor To);
    }

    public class StaticManPage : ManPage
    {
        public String Text;
        public String Name { get; set; }

        public StaticManPage(String Name, String Text)
        {
            this.Name = Name;
            this.Text = Text;
        }

        public void SendManPage(Actor To)
        {
            SendMessage(To, Text);
        }
    }
    
    public static partial class ManPages
    {
        internal static List<ManPage> Pages = new List<ManPage>();

        [Initialize]
        internal static void __()
        {
            foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(StaticManPage)))
                {
                    var page = Activator.CreateInstance(type) as StaticManPage;
                    Pages.Add(page);
                }
            }
        }       
    }
}
