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
        Temp:           float
    }

    (* Simple utility method for creating a random option. *)
    static member sysRandom = System.Random()
    static member Random(configuration : CalculationParameters) =

        let rnd  = System.Random()

        (* Below OptionRecord type  will be returned *)
        {
            OptionName  = sprintf "Option%04d" (OptionRecord.sysRandom.Next(9999))
            Expiry      = (DateTime.Now.AddMonths(OptionRecord.sysRandom.Next(2, 12))).Date
            Strike      = OptionRecord.sysRandom.NextDouble()
            CallOrPutFlag = 
                if rnd.Next()%2 |> System.Convert.ToBoolean then
                    Call
                else
                    Put
            Temp        = 17.0
        }


type OptionValuationInputs =
    (* Type containing inputs for option valuation model *)
    {
        Option: OptionRecord
        MarketData: MarketData
        CalculationParameters: CalculationParameters
        UnderlyingAssetPrice: float
    }

type OptionValutaionModel (inputs: OptionValuationInputs) =
    
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
    let s0  = inputs.UnderlyingAssetPrice
    let k   = inputs.Option.Strike
    let t   = inputs.Option.Expiry.Subtract(System.DateTime.Now.Date).TotalDays/365.
    let r   = match inputs.MarketData.TryFind "riskFreeInterestRate::percentage" with
                | Some rate -> float rate
                | None -> 0.05  // default 5% interest rate
    let v   = match inputs.MarketData.TryFind "stock::volatility" with
                | Some volatility -> float volatility
                | None -> 0.20   // default 20% volatility
    
    member this.BlackScholes() : float =
        let d1 = (log(s0/k) + (r + v**2./2.)*t) / (v*sqrt(t)) 
        let d2 = d1 - v*sqrt(sqrt(t))
        match call_or_put_flag with
        | Call -> s0*Normal.CDF(0., 1., d1) - k*exp(-r*t)*Normal.CDF(0., 1., d2)
        | Put -> Normal.CDF(0., 1., -d2)*k*exp(-r*t) - Normal.CDF(0., 1., -d1)*s0
