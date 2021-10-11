using System;
using System.Collections.Generic;
using System.Linq;
using Kemsekov;
using Xunit;

namespace Tests
{
    public class WorkScheduleTest
    {

        [Fact]
        public void Add_ThrowIfWrongActionsCount()
        {
            WorkSchedule s = new(3);
            Assert.Throws<ArgumentException>(() =>
            {
                s.Add(
                    () => { },
                    () => { }
                );
            });

            Assert.Throws<ArgumentException>(() =>
            {
                s.Add(new Action[]{
                ()=>{},
                ()=>{},
                ()=>{},
                ()=>{}
            });
            });
            Assert.Throws<ArgumentException>(() =>
            {
                s.Add(new Action[]{
                ()=>{}
            });
            });
        }
        [Fact]
        public void Step_CheckIfStepsOutOfBounds()
        {
            WorkSchedule s = new(2);
            string status = null;
            s.Add(
                () => status = "first ",
                () => status += "second"
            );
            for (int i = 0; i < 20; i++)
                s.Step();
            Assert.Equal(status, "first second");
        }
        [Fact]
        public void Step_DifferentOverrides()
        {
            WorkSchedule s = new(4);

            var rand = new Random();
            string value = "1234567890";

            List<string> results = new();
            for (int i = 0; i < 40; i++)
            {
                string copy = new string(value);
                s.Add(
                    () => { copy = copy.Replace((char)(i % 10 + 60), (char)i); },
                    () => { copy = copy.Replace((char)(i % 10 + 61), (char)i); },
                    () => { copy = copy.Replace((char)(i % 10 + 62), (char)i); },
                    () =>
                    {
                        lock (results)
                            results.Add(copy);
                    }
                );
            }
            List<IEnumerable<string>> total_results = new ();
            for (int i = 0; i < 10; i++)
            {
                results.Clear();

                for(int b = 0;b<4;b++)
                switch (rand.Next() % 4)
                {
                    case 0:
                        s.Step();
                        break;
                    case 1:
                        s.StepParallel();
                        break;
                    case 2:
                        s.StepAsync().Wait();
                        break;
                    case 3:
                        s.StepParallelAsync().Wait();
                        break;
                }
                results.Sort((v1,v2)=>{
                    int comp = 0;
                    foreach(var t in v1.Zip(v2)){
                        comp+=t.First-t.Second;
                    }
                    return comp;
                });
                total_results.Add(results);
            }

            total_results.Aggregate((v1,v2)=>{
                Assert.Equal(v1,v2);
                return v2;
            });

        }
        
    }
}
