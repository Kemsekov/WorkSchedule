using System.Threading;
using System.Threading.Tasks;
using System;

namespace WorkSchedules
{
    public class Program
    {

        static void Zero(int n)
        {
            System.Console.WriteLine($"N : {n} zero");
        }
        static void One(int n){
            System.Console.WriteLine($"N : {n} one");
        }
        static void Two(int n){
            System.Console.WriteLine($"N : {n} two");
        }

        public static async Task TT()
        {
            WorkSchedule s = new(3);
            s.Add(new Action[]{
                ()=>Zero(1), //step 1
                ()=>One(1),  //step 2
                ()=>Two(1)});//step 3
            
            s.Add(new Action[]{
                ()=>Zero(2), //step 1
                ()=>One(2),  //step 2
                ()=>Two(2)});//step 3

            s.Add(new Action[]{
                ()=>Zero(3), //step 1
                ()=>One(3),  //step 2
                ()=>Two(3)});//step 3

            for (int i = 0; i < 10; i++)
            {
                s.StepEachInNewTask(); //zero 1 ; zero 2; zero 3
                System.Console.WriteLine();

                s.StepEachInNewTask(); //one 1; one 2; one 3;
                System.Console.WriteLine();

                s.StepEachInNewTask(); //two 1; two 2; two 3
                System.Console.WriteLine();

                s.Reset();
            }

            //N : 1 zero
            //N : 2 zero
            //N : 3 zero

            //N : 1 one
            //N : 2 one
            //N : 3 one

            //N : 1 two
            //N : 2 two
            //N : 3 two

        }

    }
    public class Node : NodeBase
    {
        public Node(int i) : base(i)
        {
        }
        public void Do()
        {
            System.Console.WriteLine($"Hello {num}");
        }
    }
    public class NodeBase
    {
        public int num;
        public NodeBase(int i)
        {
            num = i;
        }
    }

}