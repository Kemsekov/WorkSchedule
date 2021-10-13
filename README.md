# WorkSchedule
[![nuget](https://img.shields.io/nuget/v/Kemsekov.WorkSchedule.svg)](https://www.nuget.org/packages/Kemsekov.WorkSchedule/) 

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
You may try to solve this by using `AutoResetEvent` or `Barrier` or whatever, but in the end you
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

workSchedule.StepParallel();
System.Console.WriteLine();

workSchedule.StepParallel();
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
Here we do some clever(not really) stuff.
We create some kind of local namespace in function `Process`. There we implement function as series of lambdas that have access
to this local namespace. This approach is very convenient, but have some disadvantages.
Because of how `GC` works in such approach we can easily run out of memory because lambdas take a reference to all local namespace. 
So it is a good practice to call `Clear` method when you done with `WorkSchedule`. This will force `GC` to remove lambdas and
everything must be fine.

# Be aware of `StepParallel()` 

Be aware of `WorkSchedule.StepParallel()` execution. If inside of your `Action` you try to add some value to non concurrency-safe collection it can leads  to undefined behaviour.

```cs
//create WorkSchedule with two steps
var w = new WorkSchedule(2);
var items = new List<int>();
for(int i = 0;i<200;i++)
    w.Add(
        //without lock
        ()=>{
            items.Add(1);
        },
        //with lock
        ()=>{
            lock(items);
            items.Add(1);
        }
    );

```

If we execute part without lock by calling `WorkSchedule.StepParallel()` we can expect that `items.Count` is not equal to 200.

```cs
//execute 200 functions without lock
w.StepParallel(0);
Console.Writeline(items.Count);
```
Output
```
137
```

This is because of `List<int>` concurrency `Add` leads to undefined behaviour, so use lock.

```cs
//execute 200 functions with lock
w.StepParallel(1);
Console.Writeline(items.Count);
```

Output
```
200
```








