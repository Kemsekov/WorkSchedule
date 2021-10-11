using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kemsekov
{
    public class WorkSchedule
    {
        List<Action[]> todoList = new List<Action[]>();
        int Depth { get; }
        int step_count = 0;
        int StepCount => step_count;

        public WorkSchedule(int depth)
        {
            Depth = depth;
        }
        /// <summary>
        /// Step through the list of works and in each of them calls work with StepCount index. Then increment StepCount.
        /// </summary>
        public void Step()
        {
            if (step_count >= Depth)
                return;
            foreach (var todo in todoList)
            {
                todo[step_count]?.Invoke();
            }
            step_count++;

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
            if (step_count >= Depth)
                return;
            _StepParallel();
        }
        public async Task StepParallelAsync()
        {
            if (step_count >= Depth)
                return;
            _StepParallel();
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
            step_count = 0;
        }
        /// <summary>
        /// Clears all todo's so there is no work to do
        /// </summary>
        public void Clear()
        {
            Reset();
            todoList.Clear();
        }
        /// <summary>
        /// Adds new work times.
        /// </summary>
        /// <param name="todo"></param>
        public void Add(params Action[] todo)
        {
            if (todo.Length == Depth)
                todoList.Add(todo);
            else
            {
                throw new ArgumentException("todo.Length != MaxStepCount. This mean that you added an array of functions bigger than it must be.");
            }
        }
    }
}
