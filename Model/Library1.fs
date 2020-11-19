namespace Model
module Distributions =

    open MathNet.Numerics.Distributions

    let normal = new Normal(0.0, 1.0)
    let mean = normal.Mean
    let variance = normal.Variance
    let stddev = normal.StdDev


    normal.Sample() |> ignore

    for a in 1..5 do
        printf "%d" a

module BlackScholesQuantitatioveFinanceModel =
    open MathNet.Numerics.Distributions

    let a = Normal.CDF(0., 1., 1.)

    let pow x n = exp(n * log(x))
    type PutCallFlag = Put | Call
    /// Cumulative distribution function
    let cnd x =
        let a1 = 0.31938153
        let a2 = -0.356563782
        let a3 = 1.781477937
        let a4 = -1.821255978
        let a5 = 1.330274429
        let pi = 3.141592654
        let l = abs(x)
        let k = 1.0 / (1.0 + 0.2316419 * l)
        let w = (1.0-1.0/sqrt(2.0*pi)*exp(-l*l/2.0)*(a1*k+a2*k*k+a3*(pow k 3.0)+a4*(pow k 4.0)+a5*(pow k 5.0)))
        if x < 0.0 then 1.0 - w else w

    /// Black-Scholes
    // call_put_flag: Put | Call
    // s: stock price
    // x: strike price of option
    // t: time to expiration in years
    // r: risk free interest rate
    // v: volatility
    let black_scholes call_put_flag s x t r v =
         let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
         let d2=d1-v*sqrt(t)
         //let res = ref 0.0
    
         match call_put_flag with
         | Put -> x*exp(-r*t)*cnd(-d2)-s*cnd(-d1)
         | Call -> s*cnd(d1)-x*exp(-r*t)*cnd(d2) 
    
    /// Convert the nr of days to years
    let days_to_years d = (float d) / 365.25

    //black_scholes Call 58.60 60.0 (days_to_years 20) 0.01 0.3

//module VisualizeBlackScholes =
//    /// Plot price of option as function of time left to maturity
//    #r "System.Windows.Forms.DataVisualization.dll"
//    open System
//    open System.Net
//    open System.Windows.Forms
//    open System.Windows.Forms.DataVisualization.Charting
//    open Microsoft.FSharp.Control.WebExtensions
//    open System.Windows.Forms.DataVisualization

//    open BlackScholesQuantitatioveFinanceModel

//    /// Create chart and form
//    let chart = new Chart(Dock = DockStyle.Fill)
//    let area = new ChartArea("Main")
//    chart.ChartAreas.Add(area)
//    chart.Legends.Add(new Legend())

//    let mainForm = new Form(Visible = true, TopMost = true, Width = 700, Height = 500)
//    do mainForm.Text <- "Option price as a function of time"
//    mainForm.Controls.Add(chart)

//    /// Create series for call option price
//    let optionPriceCall = new Series("Call option price")
//    do optionPriceCall.ChartType <- SeriesChartType.Line
//    do optionPriceCall.BorderWidth <- 2
//    do optionPriceCall.Color <- Drawing.Color.Red
//    chart.Series.Add(optionPriceCall)

//    /// Create series for put option price
//    let optionPricePut = new Series("Put option price")
//    do optionPricePut.ChartType <- SeriesChartType.Line
//    do optionPricePut.BorderWidth <- 2
//    do optionPricePut.Color <- Drawing.Color.Blue
//    chart.Series.Add(optionPricePut)

//    /// Calculate and plot call option prices
//    let opc = [for x in [(days_to_years 20)..(-(days_to_years 1))..0.0] do yield black_scholes Call 58.60 60.0 x 0.01 0.3]
//    do opc |> Seq.iter (optionPriceCall.Points.Add >> ignore)

//    /// Calculate and plot put option prices
//    let opp = [for x in [(days_to_years 20)..(-(days_to_years 1))..0.0] do yield black_scholes Put 58.60 60.0 x 0.01 0.3]
//    do opp |> Seq.iter (optionPricePut.Points.Add >> ignore)


module test=

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
            //match call_or_put_flag with
            //| Call -> s0*Normal.CDF(0., 1., d1) - k*exp(-r*t)*Normal.CDF(0., 1., d2)
            //| Put -> Normal.CDF(0., 1., -d2)*k*exp(-r*t) - Normal.CDF(0., 1., -d1)*s0
    
            17.1








module Others =
    type Class1() = 
        member this.X = "F#"

(*
module YahooFinance =
    open System
    open System.Net
    open FSharp.Charting
    open Microsoft.FSharp.Control.WebExtensions
    //open System.Windows.
    open System.Windows.Forms.DataVisualization.Charting

    //fsi.AddPrinter(fun (ch:FSharp.Charting.ChartTypes.GenericChart) -> ch.ShowChart(); "FSharpCharting")

    // Syncronous fetching (just one stock here)
    let fetchOne() =
        let u = "https://www.stats.govt.nz/assets/Uploads/New-Zealand-business-demography-statistics/New-Zealand-business-demography-statistics-At-February-2020/Download-data/Geographic-units-by-industry-and-statistical-area-2000-2020-descending-order-CSV.zip"
        //let uri = new System.Uri("http://ichart.finance.yahoo.com/table.csv?s=ORCL&d=9&e=23&f=2012&g=d&a=2&b=13&c=2012&ignore=.csv")
        let uri = new System.Uri(u)
        let client = new WebClient()
        let html = client.DownloadString(uri)
        html
*)