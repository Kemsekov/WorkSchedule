using System;
using WorkSchedules;
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
        }
        [Fact]
        public void Step_CheckIfOutOfBounds()
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
            WorkSchedule s = new(3);
            string step1 = null;
            string step2 = null;
            Action check = () =>
            {
                Assert.Equal(step1, "step1 and med");
                Assert.Equal(step2, "step2 and med");
                step1 = null;
                step2 = null;
            };
            s.Add(
                () => step1 = "step1",
                () => step1 += " and",
                () => step1 += " med");
            s.Add(
                () => step2 = "step2",
                () => step2 += " and",
                () => step2 += " med");

            for (int i = 0; i < 3; i++)
            {
                s.Step();
            }
            check();

            s.Reset();

            s.StepAsync().Wait();
            s.StepEachInNewTask();
            s.Step();

            check();
        }
        
    }
}
