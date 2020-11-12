module MARSApp.ViewModel.OptionViewModel

open MARSApp.ViewModel.ViewModelBase
open MARSApp.Model.OptionModel


//Representation of an Option to the UI
type OptionViewModel(input : OptionRecord) = 
    inherit ViewModelBase()

    let mutable userInput = input
    let mutable _BScall : Money option = None
    let mutable _BScallDelta : float  = 0.0
    let mutable _BSput : Money option = None
    let mutable _BSputDelta : float  = 0.0


    member this.OptionName 
        with get() = userInput.OptionName
        and set(x) = 
            userInput <- {userInput with OptionName = x }
            base.Notify("OptionName")

    member this.Expiry 
        with get() = userInput.Expiry
        and set(x) = 
            userInput <- {userInput with Expiry = x }
            base.Notify("Expiry")

    member this.Currency 
        with get() = userInput.Currency
        and set(x) = 
            userInput <- {userInput with Currency = x }
            base.Notify("Currency")

    member this.Strike
        with get() = userInput.Strike
        and set(x) =
            userInput <- {userInput with Strike = x }
            base.Notify("Strike")

    member this.BScall
        with get() = _BScall
        and set(x) =
            _BScall <- x
            base.Notify("BScall")

    member this.BScallDelta
        with get() = _BScallDelta
        and set(x) =
            _BScallDelta <- x
            base.Notify("BScallDelta")

    member this.BSput
        with get() = _BSput
        and set(x) =
            _BSput <- x
            base.Notify("BSput")

    member this.BSputDelta
        with get() = _BSputDelta
        and set(x) =
            _BSputDelta <- x
            base.Notify("BSputDelta")

    member this.Simulate(data : DataConfiguration, calculationParameters : CalculationConfiguration) = 
        //capture inputs
        let optionInputs : OptionValuationInputs = 
            {
                OptionType = 
                         {
                             OptionName  = this.OptionName
                             Expiry      = this.Expiry
                             Currency    = this.Currency
                             Strike      = this.Strike
                         }
                Data = data
                CalculationsParameters = calculationParameters
            }
        let calc = OptionValuationModel(optionInputs).SimulateGBM()
        calc
