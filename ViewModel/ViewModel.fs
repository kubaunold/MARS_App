//module MARSApp.ViewModel.ViewModel
//open MARSApp.ViewModel.ViewModelBase
//open MARSApp.ViewModel.ConfigurationViewModel
//open MARSApp.ViewModel.OptionViewModel
//open MARSApp.ViewModel.SimpleCommand
//open MARSApp.Model.OptionModel

namespace ViewModel

open Model
open System
open System.Collections.ObjectModel
open LiveCharts
open LiveCharts.Wpf
open LiveCharts.Defaults


type ViewModel() =
    inherit ViewModelBase()

    //let summary = 10.3

    let marketDataParameters    = ObservableCollection<ConfigurationViewModel>()
    let calculationParameters   = ObservableCollection<ConfigurationViewModel>()
    //let barChartNumbers         = ObservableCollection<ConfigurationViewModel>()
    let options                 = ObservableCollection<OptionViewModel>()
    //let summary                 = ObservableCollection<SummaryViewModel>()

    let getMarketDataParameters() = marketDataParameters |> Seq.map (fun conf -> (conf.Key , conf.Value)) |> Map.ofSeq
    let getCalculationParameters() = calculationParameters |> Seq.map (fun conf -> (conf.Key , conf.Value)) |> Map.ofSeq


    (* add some dummy data rows *)
    do
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::USDPLN"; Value = "3.65" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::USDEUR"; Value = "0.82" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "FX::EURGBP"; Value = "0.91" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "interestRate::percentage"; Value = "5" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "stock::price"; Value = "4.20" })
        //marketDataParameters.Add(ConfigurationViewModel { Key = "stock::drift"; Value = "4.20" }) //thats interestrate
        marketDataParameters.Add(ConfigurationViewModel { Key = "stock::volatility"; Value = "0.20" })
        marketDataParameters.Add(ConfigurationViewModel { Key = "stock::drift"; Value = "0.20" })


        calculationParameters.Add(ConfigurationViewModel { Key = "valuation::baseCurrency"; Value = "USD" })
        calculationParameters.Add(ConfigurationViewModel { Key = "option::steps"; Value = "200" })
        calculationParameters.Add(ConfigurationViewModel { Key = "option::seed"; Value = "5" })

        //barChartNumbers.Add(ConfigurationViewModel{Key = "one"; Value="1"})
        //barChartNumbers.Add(ConfigurationViewModel{Key = "twenty"; Value="20"})
        //barChartNumbers.Add(ConfigurationViewModel{Key = "seventy-one"; Value="71.12"})

        //summary.Add(SummaryViewModel{optionsCall=0; optionsPut=0; optionsTotal=0})


    (* charting *)
    let chartSeries = SeriesCollection()
    let predefinedChartFunctions = [| (fun x -> sin x); (fun x -> x); (fun x -> 2.*x); (fun x -> 2.*x - 3.) |] 
    let addChartSeriesFun _ =
        do
            let ls = LineSeries()
            let multiplier = System.Random().NextDouble()
            //let mapFun = predefinedChartFunctions.[ System.Random().Next(predefinedChartFunctions.Length) ]
            let mapFun = fun x -> 3.
            ls.Title <- sprintf "Test series %0.2f" multiplier
            //let series = seq { for i in 1 .. 100 do yield (0.01 * multiplier * double i) }
            let series = seq { for i in 1 .. 250 do float i }
            let a = (Seq.map mapFun series)
            ls.Values <- ChartValues<float> a
            chartSeries.Add(ls)
    let addChartSeries = SimpleCommand addChartSeriesFun
    (* add a few series for a good measure *)
    //do
    //    addChartSeriesFun ()
        //addChartSeriesFun ()

    //let clearSeries _ = chartSeries.Clear()
    let clearChartSeries = SimpleCommand (fun _ -> chartSeries.Clear ())


    (* market data commands *)
    let addMarketDataRecord     = SimpleCommand (fun _ -> marketDataParameters.Add(ConfigurationViewModel { Key = ""; Value = "" }))
    let removeMarketDataRecord  = SimpleCommand (fun record -> marketDataParameters.Remove(record :?> ConfigurationViewModel) |> ignore)
    let clearAllMarketData      = SimpleCommand (fun _ -> marketDataParameters.Clear ())

    (* calculation parameters commands *)
    let addCalcParameter            = SimpleCommand (fun _ -> calculationParameters.Add(ConfigurationViewModel { Key = ""; Value = "" }))
    let removeCalcParameterRecord   = SimpleCommand (fun record -> calculationParameters.Remove(record :?> ConfigurationViewModel) |> ignore)
    let clearCalcParameterRecord    = SimpleCommand (fun _ -> calculationParameters.Clear ())


    let addOption = SimpleCommand(fun _ -> 
        let currentCalculationParameters = getCalculationParameters()
        let currentMarketDataParameters = getMarketDataParameters()
        let optionToAdd = OptionRecord.Random currentCalculationParameters currentMarketDataParameters |> OptionViewModel
        optionToAdd.Calculate(currentMarketDataParameters, currentCalculationParameters)
        options.Add optionToAdd
        
        // add graph
        let ls = LineSeries()
        ls.Values <- ChartValues<float> optionToAdd.UnderlyingStock.StockHistory
        chartSeries.Add(ls)
    )




    // ":?>" operator converts to a type that's lower in hierarchy
    let removeOption        = SimpleCommand(fun option -> options.Remove(option :?> OptionViewModel) |> ignore)
    let clearOptions        = 
        SimpleCommand(fun _ ->
            options.Clear()
            chartSeries.Clear()
        )
    let recalculateOptions  = SimpleCommand(fun _ -> options |> Seq.iter(fun option -> option.Calculate(getMarketDataParameters(), getCalculationParameters())))






    (* Portolio's summary *)
    //member this.Summary = summary

    (* charting *)
    member this.ChartSeries = chartSeries
    member this.AddChartSeries = addChartSeries

    (* Parameters *)
    member this.MarketDataParameters = marketDataParameters
    member this.CalculationParameters = calculationParameters
    //member this.BarChartNumbers = barChartNumbers

    (* commands *)
    member this.AddOption                   = addOption
    member this.RemoveOption                = removeOption
    member this.ClearOptions                = clearOptions
    member this.RecalculateOptions          = recalculateOptions
    
    member this.AddCalcParameter    = addCalcParameter
    member this.RemoveCalcParameter = removeCalcParameterRecord 
    member this.ClearCalcParameter  = clearCalcParameterRecord 

    member this.AddMarketData       = addMarketDataRecord
    member this.RemoveMarketData    = removeMarketDataRecord
    member this.ClearAllMarketData  = clearAllMarketData

    (* Options *)
    member this.Options = options
    //member this.CountCall = countCall
    //member this.CountPut = countPut
    //member this.CountOptions = countOptions
