namespace LaZula
module Utils =
    [<Interface>]
    type ICommand = 
           abstract member Execute : unit -> unit
           abstract member CanExecute : unit -> bool
           abstract member Arguments : Map<string, string>
    type Config = {
         cmd_key: string    
    }