//using HotelSchedulerControl.Scheduler;
//using System;
//using System.Collections.Generic;
//using System.Linq;


//namespace HotelSchedulerControl.Scheduler
//{
//    /// <summary>
//    /// Wrapper ProjectManager class
//    /// </summary>
//    [Serializable]
//    public class Scheduler : Scheduler<SchedulerEvent, object>
//    {
//    }

//    /// <summary>
//    /// Concrete ProjectManager class for the IProjectManager interface
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <typeparam name="R"></typeparam>
//    [Serializable]
//    public class Scheduler<T, R> : IScheduler<T, R>
//        where T : SchedulerEvent
//        where R : class
//    {
//        HashSet<T> _mRegister = new HashSet<T>();
//        List<T> _mRootTasks = new List<T>();
//        Dictionary<T, int> _mTaskIndices = new Dictionary<T, int>(); // Map the task to its zero-based index order position

//        /// <summary>
//        /// Create a new Project
//        /// </summary>
//        public Scheduler()
//        {
//            Start = DateTime.Now;
//            Now = DateTime.Now - Start;
//        }

//        /// <summary>
//        /// Get or set the TimeSpan we are at now from Start DateTime
//        /// </summary>
//        public TimeSpan Now { get; set; }

//        /// <summary>
//        /// Get or set the starting date for this project
//        /// </summary>
//        public DateTime Start { get; set; }

//        /// <summary>
//        /// Get the date after the specified TimeSpan
//        /// </summary>
//        /// <param name="span"></param>
//        /// <returns></returns>
//        public DateTime GetDateTime(TimeSpan span)
//        {
//            return this.Start.Add(span);
//        }

//        /// <summary>
//        /// Create a new T for this Project and add it to the T tree
//        /// </summary>
//        /// <returns></returns>
//        public void Add(T task)
//        {
//            if (!this._mRegister.Contains(task))
//            {
//                _mRegister.Add(task);
//                _mRootTasks.Add(task);
//            }
//        }
//        /// <summary>
//        /// Remove task from this Project
//        /// </summary>
//        /// <param name="task"></param>
//        public void Delete(T task)
//        {
//            if (task != null)
//            {
//                _mRootTasks.Remove(task);
//                _mRegister.Remove(task);
//            }
//        }
//        /// <summary>
//        /// Get the zero-based index of the task in this Project
//        /// </summary>
//        /// <param name="task"></param>
//        /// <returns></returns>
//        public int IndexOf(T task)
//        {
//            if (_mRegister.Contains(task))
//            {
//                if (_mTaskIndices.ContainsKey(task))
//                    return _mTaskIndices[task];

//                int i = 0;
//                foreach (var x in Tasks)
//                {
//                    if (x.Equals(task))
//                    {
//                        _mTaskIndices[task] = i;
//                        return i;
//                    }
//                    i++;
//                }
//            }
//            return -1;
//        }
//        /// <summary>
//        /// Re-order position of the task by offset amount of places
//        /// If task is moved between members, the task is added to the members' group
//        /// If task is a member and it is moved above it's group or below last sibling member, then it is moved out of its group
//        /// If task is a part, then its parent split-task will be move instead
//        /// </summary>
//        /// <param name="task"></param>
//        /// <param name="offset"></param>
//        public void Move(T task, int offset)
//        {
//            if (task != null && _mRegister.Contains(task) && offset != 0)
//            {
//                int indexoftask = IndexOf(task);
//                if (indexoftask > -1)
//                {
//                    int newindexoftask = indexoftask + offset;
//                    // check for out of index bounds
//                    var taskcount = Tasks.Count();
//                    if (newindexoftask < 0) newindexoftask = 0;
//                    else if (newindexoftask > taskcount) newindexoftask = taskcount;
//                    // get the index of the task that will be displaced
//                    var displacedtask = Tasks.ElementAtOrDefault(newindexoftask);

//                    if (displacedtask == task)
//                    {
//                        return;
//                    }
//                    if (displacedtask == null)
//                    {
//                        // adding to the end of the task list
//                        _DetachTask(task);
//                        _mRootTasks.Add(task);
//                    }
//                    _RecalculateSlack();
//                    // clear indices since positions changed
//                    _mTaskIndices.Clear();
//                }
//            }
//        }
//        /// <summary>
//        /// Get the T tree
//        /// </summary>
//        public IEnumerable<T> Tasks
//        {
//            get
//            {
//                var stack = new Stack<T>(1024);
//                var rstack = new Stack<T>(30);
//                foreach (var task in _mRootTasks)
//                {
//                    stack.Push(task);
//                    while (stack.Count > 0)
//                    {
//                        var visited = stack.Pop();
//                        yield return visited;
//                        while (rstack.Count > 0) stack.Push(rstack.Pop());
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// Set the start value. Affects group start/end and dependants start time.
//        /// </summary>
//        public void SetStart(T task, TimeSpan value)
//        {
//            if (_mRegister.Contains(task) && value != task.Start)
//            {
//                _SetStartHelper(task, value);
//                _RecalculateSlack();
//            }
//            // Set start for a group task
//            else if (_mRegister.Contains(task) && value != task.Start)
//            {
//                _RecalculateSlack();
//            }
//        }

//        /// <summary>
//        /// Set the end time. Affects group end and dependants start time.
//        /// </summary>
//        public void SetEnd(T task, TimeSpan value)
//        {
//            if (_mRegister.Contains(task) && value != task.End)
//            {
//                this._SetEndHelper(task, value);
//                _RecalculateSlack();
//            }
//        }

//        /// <summary>
//        /// Set the duration of the specified task from start to end.
//        /// </summary>
//        /// <param name="task"></param>
//        /// <param name="duration">Number of timescale units between ProjectManager.Start</param>
//        public void SetDuration(T task, TimeSpan duration)
//        {
//            this.SetEnd(task, task.Start + duration);
//        }
//        /// <summary>
//        /// Detach the specified task from ProjectManager.Tasks (i.e. remove from its parent group, or if not it goes not have a parent group, unregister from root task status).
//        /// The specified task will remain registered in ProjectManager.
//        /// After execution of this helper method, the task is expected to be re-attached to ProjectManager.Tasks by regaining root task status, or joining a new group.
//        /// </summary>
//        /// <param name="task"></param>
//        private void _DetachTask(T task)
//        {
//            _mRootTasks.Remove(task);

//        }

//        private void _SetStartHelper(T task, TimeSpan value)
//        {
//            if (task.Start != value)
//            {
//                // check out of bounds
//                if (value < TimeSpan.Zero) value = TimeSpan.Zero;
//                // save offset just in case we need to use for moving task parts
//                var offset = value - task.Start;
//                // cache value
//                task.Duration = task.End - task.Start;
//                task.Start = value;

//                // affect self
//                task.End = task.Start + task.Duration;
//            }
//        }
//        private void _SetEndHelper(T task, TimeSpan value)
//        {
//            if (task.End != value)
//            {
//                if (value <= task.Start) value = task.Start + TimeSpan.FromMinutes(30); // end cannot be less than start
//                task.End = value;
//                task.Duration = task.End - task.Start;
//            }
//        }
//        private void _RecalculateSlack()
//        {
//            var max_end = this.Tasks.Max(x => x.End);
//            foreach (var task in this.Tasks)
//            {                    // no dependants, so we have all the time until the last task ends
//                task.Slack = max_end - task.End;
//            }
//        }
//    }    
//}

