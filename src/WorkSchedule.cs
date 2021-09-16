using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkSchedules
{
    public class WorkSchedule
    {
        List<Action[]> todoList = new();

        int Depth { get; }
        public WorkSchedule(int depth)
        {

            Depth = depth;
        }

        int step_count = 0;
        public void Step()
        {
            if(step_count>=Depth) return;
            foreach (var todo in todoList)
            {
                todo[step_count]?.Invoke();
            }
            step_count++;
        }
        /// <summary>
        /// Runs Step() in new Task
        /// </summary>
        public async Task StepAsync()
        {
            await Task.Run(Step);
        }
        /// <summary>
        /// Runs each todo in new Task
        /// </summary>
        public void StepParallel(){
            if(step_count>=Depth) return;
            Parallel.For(0,todoList.Count,(index,_)=>{
                todoList[index][step_count]?.Invoke();
            });
            step_count++;
        }
        public async Task StepParallelAsync(){
            await Task.Run(StepParallel);
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
        public void Clear(){
            Reset();
            todoList.Clear();
        }
        public void Add(params Action[] todo)
        {
            if(todo.Length==Depth)
                todoList.Add(todo);
            else 
                throw new ArgumentException("todo.Length != MaxStepCount. This mean that you added an array of functions bigger than it must be.");
        }
    }
}