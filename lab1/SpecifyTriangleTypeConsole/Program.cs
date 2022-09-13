// See https://aka.ms/new-console-template for more information
using SpecifyTriangleTypeConsole;
using SpecifyTriangleTypeConsole.Exceptions;
using SideLength = System.Double;

Triangle GetTriangleFromArgs(string[] args)
{
				if (args.Length < Triangle.SideNumber)
				{
								throw new NotEnoughConsoleArgsException();
				}

				SideLength[] sides = new SideLength[Triangle.SideNumber];
				for (int i = 0; i < Triangle.SideNumber; i++)
				{
								SideLength tempSide;
								bool wasSucess = SideLength.TryParse(args[i], out tempSide);
								if (!wasSucess || tempSide <= 0)
								{
												throw new WrongSideValueException();
								}

								sides[i] = tempSide;
				}

				return new Triangle(sides[0], sides[1], sides[2]);
}
try
{
				Triangle triangle = GetTriangleFromArgs(args);
				Console.WriteLine(triangle.TypeToString());
				return 0;
}
catch (Exception e)
{
				Console.WriteLine("Internal error");
				return 1;
}

