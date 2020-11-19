//module MARSApp.ViewModel.ViewModel
namespace ViewModel
open Model
//open MARSApp.ViewModel.ViewModelBase
//open MARSApp.ViewModel.ConfigurationViewModel
//open MARSApp.ViewModel.OptionViewModel
//open MARSApp.ViewModel.SimpleCommand

//open MARSApp.Model.OptionModel

open System.Collections.ObjectModel

type ViewModel() =
    inherit ViewModelBase()

    let summary = 10
    let pieChart = 7

    let marketDataParameters    = ObservableCollection<ConfigurationViewModel>()
    let calculationParameters   = ObservableCollection<ConfigurationViewModel>()
    let options                 = ObservableCollection<OptionViewModel>()

    let getMarketDataParameters() = marketDataParameters |> Seq.map (fun conf -> (conf.Key , conf.Value)) |> Map.ofSeq
    let getCalculationParameters() = calculationParameters |> Seq.map (fun conf -> (conf.Key , conf.Value)) |> Map.ofSeq


    (* add some dummy data rows *)
    do
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::USDPLN"; Value = "3.76" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::USDEUR"; Value = "0.87" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::EURGBP"; Value = "0.90" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "interestRate::percentage"; Value = "5" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "stock::price"; Value = "4.20" })
        //marketDataParameters.Add(ConfigurationViewModel { Key = "stock::drift"; Value = "4.20" }) //thats interestrate
        marketDataParameters.Add(ConfigurationViewModel { Key = "stock::volatility"; Value = "0.20" })

        calculationParameters.Add(ConfigurationViewModel { Key = "monteCarlo::runs"; Value = "100" })
        calculationParameters.Add(ConfigurationViewModel { Key = "valuation::baseCurrency"; Value = "USD" })
        calculationParameters.Add(ConfigurationViewModel { Key = "valuation::knownCurrencies"; Value = "USD PLN EUR GBP" })
        calculationParameters.Add(ConfigurationViewModel { Key = "methodology::bumpRisk"; Value = "True" })
        calculationParameters.Add(ConfigurationViewModel { Key = "methodology::bumpSize"; Value = "0.0001" })
        calculationParameters.Add(ConfigurationViewModel { Key = "valuation::deferredHaircut"; Value = "1.5" })
        calculationParameters.Add(ConfigurationViewModel { Key = "option::steps"; Value = "200" })
        calculationParameters.Add(ConfigurationViewModel { Key = "option::seed"; Value = "5" })

    //do
    //    summary.Add(7)
    

    //let calculateFun _ = do
    //        trades |> Seq.iter(fun trade -> trade.Calculate(getDataConfiguration (), getCalculationConfiguration ()))
    //        refreshSummary()

    //let calculate = SimpleCommand calculateFun



    //let calculateOptionsFun _ = do
    //        options |> Seq.iter(fun option -> option.Calculate(getDataConfiguration (), getCalculationConfiguration ()))
    //        //refreshSummary()

    //let calculateOptions = SimpleCommand calculateOptionsFun
    //let addOption = SimpleCommand(fun _ -> 
    //        let currentConfig = getCalculationConfiguration ()
    //        OptionRecord.Random currentConfig |> OptionViewModel |> options.Add
    //        )

    (* option commands *)
    let addOption = SimpleCommand(fun _ -> 
        let currentConfig = getCalculationParameters()
        OptionRecord.Random currentConfig |> OptionViewModel |> options.Add
        )
    // ":?>" operator converts to a type that's lower in hierarchy
    let removeOption        = SimpleCommand(fun option -> options.Remove(option :?> OptionViewModel) |> ignore)
    let clearOptions        = SimpleCommand(fun _ -> options.Clear())
    let recalculateOptions  = SimpleCommand(fun _ -> options |> Seq.iter(fun option -> option.Calculate(getMarketDataParameters(), getCalculationParameters())))

    (* Portolio's summary *)
    member this.Summary = summary

    (* Charting *)
    member this.PieChart = pieChart

    (* Parameters *)
    member this.MarketDataParameters = marketDataParameters
    member this.CalculationParameters = calculationParameters

    (* commands *)
    member this.AddOption = addOption
    member this.RemoveOption = removeOption
    member this.ClearOptions = clearOptions
    member this.RecalculateOptions = recalculateOptions

    (* Options *)
    member this.Options = options











    //let mutable firstName = "Kuba"
    //let mutable lastName = ""
 
    //member this.FirstName
    //    with get() = firstName 
    //    and set(value) =
    //        firstName <- value
    //        base.NotifyPropertyChanged(<@ this.FirstName @>)
 
    //member this.LastName
    //    with get() = lastName 
    //    and set(value) =
    //        lastName <- value
    //        base.NotifyPropertyChanged(<@ this.LastName @>)
 
    //member this.GetFullName() = 
    //    sprintf "%s %s" (this.FirstName) (this.LastName)