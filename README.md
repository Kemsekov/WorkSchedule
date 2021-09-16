# WorkSchedule
A little C# tool that allow to schedule and control execution of code where function is work quanta
# Problem
Imagine if you need to create some recursive algorithm that must work multithreaded
```cs
//this method executes in many threads simultaneously.
void ProcessSomething(ToProcess value){
    value.Do1();
    //Wait other threads to reach this point
    value.Do2();
    //Wait again
    value.Do3();
    //Wait
    ProcessSomething(value);
}
```
You may try to solve this by using 'AutoResetEvent` or `Barrier` or whatever, but in the end you
need to manage a whole thing by yourself and sometimes this becames a really hard task.

But you can use `WorkSchedule`!

```cs
using WorkSchedules;
using System.Linq;

WorkSchedule workSchedule = new(3);

void Process(int id,string value){
    workSchedule.Add(
        ()=>
        {
            var shifted_array = value.Select(c=>(char)(c+1)).ToArray();
            value = new string(shifted_array);
            System.Console.WriteLine($"Zero : {id} {value}");
        },
        ()=>
        {
            var shifted_array = value.Select(c=>(char)(c+2)).ToArray();
            value = new string(shifted_array);
            System.Console.WriteLine($"One : {id} {value}");
        },
        ()=>
        {
            var shifted_array = value.Select(c=>(char)(c-3)).ToArray();
            value = new string(shifted_array);
            System.Console.WriteLine($"Two : {id} {value}");
        }
    );
}

Process(0,"Zero");
Process(1,"One");
Process(2,"Two");
Process(3,"Three");

workSchedule.Step();
System.Console.WriteLine();

workSchedule.StepEachInNewTask();
System.Console.WriteLine();

workSchedule.StepEachInNewTask();
System.Console.WriteLine();

workSchedule.Clear();

/*
Possible output

    Zero : 0 [fsp
    Zero : 1 Pof
    Zero : 2 Uxp
    Zero : 3 Uisff

    One : 1 Rqh
    One : 3 Wkuhh
    One : 2 Wzr
    One : 0 ]hur

    Two : 0 Zero
    Two : 3 Three
    Two : 1 One
    Two : 2 Two

*/
```

