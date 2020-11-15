module MARSApp.Model.ConfigurationModel

type ConfigurationRecord = 
    {
        Key : string
        Value : string
    }
    
type MarketData = Map<string, string>
type CalculationConfiguration = Map<string, string>