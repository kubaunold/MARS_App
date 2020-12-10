//module MARSApp.Model.OptionModel
namespace Model
//open MARSApp.Model.ConfigurationModel
open System // used for DateTime
open MathNet.Numerics.Distributions

(* Simple type that determines whether the option is Put or Call
    Call - Owner has right to buy an underlying asset at given price
    Put - Owner has right to sell an underlying asset for a given price
*)
type CallOrPutFlag =
    | Call
    | Put
    override this.ToString() =
               match this with
               | Call -> "Call"
               | Put -> "Put"

type Stock =
    // Model for Stock Record
    {
        StockHistory:   float list
    }
    (* Simple utility method for creating a random option. *)
    static member sysRandom = System.Random()
    static member Random (calc: CalculationParameters) (market : MarketData) =
        let rnd  = System.Random()
        (* Below StockRecord type  will be returned *)
        {
            StockHistory =
                //generates list of n Uniform RVs from interval [0,1]; here it's [0,1)
                let genRandomNumbersNominalInterval (count:int) (seed:int) : float list =
                    let rnd = System.Random(seed)
                    List.init count (fun _ -> rnd.NextDouble())

                //input: UniformRM need to be from interval (0,1]
                //input: steps must be even
                //output: NormalRV have mean=0 and standard_deviation=1
                let normalizeRec (uniformList:float list) (n:int) : float list =
                    let rec buildNormalList (normalList:float list) =
                        if normalList.Length = n then normalList
                        else
                            let currentNIdOne = normalList.Length
                            let currentNIdTwo = currentNIdOne + 1
                            let oneU = uniformList.[currentNIdOne]
                            let twoU = uniformList.[currentNIdTwo]
                            let oneN = sqrt(-2.*Math.Log(oneU, Math.E))*sin(2.*Math.PI*twoU)
                            let twoN = sqrt(-2.*Math.Log(oneU, Math.E))*cos(2.*Math.PI*twoU)
                            let newUniforms = [oneN; twoN]
                            buildNormalList (normalList@newUniforms)
                    buildNormalList []

                let simulateGBM (count:int) (steps:int) (price:float) (drift:float) (vol:float) (years:float) (seed:int) =
                    let normalRV = normalizeRec (genRandomNumbersNominalInterval steps seed) steps
                    //build stock prices list
                    let rec buildStockPricesList (currentStockPricesList:float list) (steps:int) (normalId:int) : float list =
                        if normalId = steps-1 then currentStockPricesList
                        else
                            let firstExpTerm =  (drift - (vol**2.)/2.) * (float(years)/float(steps))
                            let secondExpTerm =  vol * sqrt(float(years)/float(steps)) * normalRV.[normalId]
                            let newStockPrice = currentStockPricesList.[normalId] * Math.E ** (firstExpTerm + secondExpTerm)
                            buildStockPricesList (currentStockPricesList@[newStockPrice]) steps (normalId+1)
                    let stockPricesList = buildStockPricesList [price] steps 0
                    stockPricesList

                //let count = 1000
                //let steps   = 250 //must be EVEN!
                let steps =
                    match calc.TryFind "option::steps" with
                    | Some steps -> int steps
                    | None -> 200 // default 200 steps
                //let price   = System.Random().NextDouble() * 10.
                let price =
                    match market.TryFind "stock::price" with
                    | Some price -> float price
                    | None -> 6.7 // default 6.7$ for price
                //let drift   = System.Random().NextDouble()
                //let drift = 0.2
                let drift =
                    match market.TryFind "stock::drift" with
                    | Some drift -> float drift
                    | None -> 0.4 // default 6.7$ for price
                //let vol     = System.Random().NextDouble()
                let vol =
                    match market.TryFind "stock::volatility" with
                    | Some vol -> float vol
                    | None -> 0.2
                //let years   = System.Random().NextDouble()
                let years = 1.0
                //let seed    = System.Random().Next()
                let seed =
                    match calc.TryFind "option::seed" with
                    | Some seed -> int seed
                    | None -> 5 // default
                let series  = simulateGBM 1 steps price drift vol years seed
                
                series  // return this float list
        }


