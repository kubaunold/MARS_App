//module MARSApp.ViewModel.OptionViewModel
namespace ViewModel
//open MARSApp.ViewModel.ViewModelBase
//open MARSApp.Model.OptionModel
//open MARSApp.Model.ConfigurationModel
open Model

//Representation of an Option to the UI
type OptionViewModel(input : OptionRecord) = 
    inherit ViewModelBase()

    let mutable userInput = input
    // Result of option valuation using BS_Model
    let mutable value : float option = None

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

    member this.Value
        with get() = value
        and set(x) = 
            value <- x
            base.Notify("Value")

    member this.StockPrice
        with get() = userInput.StockPrice
        and set(x) = 
            userInput <- {userInput with StockPrice = x }
            base.Notify("StockPrice")

    member this.UnderlyingStock
        with get() = userInput.UnderlyingStock
        and set(x) = 
            userInput <- {userInput with UnderlyingStock = x }
            base.Notify("UnderlyingStock")

    (* Invoke the option valuation using Black-Scholes Model *)
    member this.Calculate(marketData: MarketData, calculationParameters: CalculationParameters) =
        
        // capture inputs
        let optionValuationInputs: OptionValuationInputs =
            {
                Option =
                    {
                        OptionName      = this.OptionName
                        Expiry          = this.Expiry
                        Strike          = this.Strike
                        CallOrPutFlag   = this.CallOrPutFlag
                        StockPrice      = this.StockPrice
                        UnderlyingStock = this.UnderlyingStock
                    }
                MarketData              = marketData
                CalculationParameters   = calculationParameters
                UnderlyingAssetPrice    = 167.
            }

        // run Black-Scholes model
        let optionPrice = OptionValutionModel(optionValuationInputs).BlackScholes()

        // present to user
        this.Value <- Option.Some(optionPrice)
        //this.Value <- optionPrice

