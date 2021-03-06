using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemsekov
{
    /// <summary>
    /// A tool that allows you to divide the logic of a function into 
    /// parts and subsequently control their execution independently of each other.
    /// </summary>
    public class WorkSchedule
    {
        List<Action[]> todoList = new List<Action[]>();

        /// <summary>
        /// Maximum allowed number of steps to run.
        /// </summary>
        /// <value></value>
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
        public void Step(){
            Step(step_count++);
        }
        public void Step(int index)
        {
            if (index >= Depth)
                return;
            foreach (var todo in todoList)
            {
                todo[index]?.Invoke();
            }
        }
        /// <summary>
        /// Runs <see cref="Step()"/> in new Task and return it.
        /// </summary>
        public Task StepAsync(){
            return StepAsync(step_count++);
        }
        public Task StepAsync(int index)
        {
            return Task.Run(()=>Step(index));
        }
        /// <summary>
        /// Runs todo List in parallel
        /// </summary>
        public void StepParallel(){
            StepParallel(step_count++);
        }
        
        public void StepParallel(int index)
        {
            if (index >= Depth)
                return;
            _StepParallel(index);
        }

        private void _StepParallel(int index)
        {
            Parallel.ForEach(todoList, (value, _) =>
            {
                value[index]?.Invoke();
            });
        }

        /// <summary>
        /// Reset steps to 0
        /// </summary>
        public void Reset()
        {
            step_count = 0;
        }
        /// <summary>
        /// Clears todo List
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
