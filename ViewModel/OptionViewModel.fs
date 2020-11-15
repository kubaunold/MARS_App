module MARSApp.ViewModel.OptionViewModel

open MARSApp.ViewModel.ViewModelBase
open MARSApp.Model.OptionModel
open MARSApp.Model.ConfigurationModel

//Representation of an Option to the UI
type OptionViewModel(input : OptionRecord) = 
    inherit ViewModelBase()

    let mutable userInput = input

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

    member this.Strike
        with get() = userInput.Strike
        and set(x) =
            userInput <- {userInput with Strike = x }
            base.Notify("Strike")

    member this.CallOrPutFlag
        with get() = userInput.CallOrPutFlag
        and set(x) =
            userInput <- {userInput with CallOrPutFlag = x}
            base.Notify("CallOrPutFlag")

    (* Invoke the option valuation using Black-Scholes Model *)
    member this.Calculate()


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


    // Invoke the valuation based on user input
    member this.Calculate(data : DataConfiguration, calculationParameters : CalculationConfiguration) = 
        
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
        //calculate
        let calcTuple  = OptionValuationModel(optionInputs).Calculate()

        let BScall = (match calcTuple with (a,_,_,_,_) -> a)
        let BScallDelta = (match calcTuple with (_,b,_,_,_) -> b)
        let BSput = (match calcTuple with (_,_,c,_,_) -> c)
        let BSputDelta = (match calcTuple with (_,_,_,d,_) -> d)

        let gbm = (match calcTuple with (_,_,_,_,e) -> e)

        //do
        //    let ls = LiveCharts.Wpf.LineSeries()
        //    let series = gbm
        //    ls.Values <- LiveCharts.ChartValues<float> series
        //    chartSeries.Add(ls)

        


        //present to the user
        this.BScall <- Option.Some (BScall)
        this.BScallDelta <- BScallDelta
        this.BSput <- Option.Some (BSput)
        this.BSputDelta <- BSputDelta

    //type ChartViewModel(input:ChartInputs) =
    //    inherit ViewModelBase()

    //    member this.SimulateGBM(data : DataConfiguration, calculationParameters : CalculationConfiguration) =
    //        let calc = ChartValuationModel(input).SimulateGBM()
    //        calc
