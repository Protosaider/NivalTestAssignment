using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task2
{
    class Program
    {
		static void ReadInput(out Int32 headsN, out Int32 tailsM, out String yesNo)
		{
			Regex yesNoRegex = new Regex(@"^Y|N\s*$");

			while (true)
			{
				Console.WriteLine("Input number of heads (N)");
				var line = Console.ReadLine();
				var result = Int32.TryParse(line, out headsN);
				if (!result)
				{
					Console.Clear();
                    Console.WriteLine("Wrong input");
					continue;
				}

				Console.WriteLine("Input number of tails (M)");
				line = Console.ReadLine();
				result = Int32.TryParse(line, out tailsM);
				if (!result)
				{
					Console.Clear();
					Console.WriteLine("Wrong input");
					continue;
				}

				if (headsN <= 0)
				{
					Console.Clear();
					Console.WriteLine("Amount of heads must be > 0");
					continue;
				}

				if (headsN == 1 && tailsM <= 0)
				{
					Console.Clear();
					Console.WriteLine("Amount of tails must be > 0, because amount of heads = 1");
					continue;
				}

				if (tailsM < 0)
				{
					Console.Clear();
					Console.WriteLine("Amount of tails must be >= 0");
					continue;
				}

				Console.WriteLine("Is the sword flawed? (Y/N)");
				line = Console.ReadLine();
				var matchValue = yesNoRegex.Match(line).Value;
				if (String.IsNullOrEmpty(matchValue))
				{
					Console.Clear();
					Console.WriteLine("Wrong input");
					continue;
				}

				yesNo = matchValue;
                break;
			}

			Console.Clear();
        }

		static Int32 Calculate(Int32 nHeads, Int32 mTails, Regex yNRegex, String yN)
        {
            Int32 headsN = nHeads, tailsM = mTails;
            Regex yesNoRegex = new Regex(@"^Y|N\s*$");
            String yesNo = yN;

			Int32 amountOfSteps;

			if (String.Equals(yesNo, "N"))
            {
                var additionalTailChops = 4 - (headsN * 2 + tailsM) % 4;
                //additionalTailChops = additionalTailChops == 4 ? 0 : additionalTailChops;
                additionalTailChops = additionalTailChops % 4;

                var tailChops = (tailsM + additionalTailChops) / 2;

                var headChops = (headsN + tailChops) / 2;

				amountOfSteps = headChops + tailChops + additionalTailChops;

                Console.WriteLine("Amount of steps: " + amountOfSteps);

                Console.WriteLine("Generate steps? (Y/N)");
                var line = Console.ReadLine();

                var matchValue = yesNoRegex.Match(line).Value;

                if (!String.IsNullOrEmpty(matchValue) && String.Equals(matchValue, "Y"))
                {
                    Console.WriteLine("Cut\tHeads\tTails");
                    Console.WriteLine($"_\t{headsN}\t{tailsM}");

                    var N = headsN;
                    var M = tailsM;

                    for (var i = 0; i < additionalTailChops; i++)
                    {
                        M++;
                        Console.WriteLine($"T\t{N}\t{M}");
                    }

                    while (M > 0)
                    {
                        M -= 2;
                        N++;
                        Console.WriteLine($"T\t{N}\t{M}");
                    }

                    while (N > 0)
                    {
                        N -= 2;
                        Console.WriteLine($"H\t{N}\t{M}");
                    }
                }
            }
            else
            {
                void CalculateSteps(Int32 n, Int32 m, ref List<Tuple<Int32, Int32>> nextSteps)
                {
                    nextSteps.Clear();

                    if (n <= 8 && m <= 100)
                    {
                        if (n > 0)
                            nextSteps.Add(new Tuple<Int32, Int32>(n, RoundTail(m + 3)));
                        if (m >= 10)
                            nextSteps.Add(new Tuple<Int32, Int32>(n, RoundTail(m + 10 + 3)));
                        if (n >= 2)
                            nextSteps.Add(new Tuple<Int32, Int32>(n - 2, RoundTail(m + 3)));
                        if (m >= 20)
                            nextSteps.Add(new Tuple<Int32, Int32>(n + 1, RoundTail(m - 20 + 3)));
                    }


                    //               if (n <= 4 && m <= 90)
                    //{
                    //	if (n > 0)
                    //		nextSteps.Add(new Tuple<Int32, Int32>(n, RoundTail(m + 3)));
                    //	if (m >= 10)
                    //		nextSteps.Add(new Tuple<Int32, Int32>(n, RoundTail(m + 10 + 3)));
                    //}

                    //if (n >= 2 && n <= 6)
                    //	nextSteps.Add(new Tuple<Int32, Int32>(n - 2, RoundTail(m + 3)));
                    //if (m >= 20 && m <= 110)
                    //	nextSteps.Add(new Tuple<Int32, Int32>(n + 1, RoundTail(m - 20 + 3)));
                }

                Int32 RoundTail(Int32 tail)
                {
                    switch (tail % 10)
                    {
                        case 7:
                            return tail - 1;
                        case 9:
                            return tail + 1;
                        default:
                            return tail;
                    }
                }

                Int32 heads = headsN;
                Int32 tails = tailsM * 10;

                amountOfSteps = 0;

                //traverse down - 6, 100 - magic numbers
                var straightDownPath = new Stack<Tuple<Int32, Int32>>();
                straightDownPath.Push(new Tuple<Int32, Int32>(heads, tails));

                while (heads > 6 || tails >= 100)
                {
                    while (heads > 6)
                    {
                        heads -= 2;
                        tails = RoundTail(tails + 3);
                        amountOfSteps++;
                        straightDownPath.Push(new Tuple<Int32, Int32>(heads, tails));
                    }

                    while (tails >= 100)
                    {
                        heads++;
                        tails = RoundTail(tails - 20 + 3);
                        amountOfSteps++;
                        straightDownPath.Push(new Tuple<Int32, Int32>(heads, tails));
                    }
                }


                var nodes = new Dictionary<Tuple<Int32, Int32>, Node>();
                var stepsToCheck = new Queue<Tuple<Int32, Int32>>();
                var possibleSteps = new List<Tuple<Int32, Int32>>(4);

                var endNodes = new Dictionary<Tuple<Int32, Int32>, Node>(2);

                stepsToCheck.Enqueue(new Tuple<Int32, Int32>(heads, tails));
                nodes[stepsToCheck.Peek()] = new Node(stepsToCheck.Peek(), amountOfSteps);

                Node currentNode = null;

                while (true)
                {
                    if (stepsToCheck.Count == 0)
                        break;

                    var currentStep = stepsToCheck.Dequeue();
                    currentNode = nodes[currentStep];

                    CalculateSteps(currentStep.Item1, currentStep.Item2, ref possibleSteps);

                    foreach (var step in possibleSteps)
                    {
                        Node node;

                        if (nodes.TryGetValue(step, out node))
                        {
                            if (currentNode.StepsCount + 1 < node.StepsCount)
                            {
                                node.Previous = currentNode;
                                node.StepsCount = currentNode.StepsCount + 1;
                            }
                        }
                        else
                        {
                            if (step.Item1 == 0 && step.Item2 <= 6)
                            {
                                if (!endNodes.TryGetValue(step, out node))
                                {
                                    endNodes[step] = new Node(currentNode, step, currentNode.StepsCount + 1);
                                    nodes[step] = endNodes[step];
                                }
                            }
                            else
                            {
                                stepsToCheck.Enqueue(step);
                                nodes[step] = new Node(currentNode, step, currentNode.StepsCount + 1);
                            }
                        }
                    }
                }

                amountOfSteps = Int32.MaxValue;
                foreach (var endNode in endNodes)
                {
                    if (endNode.Value.StepsCount < amountOfSteps)
                    {
                        amountOfSteps = endNode.Value.StepsCount;
                        currentNode = endNode.Value;
                    }
                }

                var path = new Stack<Tuple<Int32, Int32>>(amountOfSteps + 1);
                path.Push(currentNode.Step);
                while (currentNode.Previous != null)
                {
                    currentNode = currentNode.Previous;
                    path.Push(currentNode.Step);
                }

                straightDownPath.Pop();
                for (var i = straightDownPath.Count; i > 0; i--)
                {
                    var tuple = straightDownPath.Pop();
                    path.Push(tuple);
                }

                Console.WriteLine($"Total steps count: {amountOfSteps}");
                Console.ReadLine();
                Console.WriteLine("Path:");

                amountOfSteps = 0;
                while (path.Count > 0)
                {
                    var (head, tail) = path.Pop();
                    Console.WriteLine($"{head}\t{(tail - tail % 10) / 10}\t{amountOfSteps}");
                    amountOfSteps++;
                }

            }

			return amountOfSteps;
		}


		static void TestFlawedSword()
		{
			Regex yesNoRegex = new Regex(@"^Y|N\s*$");
			String yes = "Y";

			Int32 N = 13;
			Int32 M = 13;

			var array = new Int32[N, M];
			String headerLine = "\t";

            for (var i = 0; i < N; i++)
				for (var j = 0; j < M; j++)
				{
					if (i == 0 && j == 0)
						array[i, j] = -1;
					else
						array[i, j] = Calculate(i, j, yesNoRegex, yes);

					if (i == 0)
                        headerLine += $"{j}\t";
                }

			Console.WriteLine(headerLine);
			for (var i = 0; i < N; i++)
			{
				headerLine = $"{i}\t";
				for (var j = 0; j < M; j++)
					headerLine += $"{array[i, j]}\t";
				Console.WriteLine(headerLine);
			}

        }


        static void Main(string[] args)
		{
			Int32 headsN, tailsM;
			Regex yesNoRegex = new Regex(@"^Y|N\s*$");
            String yesNo;

            ReadInput(out headsN, out tailsM, out yesNo);

            Calculate(headsN, tailsM, yesNoRegex, yesNo);

            //TestFlawedSword();

            Console.ReadLine();
		}


		class Node
		{
			public Node Previous;
			public Tuple<Int32, Int32> Step;
            public Int32 StepsCount;

			public Node() { }
			public Node(Tuple<Int32, Int32> step, Int32 stepsCount)
			{
				Step = step;
				StepsCount = stepsCount;
			}
			public Node(Node previous, Tuple<Int32, Int32> step, Int32 stepsCount) : this(step, stepsCount) => Previous = previous;
		}
    }
}

