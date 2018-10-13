using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
	public interface ClientCommandHandler
	{
		void HandleCommand(PendingCommand Command);
	}
}
