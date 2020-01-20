using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4
{
    class Program
    {
		static Int32 Factorial(int n)
		{
			if (n < 0)
				return 0;
			if (n == 0)
				return 1;
			if (n == 1 || n == 2)
				return n;

			bool[] u = new bool[n + 1]; // маркеры для решета Эратосфена
			List<Tuple<int, int>> p = new List<Tuple<int, int>>(); // множители и их показатели степеней
			for (int i = 2; i <= n; ++i)
				if (!u[i]) // если i - очередное простое число
				{
					// считаем показатель степени в разложении
					int k = n / i;
					int c = 0;
					while (k > 0)
					{
						c += k;
						k /= i;
					}
					// запоминаем множитель и его показатель степени
					p.Add(new Tuple<int, int>(i, c));
					// просеиваем составные числа через решето               
					int j = 2;
					while (i * j <= n)
					{
						u[i * j] = true;
						++j;
					}
				}
            // вычисляем факториал
			Int32 r = 1;
			for (int i = p.Count() - 1; i >= 0; --i)
				r *= (Int32)Math.Pow(p[i].Item1, p[i].Item2);
			return r;
		}

		static void ReadInput(out Int32 thingiesN, out Int32 boxesM)
		{
			while (true)
			{
				Console.WriteLine("Input number of round thingies to get (N)");
				var line = Console.ReadLine();
				var result = Int32.TryParse(line, out thingiesN);
				if (!result)
				{
					Console.Clear();
					Console.WriteLine("Wrong input");
					continue;
				}

				if (thingiesN < 0)
				{
					Console.Clear();
					Console.WriteLine("Amount of thingies must be >= 0");
					continue;
				}

                Console.WriteLine("Input number of boxes (M)");
				line = Console.ReadLine();
				result = Int32.TryParse(line, out boxesM);
				if (!result)
				{
					Console.Clear();
					Console.WriteLine("Wrong input");
					continue;
				}

				if (boxesM <= 0)
				{
					Console.Clear();
					Console.WriteLine("Amount of boxes must be > 0");
					continue;
				}

                if (thingiesN > boxesM * 2)
				{
					Console.Clear();
					Console.WriteLine("Amount of thingies must be less or equal to amount of boxes * 2");
					continue;
				}

				break;
			}

			Console.Clear();
		}


        static void Main(string[] args)
		{
			const Double probabilityOneThingy = 0.4;
			const Double probabilityTwoThingies = 0.1;
			const Double probabilityEmpty = 0.5;


			Int32 thingiesN, boxesM;
			ReadInput(out thingiesN, out boxesM);


			Int32 twoThingiesAmount = 0;
			Int32 oneThingyAmount = thingiesN;

            if (boxesM - thingiesN < 0)
			{
				twoThingiesAmount = thingiesN - boxesM;
				oneThingyAmount = boxesM - twoThingiesAmount;
			}

			Int32 emptyAmount = boxesM - oneThingyAmount - twoThingiesAmount;

            Int32 factorialM = Factorial(boxesM);

			Int32 factorialOneThingy = Factorial(oneThingyAmount);
			Int32 factorialTwoThingies = Factorial(twoThingiesAmount);
            Int32 factorialEmpty = Factorial(emptyAmount);

            Double totalProbabilityOneThingy = Math.Pow(probabilityOneThingy, oneThingyAmount);
			Double totalProbabilityTwoThingies = Math.Pow(probabilityTwoThingies, twoThingiesAmount);
            Double totalProbabilityEmpty = Math.Pow(probabilityEmpty, emptyAmount);

			Double overallProbability = 0;

			for (var i = 0; i <= oneThingyAmount / 2;)
			{
				Double currentProbability =
					totalProbabilityOneThingy * totalProbabilityTwoThingies * totalProbabilityEmpty;

				Int32 numberOfDistinguishablePermutations =
					factorialM / (factorialOneThingy * factorialTwoThingies * factorialEmpty);

				overallProbability += currentProbability * numberOfDistinguishablePermutations;

                totalProbabilityOneThingy /= probabilityOneThingy * probabilityOneThingy;
				totalProbabilityTwoThingies *= probabilityTwoThingies;
				totalProbabilityEmpty *= probabilityEmpty;

				factorialOneThingy /= oneThingyAmount - i * 2 == 0 ? 1 : oneThingyAmount - i * 2;
				factorialOneThingy /= oneThingyAmount - i * 2 - 1 == 0 ? 1 : oneThingyAmount - i * 2 - 1;
				i++;
                factorialTwoThingies *= twoThingiesAmount + i;
				factorialEmpty *= emptyAmount + i;
            }

            Console.WriteLine($"Probability to get {thingiesN} round thingies from {boxesM} boxes is {overallProbability}");
            Console.ReadLine();
		}
    }
}
