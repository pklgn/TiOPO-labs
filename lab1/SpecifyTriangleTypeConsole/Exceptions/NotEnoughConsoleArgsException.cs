using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecifyTriangleTypeConsole.Exceptions;

public class NotEnoughConsoleArgsException : System.Exception
{
				public NotEnoughConsoleArgsException() : base("Not enough arguments for a triangle")
				{
				}
}
