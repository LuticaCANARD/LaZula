namespace LaZula.SystemController

open System
open System.Threading.Tasks
open System.Threading

type TaskActions =
    struct
         val task:Task
         val token:CancellationTokenSource
         new (task:unit->unit) = 
            let cs = new CancellationTokenSource()
            let _taskApply = Task.Factory.StartNew(task,cs.Token)
            {
                task=_taskApply
                token=cs
            }
    end

[<Struct>]
type SystemCancelController = {
    serverTask : TaskActions
    uiTask : TaskActions
}