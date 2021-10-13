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
            status = null;
            s.Reset();
            for (int i = 0; i < 20; i++)
                s.Step(i);
            Assert.Equal(status, "first second");
        }
        [Fact]
        public void Step_DifferentOverrides()
        {
            WorkSchedule s = new(4);

            var rand = new Random();
            List<int> items = new();

            for (int i = 0; i < 200; i++)
            {
                int copy = i;
                s.Add(
                    () => { items.Add(copy); },
                    () => { items.Add(copy + 1); },
                    //because it runs in parallel we need to lock
                    () => { 
                        lock(items)
                        items.Add(copy + 2);
                    },
                    //and here
                    () => { 
                        lock(items)
                        items.Add(copy + 3);
                    }
                );
            }
            for (int i = 0; i < 200; i++)
            {
                s.Step();
                Assert.Equal(items.Count, 200);
                Assert.Equal(items.GetRange(0, 200), Enumerable.Range(0, 200));
                items.Clear();

                s.StepAsync().Wait();
                Assert.Equal(items.Count, 200);
                Assert.Equal(items.GetRange(0, 200), Enumerable.Range(1, 200));
                items.Clear();

                s.StepParallel();
                Assert.Equal(items.Count, 200);
                items.Sort();
                Assert.Equal(items.GetRange(0, 200), Enumerable.Range(2, 200));
                items.Clear();

                s.StepParallel();
                Assert.Equal(items.Count, 200);
                items.Sort();
                Assert.Equal(items.GetRange(0, 200), Enumerable.Range(3, 200));
                items.Clear();
                s.Reset();
            }
        }
    }
}
