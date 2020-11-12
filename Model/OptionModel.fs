module MARSApp.Model.OptionModel

(* Model for Option trade. *)
type OptionRecord =    //record as well
    {
        OptionName : string
        Expiry    : DateTime
        Currency  : string
        Strike : float
        //BSput : float
        //BSputDelta : float
        //BScall : float
        //BScallDelta : float
    }


    (* Simple utility method for creating a random option. *)
    static member sysRandom = System.Random()
    static member Random(configuration : CalculationConfiguration) = 
        (* We pick a random currency either from given short list, or from valuation::knownCurrencies config key *)
        let knownCurrenciesDefault = [| "EUR"; "USD"; "PLN"; |]
        
        let knownCurrencies = if configuration.ContainsKey "valuation::knownCurrencies" 
                              then configuration.["valuation::knownCurrencies"].Split([|' '|])
                              else knownCurrenciesDefault


        let rnd  = System.Random()
        //let r = rnd.Next()%2 |> System.Convert.ToBoolean

        
        {
            OptionName  = sprintf "Option%04d" (OptionRecord.sysRandom.Next(9999))
            Expiry      = (DateTime.Now.AddMonths (OptionRecord.sysRandom.Next(2, 12))).Date
            Currency    = knownCurrencies.[ OptionRecord.sysRandom.Next(knownCurrencies.Length) ]
            Strike      = OptionRecord.sysRandom.NextDouble()
            //BSput       = OptionRecord.sysRandom.NextDouble()
            //BSputDelta  = OptionRecord.sysRandom.NextDouble()
            //BScall      = OptionRecord.sysRandom.NextDouble()
            //BScallDelta = OptionRecord.sysRandom.NextDouble()
        }


