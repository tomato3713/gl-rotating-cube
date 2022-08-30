using System;

namespace CSharpOpentk
{
	class Program
	{
		static void Main(string[] args)
		{
			using (Cube cube = new Cube(800, 600, "Cube Spin by OpenTK with C#"))
			{
				cube.Run();
			}
		}
	}
}
