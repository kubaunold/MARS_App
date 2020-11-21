namespace ViewModel

open System
open System.Windows.Data
open System.Globalization
    
(* 
   Converter class to change the way how optional values are displayed. 
   Here we want to show empty string if the value is missing, and the string rendering of the value otherwise.

   What we don't want is to display Some .../None, and this class is handling this.
*)
type OptionTypeToStringConverter() =
    interface IValueConverter with
        member this.Convert (value : obj, _ : Type, _ : obj, _ : CultureInfo) =
            match value with
             | null -> null
             // ':?' - match expression; returns true if value matches specific type
             | :? Model.CallOrPutFlag as copf -> 
                // boxing a value wraps the value type inside a System.Object and therefore we can check if the object is null although it can only be "Put"/"Call". Then we have to unbox it back to it's original type after the null check
                box copf

        member this.ConvertBack (_ : obj, _ : Type, _ : obj, _ : CultureInfo) =
            raise( new NotImplementedException("OptionTypeToStringConverter.ConvertBack is not implemented yet."))