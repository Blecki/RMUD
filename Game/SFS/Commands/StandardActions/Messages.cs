using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS;

namespace SFS.Commands.StandardActions
{
    public class ClothingMessages 
    {
        [AtStartup]
        public static void __()
        {
            Core.StandardMessage("clothing nude", "You are naked.");
            Core.StandardMessage("clothing wearing", "You are wearing..");
            Core.StandardMessage("clothing remove first", "You'll have to remove <the0> first.");
            Core.StandardMessage("clothing they are nude", "^<the0> is naked.");
            Core.StandardMessage("clothing they are wearing", "^<the0> is wearing <l1>.");
            Core.StandardMessage("clothing remove what", "I couldn't figure out what you're trying to remove.");
            Core.StandardMessage("clothing not wearing", "You'd have to be actually wearing that first.");
            Core.StandardMessage("clothing you remove", "You remove <the0>.");
            Core.StandardMessage("clothing they remove", "^<the0> removes <a1>.");
            Core.StandardMessage("clothing wear what", "I couldn't figure out what you're trying to wear.");
            Core.StandardMessage("clothing already wearing", "You're already wearing that.");
            Core.StandardMessage("clothing you wear", "You wear <the0>.");
            Core.StandardMessage("clothing they wear", "^<the0> wears <a1>.");
            Core.StandardMessage("clothing cant wear", "That isn't something that can be worn.");
            Core.StandardMessage("not here", "I don't see that here.");
            Core.StandardMessage("gone", "The doesn't seem to be here any more.");
            Core.StandardMessage("dont have that", "You don't have that.");
            Core.StandardMessage("already have that", "You already have that.");
            Core.StandardMessage("does nothing", "That doesn't seem to do anything.");
            Core.StandardMessage("nothing happens", "Nothing happens.");
            Core.StandardMessage("unappreciated", "I don't think <the0> would appreciate that.");
            Core.StandardMessage("already open", "It is already open.");
            Core.StandardMessage("already closed", "It is already closed.");
            Core.StandardMessage("close it first", "You'll have to close it first.");
            Core.StandardMessage("wrong key", "That is not the right key.");
            Core.StandardMessage("error locked", "It seems to be locked.");
        }
    }
}
