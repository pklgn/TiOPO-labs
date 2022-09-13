// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text.RegularExpressions;

bool CheckTriangle(string testLine, string expectedResult)
{
				Process process = new Process();
				process.StartInfo.FileName = "..\\..\\..\\..\\SpecifyTriangleTypeConsole\\bin\\Debug\\net6.0\\SpecifyTriangleTypeConsole.exe";
				process.StartInfo.Arguments = testLine;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();

				string? result = process.StandardOutput.ReadLine();

				process.WaitForExit();

				return (expectedResult == result);
}

void RunTests()
{
				using StreamWriter output = new("OUTPUT.TXT");
				string? testLine;
				while ((testLine = Console.ReadLine()) != null)
				{
								string[] testLineSplittedBySpace = testLine.Split(" ");

								string[] argsString = new string[3];
								for (int i = 0; i < 3; ++i)
								{
												argsString[i] = testLineSplittedBySpace[i];
								}
								string args = string.Join(" ", argsString);
								string expectedResult = testLineSplittedBySpace.Last();
								bool wasSuccess = CheckTriangle(args, expectedResult);
								string result = (wasSuccess) ? "success" : "error";
								output.WriteLine(result);
				}
}

RunTests();