using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkSchedules
{
    public class WorkSchedule
    {
        List<Action[]> todoList = new();
        SemaphoreSlim block = new SemaphoreSlim(1);
        int Depth { get; }
        int step_count = 0;
        int StepCount
        {
            get => step_count;
            set
            {
                block.Wait();
                step_count = value < Depth ? value : step_count;
                block.Release();
            }
        }

        public WorkSchedule(int depth)
        {
            Depth = depth;
        }
        /// <summary>
        /// Step through the list of works and in each of them calls work with StepCount index. Then increment StepCount.
        /// </summary>
        public void Step()
        {
            block.Wait();
            if (step_count >= Depth)
            {
                block.Release();
                return;
            }
            foreach (var todo in todoList)
            {
                todo[step_count]?.Invoke();
            }
            step_count++;
            block.Release();

        }
        /// <summary>
        /// Runs Step() in new Task and return it.
        /// </summary>
        public Task StepAsync()
        {
            return Task.Run(Step);
        }
        /// <summary>
        /// Runs todo work in parallel.
        /// </summary>
        public void StepParallel()
        {
            block.Wait();
            if (step_count >= Depth)
            {
                block.Release();
                return;
            }
            _StepParallel();
            block.Release();
        }
        public async Task StepParallelAsync()
        {
            await block.WaitAsync();
            if (step_count >= Depth)
            {
                block.Release();
                return;
            }
            _StepParallel();
            block.Release();
        }

        private void _StepParallel()
        {
            Parallel.For(0, todoList.Count, (index, _) =>
            {
                todoList[index][step_count]?.Invoke();
            });
        }


        /// <summary>
        /// Reset all steps to 0
        /// </summary>
        public void Reset()
        {
            StepCount = 0;
        }
        /// <summary>
        /// Clears all todo's so there is no work to do
        /// </summary>
        public void Clear()
        {
            block.Wait();
            Reset();
            todoList.Clear();
            block.Release();
        }
        /// <summary>
        /// Adds new work times.
        /// </summary>
        /// <param name="todo"></param>
        public void Add(params Action[] todo)
        {
            block.Wait();
            if (todo.Length == Depth)
                todoList.Add(todo);
            else
            {
                block.Release();
                throw new ArgumentException("todo.Length != MaxStepCount. This mean that you added an array of functions bigger than it must be.");
            }
            block.Release();
        }
    }
}