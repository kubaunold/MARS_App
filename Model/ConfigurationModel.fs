module MARSApp.Model.ConfigurationModel

type ConfigurationRecord = 
    {
        Key : string
        Value : string
    }
    
type DataConfiguration = Map<string, string>
type CalculationConfiguration = Map<string, string>