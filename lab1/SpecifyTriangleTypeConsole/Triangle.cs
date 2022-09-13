using SpecifyTriangleTypeConsole.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecifyTriangleTypeConsole;
using SideLength = System.Double;

public class Triangle
{
				public Triangle(SideLength a, SideLength b, SideLength c)
				{
								aSide = a;
								bSide = b;
								cSide = c;
								IdentifyType();
				}
				public enum Type { Common, Isosceles, Equilateral, NoTriangle };

				public const int SideNumber = 3;

				private SideLength aSide;
				private SideLength bSide;
				private SideLength cSide;
				private Type type;

				public SideLength GetASide() => aSide;
				public SideLength GetBSide() => bSide;
				public SideLength GetСSide() => cSide;
				public new Type GetType() => type;

				public string TypeToString()
				{
								switch (type)
								{
												case Type.Common:
																return "Common";
												case Type.Isosceles:
																return "Isosceles";
												case Type.Equilateral:
																return "Equilateral";
												case Type.NoTriangle:
																return "No triangle";
												default:
																return "Internal error";
								}
				}

				private void IdentifyType()
				{
								if ((aSide + bSide > cSide) && (aSide + cSide > bSide) && (bSide + cSide > aSide))
								{
												if (aSide == bSide && aSide == cSide)
												{
																type = Type.Equilateral;
												}
												else if (aSide == bSide || aSide == cSide || bSide == cSide)
												{
																type = Type.Isosceles;
												}
												else
												{
																type = Type.Common;
												}
								}
								else
								{
												type = Type.NoTriangle;
								}
				}
}
