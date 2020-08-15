using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPlusPlusCheckr
{
	class Program
	{
		static void Main(string[] args)
		{
			FileReader.Ins().LogError("lalla");
			FileReader.Ins().LogWarnning("ccccc");
			FileReader.Ins().LogMsg("aaaaa");

			Console.ReadLine();
		}
	}
}
