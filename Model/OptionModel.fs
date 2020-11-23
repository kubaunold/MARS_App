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


type OptionRecord =
    (* Model for Option Record. *)
    {
        OptionName:     string
        Expiry:         DateTime
        Strike:         float
        CallOrPutFlag:  CallOrPutFlag
        StockPrice:     float
    }

    (* Simple utility method for creating a random option. *)
    static member sysRandom = System.Random()
    static member Random(configuration : CalculationParameters) =
        let rnd  = System.Random()
        (* Below OptionRecord type  will be returned *)
        {
            OptionName  = sprintf "Option%03d" (OptionRecord.sysRandom.Next(999))
            Expiry      = (DateTime.Now.AddMonths(OptionRecord.sysRandom.Next(2, 12))).Date
            Strike      =  60.0 (*OptionRecord.sysRandom.NextDouble() * 60.*)
            CallOrPutFlag = 
                if rnd.Next()%2 |> System.Convert.ToBoolean then
                    Call
                else
                    Put
            StockPrice  = 58.60
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

    member this.BlackScholes() : float =
        let d1 = ( log(s0/k) + (r+v*v/2.)*t) / (v*sqrt(t))
        let d2 = d1 - v*sqrt(t)
        match call_or_put_flag with
        | Call -> s0*fi(d1) - k*exp(-r*t)*fi(d2)
        | Put ->  k*exp(-r*t)*fi(-d2) - s0*fi(-d1)
        //| Call -> 1.
        //| Put ->  2.

        //17.1 


