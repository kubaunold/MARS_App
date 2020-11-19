//module MARSApp.Model.ConfigurationModel
namespace Model

type ConfigurationRecord = 
    {
        Key : string
        Value : string
    }
    
type MarketData = Map<string, string>
type CalculationParameters = Map<string, string>