type OptionRecord =
    (* Model for Option Record. *)
    {
        OptionName:     string
        Expiry:         DateTime
        Strike:         float
        CallOrPutFlag:  CallOrPutFlag
        StockPrice:     float
        UnderlyingStock:Stock
    }

    (* Simple utility method for creating a random option. *)
    static member sysRandom = System.Random()
    static member Random (calc : CalculationParameters) (market : MarketData) =
        let rnd  = System.Random()
        (* Below OptionRecord type  will be returned *)
        {
            OptionName      = sprintf "Option%03d" (OptionRecord.sysRandom.Next(999))
            Expiry          = (DateTime.Now.AddMonths(OptionRecord.sysRandom.Next(2, 12))).Date
            //Strike          =  60.0 (*OptionRecord.sysRandom.NextDouble() * 60.*)
            Strike          =
                let s =
                    match market.TryFind "stock::price" with
                    | Some price -> float price
                    | None -> 6.70  // default 6.70$ for stock price
                s*1.1   //Strike price is 110% of stock price
            
            CallOrPutFlag   = 
                if rnd.Next()%2 |> System.Convert.ToBoolean then
                    Call
                else
                    Put
            
            StockPrice      =
                let s =
                    match market.TryFind "stock::price" with
                    | Some price -> float price
                    | None -> 6.70  // default 6.70$ for stock price
                s
            UnderlyingStock = Stock.Random (calc: CalculationParameters) (market : MarketData)
        }


type OptionValuationInputs =
    (* Type containing inputs for option valuation model *)
    {
        Option: OptionRecord
        MarketData: MarketData
        CalculationParameters: CalculationParameters
        UnderlyingAssetPrice: float
    }

type OptionValutionModel (inputs: OptionValuationInputs) =
    
    (* Black-Scholes for Option Valuation
    Parameters:
        call_or_put_flag (CallOrPutFlag):
            Flag that determines whether this option is put or call
        s0 (float): Underlying asset price at the time being
        k (float): strike price
        t (float): expiry date (counted in years from now on, e.g. 1.5)
        r (float): risk free rate (e.g. 0.05 means 5%)
        v (float): volatility (e.g. 0.2 means 20%)
    *)
    let call_or_put_flag = inputs.Option.CallOrPutFlag
    //let s0  = inputs.UnderlyingAssetPrice
    let s0  = inputs.Option.StockPrice
    let k   = inputs.Option.Strike
    let t   = inputs.Option.Expiry.Subtract(System.DateTime.Now.Date).TotalDays/365.
    let r   = match inputs.MarketData.TryFind "riskFreeInterestRate::percentage" with
                | Some rate -> float rate
                | None -> 0.01  // default 1% interest rate
    let v   = match inputs.MarketData.TryFind "stock::volatility" with
                | Some volatility -> float volatility
                | None -> 0.30   // default 30% volatility
    
    // fi(x) is Cumulative Standard! (mean of 0 and st. deviation of 1) Distribution Function aka Standardized CDF
    let fi x = Normal.CDF(0., 1., x)

    (* Black-Scholes for Option Valuation
    Parameters:
        call_or_put_flag (CallOrPutFlag):
            Flag that determines whether this option is put or call
        s0 (float): Underlying asset price at the time being
        k (float): strike price
        t (float): expiry date (counted in years from now on, e.g. 1.5)
        r (float): risk free rate (e.g. 0.05 means 5%)
        v (float): volatility (e.g. 0.2 means 20%)
    *)
    member this.BlackScholes() : float =
        let d1 = ( log(s0/k) + (r+v*v/2.)*t) / (v*sqrt(t))
        let d2 = d1 - v*sqrt(t)
        match call_or_put_flag with
        | Call -> s0*fi(d1) - k*exp(-r*t)*fi(d2)
        | Put ->  k*exp(-r*t)*fi(-d2) - s0*fi(-d1)