//Int32 N = headsN;
//Int32 M = tailsM;
//Int32 I = 0; //additional tail chops
//Int32 J = 0; //additional head chops

//Double AllChops(Int32 m, Int32 n, Int32 i, Int32 j) =>
//(12.0 * n + 18.0 * m + 42.0 * i + 24.0 * j) / 19.0;

//List<Tuple<Int32, Tuple<Int32, Int32>>> potentialMinimum = new List<Tuple<Int32, Tuple<Int32, Int32>>>();

////for (var iterationIndex = 0; iterationIndex < 100; iterationIndex++)
//while (potentialMinimum.Count == 0)
////for (var iterationIndex = 0; potentialMinimum.Count == 0; iterationIndex++)
//{
//for (var i = 0; i <= I; i++)
//{
//	if (Math.Abs(AllChops(M, N, i, J) % 1) <= Double.Epsilon* 100)
//		potentialMinimum.Add(

//			new Tuple<Int32, Tuple<Int32, Int32>>(i + J, new Tuple<Int32, Int32>(i, J))
//		);
//}

//for (var j = 0; j<J; j++)
//{
//	if (Math.Abs(AllChops(M, N, I, j) % 1) <= Double.Epsilon* 100)
//		potentialMinimum.Add(

//			new Tuple<Int32, Tuple<Int32, Int32>>(I + j, new Tuple<Int32, Int32>(I, j))
//		);
//}

//J++;
//I++;
//}

//potentialMinimum.Sort((i, j) => i.Item1.CompareTo(j.Item1));

//Console.WriteLine($"{potentialMinimum[0].Item1} {potentialMinimum[0].Item2.Item1} {potentialMinimum[0].Item2.Item2}");